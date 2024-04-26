using System.Collections;
using System.Collections.Generic;
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
    /// <param name="corners"></param>
    /// <param name="nowPos"></param>
    /// <param name="limitRange"></param>
    /// <returns></returns>
    public List<Vector3> CalcCornersFromRange(Vector3[] corners,Vector3 nowPos,float limitRange)
    {
        List<Vector3> result=new();
        Vector3 prevPos=nowPos;
        float range=0f;
        foreach(Vector3 pos in corners)
        {
            range+=(pos-prevPos).magnitude;
            if(range>=limitRange)
            {
                break;
            }
            result.Add(pos);
        }

        return result;
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
