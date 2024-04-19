using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CharacterBrain : MonoBehaviour
{
    public enum StateType
    {
        Idle,
        Walk
    }

    [SerializeField] GameObject aiInput;
    IInputProvider inputProvider;
    public AIInputProvider AIInputProvider{get;private set;}
    [SerializeField] Animator animator;
    GenericStateMachine stateMachine;
    Vector3 moveValue;
    Vector3 rootMotionDaltaPosition=Vector3.zero;

    [SerializeField] float moveSpeed=2.0f;
    [SerializeField] float rotSpeed=600.0f;

    const float stopMagnitude=0.2f;

    MainObjectData mainObjectData;
    public MainObjectData MainObjectData=>mainObjectData;

    [SerializeField]CharacterIDs ID;

    public void AddRootMotionDelta(ref Vector3 v)
    {
        rootMotionDaltaPosition += v;
    }
    // Start is called before the first frame update
    void Awake()
    {
        TryGetComponent(out stateMachine);
    }

    void Start()
    {
        inputProvider=aiInput.GetComponent<IInputProvider>();
        AIInputProvider=inputProvider as AIInputProvider;
        this.UpdateAsObservable()
        .Where(_=>enabled)
        .Subscribe(_=>
        {
            stateMachine.DoUpdate();
            transform.position+=moveValue*Time.deltaTime;
            transform.position+=rootMotionDaltaPosition*Time.deltaTime;
            rootMotionDaltaPosition=Vector3.zero;


            transform.position=new Vector3(transform.position.x,0f,transform.position.z);

            moveValue=Vector3.zero;
        }
        );
        this.FixedUpdateAsObservable()
        .Where(_=>enabled)
        .Subscribe(_=>
        {
            stateMachine.DoFixedUpdate();
        }
        );
        
    }
    public class ASBase : GenericStateMachine.StateBase
    {
        public CharacterBrain Brain { get; private set;}

        public override void Initialize(GenericStateMachine stateMachine)
        {
            Brain = stateMachine.GetComponent<CharacterBrain>();
        }
    }

    [System.Serializable]
    public class ASIdle:ASBase
    {
        public override void OnUpdate()
        {
            Vector3 vMove = Brain.inputProvider.MoveVector;

            //歩き
            if (vMove.magnitude > stopMagnitude)
            {
                Brain.animator.SetInteger("StateType", (int)StateType.Walk);
            }
        }
    }
    [System.Serializable]
    public class ASWalk:ASBase
    {
        public override void OnUpdate()
        {
            Vector3 vMove = Brain.inputProvider.MoveVector;
            //停止
            if (vMove.magnitude<=stopMagnitude)
            {
                Brain.animator.SetInteger("StateType", (int)StateType.Idle);
                return;
            }
            Brain.moveValue = vMove*Brain.moveSpeed;
            
            Quaternion qRota = Quaternion.RotateTowards(
                Brain.transform.rotation,      
                Quaternion.LookRotation(vMove),
                Brain.rotSpeed*Time.deltaTime      
                );
            Brain.transform.rotation=qRota;
        }
    }

}
