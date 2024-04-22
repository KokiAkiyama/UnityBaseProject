using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UniRx;
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

    public ReactiveProperty<CharacterBrain> Target=new(null);

    bool isAttack=false;

    public bool IsAttack
    {
        get
        {
            return isAttack;
        }
        set
        {
            isAttack=value;
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


    void Start()
    {
        Target.
        Where(newTarget=>newTarget!=null)
        .Subscribe(newTarget=>
        {
            SetDestination(newTarget.transform.position);
        });
    }


    public void ChangeState(StateType type)
    {
        animator.SetInteger("StateType",(int)type);
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
            AIInputProvider.moveVec=Vector3.zero;
            if(AIInputProvider.pathFinding.IsAlived==false)
            {
                AIInputProvider.ChangeState(StateType.Move);
                return;
            }
        }
    }

    [System.Serializable]
    public class AISMove : AISBase
    { 
        public override void OnUpdate()
        {
            if(AIInputProvider.pathFinding.IsAlived && AIInputProvider.Target.Value==null)
            {
                AIInputProvider.ChangeState(StateType.Wait);
                AIInputProvider.moveVec=Vector3.zero;
                return;
            }
            base.OnUpdate();

            var dir = AIInputProvider.pathFinding.DesiredVelecity;
            dir.y = 0f;
            if (dir.sqrMagnitude >= 1.0f) dir.Normalize();

            AIInputProvider.moveVec = dir;

        
            if(AIInputProvider.Target.Value!=null)
            {
                foreach(var objectData in AIInputProvider.eyeSight.Founds)
                {
                    var character=objectData.GetComponent<CharacterBrain>();
                    if(character==null){continue;}

                    if(AIInputProvider.Target.Value==character)
                    {
                        AIInputProvider.IsAttack=true;
                        return;
                    }

                }
            }
        }
    }

    [System.Serializable]
    public class AISMeleeAttack : AISBase
    {
        public override void OnUpdate()
        {
            if(AIInputProvider.isAttack==false)
            {
                AIInputProvider.ChangeState(StateType.Wait);
                return;
            }
        }
    }

    
}
