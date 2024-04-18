using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
public class AIInputProvider : MonoBehaviour, IInputProvider
{
    Vector3 _moveVec;
    public Vector3 MoveVector => _moveVec;

    [SerializeField] AIEyeSight _eyeSight;
    [SerializeField] AIPathFinding _pathFinding;


    MainObjectData _targetChara;
    public bool IsAttack
    {
        get
        {
            return false;
        }
    }

    [System.Serializable]
    public abstract class AISBase:GenericStateMachine.StateBase
    {
        public AIInputProvider AIInputProvider { get; private set; }

        public override void Initialize(GenericStateMachine stateMachine)
        {
            base.Initialize(stateMachine);
            AIInputProvider = stateMachine.GetComponent<AIInputProvider>();

        }
    }

    [System.Serializable]
    public class AISWait : AISBase
    { 
        public override void OnUpdate()
        {
            base.OnUpdate();

            if(AIInputProvider._pathFinding.IsAlived==false)
            {
                
                return;
            }

        }
    }

    [System.Serializable]
    public class AISMove : AISBase
    { 
        public override void OnUpdate()
        {
            base.OnUpdate();


            AIInputProvider._pathFinding.SetDestination(AIInputProvider._targetChara.transform.position);
            
            var dir = AIInputProvider._pathFinding.DesiredVelecity;
            dir.y = 0f;
            if (dir.sqrMagnitude >= 1.0f) dir.Normalize();

            AIInputProvider._moveVec = dir;

        }
    }

    
}
