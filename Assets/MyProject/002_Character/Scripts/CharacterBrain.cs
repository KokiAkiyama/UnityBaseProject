using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Damage;

public class CharacterBrain : MonoBehaviour
{
    public enum StateType
    {
        Idle,
        Walk,
        MeleeAttack,
        Dead
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

    public bool IsTurnEnd=false;

    [SerializeField] CharacterIDs ID;
    
    [SerializeField]CharacterData turnMaxParam;

    [SerializeField] CharacterData param;
    public CharacterData Param{get=>param;set=>param=value;}
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
        GameManager.Instance.CharacterManager.Copy(ref param,ID);
        param.Copy(ref turnMaxParam);

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


            transform.position = new Vector3(transform.position.x, inputProvider.AgentHeight, transform.position.z);

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
        //キャラクターマネージャーに登録
        GameManager.Instance.CharacterManager.AddCharacter(this);
    }

    void OnTriggerEnter(Collider other)
    {
        stateMachine.DoOnTriggerEnter(other);
    }

    public void ChangeState(StateType type)
    {
        animator.SetInteger("StateType",(int)type);
    }

    public void StartTurn()
    {
        turnMaxParam.Copy(ref param);
    }

    public void Attack()
    {
        if(AIInputProvider.Target.Value==null){return;}
        
        DamageParam damageParam=new();
        damageParam.DamageValue = param.strength;
        damageParam.damageType = attackParam.damageType;

        AIInputProvider.Target.Value.GetDamage(damageParam);
    }



    public void GetDamage(DamageParam damageParam)
    {
        //耐久力補正値を考慮したダメージ計算
        param.HP=Mathf.Max(param.HP-(damageParam.DamageValue-param.CorrectionValue_Constitution),0);
        //死亡
        if(param.HP<=0)
        {
            
            ChangeState(StateType.Dead);

        }
    }

    public void EndAttack()
    {
        AIInputProvider.IsAttack=false;
        AIInputProvider.Target.Value=null;
        ChangeState(StateType.Idle);
    }

    void Rotate()
    {
        var vMove=inputProvider.MoveVector;
        
        Quaternion qRota = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(vMove),
                rotSpeed * Time.deltaTime
                );
            transform.rotation = qRota;
    }

    public void DrawGizmosCalceCorners(Vector3 destPos)=>AIInputProvider.DrawGuizmosCalcCorners(destPos,param.ActionRange);


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
                Owner.ChangeState(StateType.Walk);
            }
        }
    }
    [System.Serializable]
    public class ASWalk : ASBase
    {
        public override void OnUpdate()
        {
            Vector3 vMove = Owner.inputProvider.MoveVector;

            if(Owner.AIInputProvider.IsAttack)
            {
                Owner.ChangeState(StateType.MeleeAttack);
                return;
                
            }



            //停止
            if (vMove.magnitude <= stopMagnitude)
            {
                Owner.ChangeState(StateType.Idle);
                return;
            }
            Owner.moveValue = vMove * Owner.moveSpeed;

            Owner.Rotate();
        }

        // public override void OnTriggerEnter(Collider other)
        // {
        //     base.OnTriggerEnter(other);

        //     if(Owner.AIInputProvider.Target.Value==null){return;}

        //     var brain=other.GetComponent<CharacterBrain>();
        //     if(brain==Owner.AIInputProvider.Target.Value)
        //     {
        //         Owner.ChangeState(StateType.MeleeAttack);
        //         return;
        //     }
            
        // }
    }
    [System.Serializable]
    public class ASMeleeAttack : ASBase
    {
    }

    [System.Serializable]
    public class ASDead : ASBase
    {
        public override void OnEnter()
        {
            base.OnEnter();

            Owner.AIInputProvider.IsDead=true;
        }

    }
    
}
