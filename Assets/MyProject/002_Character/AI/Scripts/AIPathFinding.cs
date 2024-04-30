using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
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
    /// 既存の経路から制限距離を考慮したものに再計算する
    /// </summary>
    /// <param name="corners">NavMeshPathで算出された経由地</param>
    /// <param name="nowPos">現在のエージェントの座標</param>
    /// <param name="resultPos">移動できる限界座標</param>
    /// <param name="limitRange">移動可能距離</param>
    /// <returns>再計算された経由地</returns>
    public static List<Vector3> CalcCornersFromRange(Vector3[] corners,Vector3 nowPos,ref Vector3 resultPos,float limitRange)
    {
        List<Vector3> result=new();
        Vector3 prevPos=nowPos;
        float range=0f;
        corners.Reverse();
        foreach(Vector3 pos in corners)
        {
            float distance=(pos-prevPos).magnitude;
            if((range+distance)>=limitRange)
            {
                //目的地までの移動限界座標の算出
                float resultDist=limitRange-range;
                Vector3 moveDir=(pos-prevPos).normalized;
                resultPos=prevPos+(moveDir*resultDist);
                break;
            }
            range+=distance;
            result.Add(pos);
            prevPos=pos;
        }
        result.Reverse();
        return result;
    }  


    public void DrawGuizmosCalcCorners(Vector3 destPos,float limitRange)
    {
        //経路を計算
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position,destPos,NavMesh.AllAreas,path);
        List<Vector3> calcCorners=new();
        path.GetCornersNonAlloc(calcCorners.ToArray());
        //制限距離に応じて再計算
        calcCorners=CalcCornersFromRange(path.corners,transform.position,ref destPos,limitRange);
        //結果を描画
        Vector3 prevPos = new();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(destPos, 0.2f);
        int i = 0;
        foreach(var pos in calcCorners)
        {
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
