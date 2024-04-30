using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using System.IO;
using System.Reflection;
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
    public void CalcCornersFromRange(ref Vector3 destPos,ref List<Vector3> corners,ref float limitRange)
    {
        //経路を計算
        CalcCorners(ref corners,ref destPos);
        //制限距離に応じて再計算
        ReCalcCornersFromRange(ref corners,ref destPos,transform.position,ref limitRange);
    }

    //経路計算
    bool CalcCorners(ref List<Vector3> corners,ref Vector3 destPos)
    {
        NavMeshPath path = new NavMeshPath();
        bool canArrive=NavMesh.CalculatePath(transform.position,destPos,NavMesh.AllAreas,path);
        corners=path.corners.ToList();
        if(canArrive==false)
        {
            if(corners.Count<=0)
            {
                destPos=transform.position;
            }
            else
            {
                destPos=corners.First();
            }
            
        }
        return canArrive;
    }
    /// <summary>
    /// 経路の距離を計算
    /// </summary>
    /// <returns></returns>
    public float CalcPathDistance()
    {
        float totalDistance = 0f;
        Vector3 prevPos=navAgent.destination;
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
        Vector3 prevPos=transform.position;
        float distance=0f;

        if(calcCorners.Count<=0)
        {
            distance=(destPos-prevPos).magnitude;

        }

        foreach(Vector3 pos in calcCorners)
        {
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
    public static void ReCalcCornersFromRange(ref List<Vector3> corners,ref Vector3 resultPos,Vector3 nowPos,ref float limitRange)
    {
        float totalDistance=0f;
        //経路がない場合は制限距離を計算して終了
        if(corners.Count == 0)
        {
            totalDistance=(resultPos-nowPos).magnitude;
            if(totalDistance <= 0)
            {
                return;
            }

            if(totalDistance > limitRange)
            {
                Vector3 moveDir=(resultPos-nowPos).normalized;
                resultPos=nowPos+(moveDir*limitRange);
            }
            limitRange-=totalDistance;
            return;
        }

        Vector3[] copy=new Vector3 [corners.Count];
        corners.CopyTo(copy);
        
        //移動順に変換
        copy.Reverse();
        
        corners.Clear();
        Vector3 prevPos=nowPos;


        foreach(Vector3 pos in copy)
        {
            float distance=(pos-prevPos).magnitude;
            if((totalDistance+distance)>=limitRange)
            {
                //目的地までの移動限界座標の算出
                float resultDist=limitRange-totalDistance;
                Vector3 moveDir=(pos-prevPos).normalized;
                resultPos=prevPos+(moveDir*resultDist);
                totalDistance+=resultDist;

                break;
            }
            totalDistance+=distance;
            corners.Add(pos);
            prevPos=pos;
        }
        corners.Reverse();
        limitRange-=totalDistance;
    }  


    public void DrawGuizmosCalcCorners(Vector3 destPos,float limitRange)
    {
        //制限距離に応じて経路を計算
        List<Vector3> calcCorners=new();
        CalcCornersFromRange(ref destPos,ref calcCorners,ref limitRange);
        //結果を描画
        Gizmos.color = Color.blue;
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
