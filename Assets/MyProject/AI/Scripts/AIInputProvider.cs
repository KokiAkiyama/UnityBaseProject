using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Unity.VisualScripting;
public class AIInputProvider : MonoBehaviour, IInputProvider
{
    public enum StateType
    {
        Wait,
        Move
    }

    Vector3 moveVec;
    public Vector3 MoveVector => moveVec;

    [SerializeField] AIEyeSight eyeSight;
    [SerializeField] AIPathFinding pathFinding;
    Animator animator;

    MainObjectData _targetChara;
    public bool IsAttack
    {
        get
        {
            return false;
        }
    }

    public void SetDestination(Vector3 pos)
    {
        pathFinding.SetDestination(pos);
    }

    void Awake()
    {
       TryGetComponent(out animator);
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

            if(AIInputProvider.pathFinding.IsAlived==false)
            {
                AIInputProvider.animator.SetInteger("StateType" ,(int)StateType.Move);
                return;
            }
        }
    }

    [System.Serializable]
    public class AISMove : AISBase
    { 
        public override void OnUpdate()
        {
            if(AIInputProvider.pathFinding.IsAlived)
            {
                AIInputProvider.animator.SetInteger("StateType" ,(int)StateType.Wait);
                AIInputProvider.moveVec=Vector3.zero;
                return;
            }
            base.OnUpdate();

            var dir = AIInputProvider.pathFinding.DesiredVelecity;
            dir.y = 0f;
            if (dir.sqrMagnitude >= 1.0f) dir.Normalize();

            AIInputProvider.moveVec = dir;


            
        }
    }

    
}
