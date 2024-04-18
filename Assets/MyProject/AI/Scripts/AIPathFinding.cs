using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]

public class AIPathFinding : MonoBehaviour
{
    NavMeshAgent _navAgent;
    //到着しているか
    public bool IsAlived
    {
        get
        {
            //パス計算中
            if (_navAgent.pathPending) return false;
            //止まっている
            if(_navAgent.isStopped) return true;
            
            return _navAgent.remainingDistance <= _navAgent.stoppingDistance;
        }
    }

    public Vector3 DesiredVelecity => _navAgent.desiredVelocity;
    // Start is called before the first frame update
    void Awake()
    {
        TryGetComponent(out _navAgent);
        //移動は手動で行う
        _navAgent.angularSpeed = 0f;
        _navAgent.acceleration = 0f;
        _navAgent.updatePosition = false;
        _navAgent.updateRotation = false;

        _navAgent.isStopped = true;

    }
    public void Stop()
    {
        _navAgent.isStopped = true;
    }
    void Update()
    {
        //パス計算中
        if (_navAgent.pathPending) return;

        //到着している
        if(IsAlived)
        {
            _navAgent.isStopped = true;
        }


        //エージェントの座標更新
        _navAgent.nextPosition = transform.position;   
    }
    //目的地設定
    public void SetDestination(Vector3 position)
    {
        _navAgent.isStopped = false;
        _navAgent.SetDestination(position);
    }

    private void OnDrawGizmos()
    {
        if (_navAgent == null) { return; }
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(_navAgent.nextPosition, 0.0f);
        Vector3 prevPos = new();
        int i = 0;
        foreach(Vector3 pos in _navAgent.path.corners)
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
