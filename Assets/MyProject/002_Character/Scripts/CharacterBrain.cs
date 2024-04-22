using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Damage;
using System.Data.Common;

public class CharacterBrain : MonoBehaviour
{
    public enum StateType
    {
        Idle,
        Walk,
        MeleeAttack,
    }

    public struct AttackParam
    {
        DamageType type;
    }

    [SerializeField] GameObject aiInput;
    IInputProvider inputProvider;
    public AIInputProvider AIInputProvider { get; private set; }
    [SerializeField] Animator animator;
    GenericStateMachine stateMachine;
    Vector3 moveValue;
    Vector3 rootMotionDaltaPosition = Vector3.zero;

    [SerializeField] float moveSpeed = 2.0f;
    [SerializeField] float rotSpeed = 600.0f;

    const float stopMagnitude = 0.2f;

    [SerializeField] MainObjectData mainObjectData;
    public MainObjectData MainObjectData => mainObjectData;

    [SerializeField] CharacterIDs ID;

    [SerializeField] CharacterData param;

    CharacterBrain target;
    public CharacterBrain Target
    {
        get{return target;}
        set{target=value;}
    }

    DamageParam attackParam=new();

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
        //データベースからパラメータを取得
        GameManager.Instance.CharacterController.Copy(ref param,ID);


        inputProvider = aiInput.GetComponent<IInputProvider>();
        AIInputProvider = inputProvider as AIInputProvider;
        this.UpdateAsObservable()
        .Where(_ => enabled)
        .Subscribe(_ =>
        {
            stateMachine.DoUpdate();
            transform.position += moveValue * Time.deltaTime;
            transform.position += rootMotionDaltaPosition * Time.deltaTime;
            rootMotionDaltaPosition = Vector3.zero;


            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

            moveValue = Vector3.zero;
        }
        );
        this.FixedUpdateAsObservable()
        .Where(_ => enabled)
        .Subscribe(_ =>
        {
            stateMachine.DoFixedUpdate();
        }
        );

    }

    void OnTriggerEnter(Collider other)
    {
        stateMachine.DoOnTriggerEnter(other);
    }


    public void Attack()
    {
        if(target==null){return;}
        
        DamageParam damageParam=new();
        damageParam.DamageValue = param.strength;
        damageParam.damageType = attackParam.damageType;

        target.GetDamage(damageParam);
    }



    public void GetDamage(DamageParam damageParam)
    {
        param.HP=Mathf.Max(param.HP-(damageParam.DamageValue),0);
        //死亡
        if(param.HP<=0)
        {


        }
    }

    public class ASBase : GenericStateMachine.StateBase
    {
        public CharacterBrain Owner { get; private set; }

        public override void Initialize(GenericStateMachine stateMachine)
        {
            Owner = stateMachine.GetComponent<CharacterBrain>();
        }
    }

    [System.Serializable]
    public class ASIdle : ASBase
    {
        public override void OnUpdate()
        {
            Vector3 vMove = Owner.inputProvider.MoveVector;

            //歩き
            if (vMove.magnitude > stopMagnitude)
            {
                Owner.animator.SetInteger("StateType", (int)StateType.Walk);
            }
        }
    }
    [System.Serializable]
    public class ASWalk : ASBase
    {
        public override void OnUpdate()
        {
            Vector3 vMove = Owner.inputProvider.MoveVector;
            //停止
            if (vMove.magnitude <= stopMagnitude)
            {
                Owner.animator.SetInteger("StateType", (int)StateType.Idle);
                return;
            }
            Owner.moveValue = vMove * Owner.moveSpeed;

            Quaternion qRota = Quaternion.RotateTowards(
                Owner.transform.rotation,
                Quaternion.LookRotation(vMove),
                Owner.rotSpeed * Time.deltaTime
                );
            Owner.transform.rotation = qRota;
        }

        public override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            if(Owner.target==null){return;}

            var brain=other.GetComponent<CharacterBrain>();
            if(brain==Owner.Target)
            {
                Owner.animator.SetInteger("StateType", (int)StateType.MeleeAttack);
                return;
            }
            
        }
    }
    [System.Serializable]
    public class ASMeleeAttack : ASBase
    {
        
    }
}
