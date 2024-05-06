using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(NavMeshAgent))]

public class AIPathFinding : MonoBehaviour
{
    NavMeshAgent navAgent;

    //到着しているか
    public bool IsAlived
    {
        get
        {
            //パス計算中
            if (navAgent.pathPending) return false;
            //止まっている
            if(navAgent.isStopped) return true;
            
            return navAgent.remainingDistance <= navAgent.stoppingDistance;
        }
    }

    public Vector3 DesiredVelecity => navAgent.desiredVelocity;
    // Start is called before the first frame update
    void Awake()
    {
        TryGetComponent(out navAgent);
        //移動は手動で行う
        navAgent.angularSpeed = 0f;
        navAgent.acceleration = 0f;
        navAgent.updatePosition = false;
        navAgent.updateRotation = false;

        navAgent.isStopped = true;

    }
    public void Stop()
    {
        navAgent.isStopped = true;
    }
    void Update()
    {
        //パス計算中
        if (navAgent.pathPending) return;

        //到着している
        if(IsAlived)
        {
            navAgent.isStopped = true;
        }


        //エージェントの座標更新
        navAgent.nextPosition = transform.position;   
    }
    //目的地設定
    public void SetDestination(Vector3 position)
    {
        navAgent.isStopped = false;
        navAgent.SetDestination(position);
    }
    /// <summary>
    ///  制限距離を加味した目的地に変換
    /// </summary>
    /// <param name="destPos">目的地</param>
    /// <param name="calcCorners">経路を格納</param>
    /// <param name="limitRange">制限距離</param>
    /// <returns>指定された目的地に（再計算無しで）到着できるか</returns>

    public bool CalcCornersFromRange(ref Vector3 destPos,ref List<Vector3> corners,float limitRange, out float totalDistance)
    {
        //経路を計算
        CalcCorners(ref corners,ref destPos);
        //制限距離に応じて再計算
        return ReCalcCornersFromRange(ref corners,ref destPos,transform.position,limitRange,out totalDistance);
    }

    //経路計算
    bool CalcCorners(ref List<Vector3> corners,ref Vector3 destPos)
    {
        NavMeshPath path = new NavMeshPath();
        bool canArrive=NavMesh.CalculatePath(transform.position,destPos,NavMesh.AllAreas,path);
        path.GetCornersNonAlloc(path.corners);
        corners=path.corners.ToList();
        return canArrive;
    }
    /// <summary>
    /// 経路の距離を計算
    /// </summary>
    /// <returns></returns>
    public float CalcPathDistance()
    {
        float totalDistance = 0f;
        Vector3 prevPos=new();
        bool isFirst=true;
        foreach(var corner in navAgent.path.corners)
        {
            if(isFirst)
            {
                prevPos=corner;
                isFirst=false;
                continue;
            }
            totalDistance+=(corner-prevPos).magnitude;
            prevPos=corner;
        }
    //    totalDistance+=(transform.position-prevPos).magnitude;
        return totalDistance;
    }

    /// <summary>
    /// 目的地が制限距離内にあるか
    /// </summary>
    /// <param name="destPos">目的地</param>
    /// <param name="limitRange">制限距離</param>
    /// <returns>結果</returns>
    public bool CanMoveInRange(Vector3 destPos,float limitRange)
    {
        List<Vector3> calcCorners=new();
        CalcCorners(ref calcCorners,ref destPos);
        if (calcCorners.Count <= 0){return false;}

        Vector3 prevPos=transform.position;
        float distance=0f;

        
        bool isFirst = true;
        foreach(Vector3 pos in calcCorners)
        {
            if (isFirst)
            {
                prevPos = pos;
                isFirst = false;
                continue;
            }
            distance+=(pos-prevPos).magnitude;
            prevPos=pos;
        }
        
        return distance <= limitRange;
    }

    /// <summary>
    /// 既存の経路から制限距離を考慮したものに再計算する
    /// </summary>
    /// <param name="corners">NavMeshPathで算出された経由地</param>
    /// <param name="resultPos">移動できる限界座標</param>
    /// <param name="nowPos">現在のエージェントの座標</param>
    /// <param name="limitRange">移動可能距離</param>
    /// <param name="totalDistance">算出された移動距離</param>
    public static bool ReCalcCornersFromRange(ref List<Vector3> corners,ref Vector3 resultPos,Vector3 nowPos,float limitRange,out float totalDistance)
    {
        bool isArrivable = true;

        if (corners.Count==0)
        {
            totalDistance = (resultPos - nowPos).magnitude;


            if (totalDistance!=0f && totalDistance > limitRange)
            {
                //目的地までの移動限界座標の算出
                float resultDist = limitRange - totalDistance;
                Vector3 moveDir = (resultPos - nowPos).normalized;
                resultPos = nowPos + (moveDir * resultDist);
                totalDistance = limitRange;
                isArrivable = false;
            }

            return isArrivable; 
        }
  
        

        Vector3[] copy=new Vector3 [corners.Count];
        corners.CopyTo(copy);
        
        //移動順に変換
        copy.Reverse();
        
        corners.Clear();
        Vector3 prevPos=new();
        bool isFirst = true;

        totalDistance = 0f;

        foreach (Vector3 pos in copy)
        {
            if(isFirst)
            {
                prevPos = pos;
                isFirst = false;
                corners.Add(pos);
                continue;
            }

            float distance=(pos-prevPos).magnitude;
            if((totalDistance+distance)>=limitRange)
            {
                //目的地までの移動限界座標の算出
                float resultDist=limitRange-totalDistance;
                Vector3 moveDir=(pos-prevPos).normalized;
                resultPos=prevPos+(moveDir*resultDist);
                totalDistance = limitRange;
                isArrivable = false;
                break;
            }
            totalDistance+=distance;
            corners.Add(pos);
            prevPos=pos;
        }
        corners.Reverse();

        return isArrivable;
    }  


    public void DrawGuizmosCalcCorners(Vector3 destPos,float limitRange,Color color)
    {
        //制限距離に応じて経路を計算
        List<Vector3> calcCorners=new();
        CalcCornersFromRange(ref destPos,ref calcCorners,limitRange,out float totalDistance);
        //結果を描画
        Gizmos.color = color;
        Gizmos.DrawWireSphere(destPos, 0.2f);
        Vector3 prevPos = destPos;
        
        foreach(var pos in calcCorners)
        {
            Gizmos.DrawLine(prevPos, pos);
            prevPos = pos;
        }

    }

    private void OnDrawGizmos()
    {
        if (navAgent == null) { return; }
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(_navAgent.nextPosition, 0.0f);
        Vector3 prevPos = new();
        int i = 0;
        foreach(Vector3 pos in navAgent.path.corners)
        {
            Gizmos.DrawWireSphere(pos, 0.2f);
            if(i==0)
            {
                prevPos = pos;
            }
            else
            {
                Gizmos.DrawLine(prevPos, pos);
                prevPos = pos;
            }
            ++i;
        }
    }
}
