using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CharacterBrain : MonoBehaviour
{
    [SerializeField] GameObject aiInput;
    IInputProvider inputProvider;
    Rigidbody rigidbody;
    
    [SerializeField] Animator animator;
    GenericStateMachine stateMachine;
    Vector3 moveValue;
    Vector3 velocity;
    Vector3 rootMotionDaltaPosition=Vector3.zero;
    public void AddRootMotionDelta(ref Vector3 v)
    {
        rootMotionDaltaPosition += v;
    }
    // Start is called before the first frame update
    void Awake()
    {
        TryGetComponent(out stateMachine);
        TryGetComponent(out rigidbody);
    }

    void Start()
    {
        inputProvider=aiInput.GetComponent<IInputProvider>();

        this.UpdateAsObservable()
        .Where(_=>enabled)
        .Subscribe(_=>
        {
            stateMachine.DoUpdate();
            rigidbody.velocity=moveValue+velocity;
            rigidbody.velocity+=rootMotionDaltaPosition;
            rootMotionDaltaPosition=Vector3.zero;


            rigidbody.velocity=new Vector3(rigidbody.velocity.x,0f,rigidbody.velocity.z);
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

            //��
            if (vMove.magnitude > 0.2f)
            {
                Brain.animator.SetFloat("MoveSpeed", vMove.magnitude);
            }
            else
            {
                Brain.animator.SetFloat("MoveSpeed", 0.0f);
            }
        }
    }
    [System.Serializable]
    public class ASWalk:ASBase
    {
        public override void OnUpdate()
        {
            Vector3 vMove = Brain.inputProvider.MoveVector;
            //��
            if (vMove.magnitude<=0.2f)
            {
                Brain.animator.SetFloat("MoveSpeed", vMove.magnitude);
            }
            Brain.moveValue = vMove*2.0f;
            
            Quaternion qRota = Quaternion.RotateTowards(
                Brain.transform.rotation,      
                Quaternion.LookRotation(vMove),
                600*Time.deltaTime      
                );
            Brain.transform.rotation=qRota;
        }
    }

}
