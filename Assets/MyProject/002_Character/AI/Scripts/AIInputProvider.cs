using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UniRx;
using System;
using Utility.UnityEngineEx;
public class AIInputProvider : MonoBehaviour, IInputProvider
{
    public enum StateType
    {
        Wait,
        Move,
        MeleeAttack,
        Dead,
        Damage,
    }
    CharacterBrain ownerBrain;

    StateType stateType=StateType.Wait;

    Vector3 moveVec;
    public Vector3 MoveVector => moveVec;
    [SerializeField] AIEyeSight meleeSight;
    [SerializeField] AIPathFinding pathFinding;
    Animator animator;

    MainObjectData _targetChara;

    public ReactiveProperty<CharacterBrain> Target=new(null);

    public bool IsAttack{get;set;}=false;

    public bool IsDead{get;set;}=false;
    
    public bool IsMove{get=>moveVec.magnitude>0f;}
    public bool IsDamage{get;set;}=false;

    public bool CanControl=>(stateType==StateType.Wait);
    public float AgentHeight => pathFinding.transform.position.y;

    float movePathDistance=0f;
    float movedPathDistance=0f;
    public float MovedPathDistance=>movedPathDistance;
    public float RemainingPathDistance=> movePathDistance-movedPathDistance;

    bool IsArriveThisTurn=>RemainingPathDistance<=ownerBrain.Param.ActionRange;
    
    public void SetDestination(Vector3 pos)
    {
        if (ownerBrain.Param.ActionRange <= 0f ||
           GameManager.Instance.TurnManager.IsActionCharacter(ownerBrain) == false)
        {
            Target.Value = null;
            
            return;
        }


        List<Vector3> corners=new();
        bool isArrivable=pathFinding.CalcRouteFromRange(ref pos,ref corners, ownerBrain.Param.ActionRange,out movePathDistance);
        movedPathDistance = 0f;
        pathFinding.SetDestination(pos);
        //このターンで到達しない場合はターゲットから外す
        if(isArrivable==false && Target.Value!=null)
        {
            Target.Value=null;
        }


        ownerBrain.AddActiveControl();

    }



    bool CheckAttack()
    { 
        if(ownerBrain.Param.ActionPoint<=0){return false;}

        //AIEyeSightを使ったターゲット搜索
        if(Target.Value!=null)
        {
            foreach(var objectData in meleeSight.Founds)
            {
                var character=objectData.GetComponent<CharacterBrain>();
                if(character==null){continue;}
                if(Target.Value==character)
                {
                    return true;
                }
            }
        }
        return false;
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
            if(ownerBrain.Param.ActionPoint<=0 ||
               GameManager.Instance.TurnManager.IsActionCharacter(ownerBrain)==false)
            {
                newTarget=null;
                return;
            }
            SetDestination(newTarget.transform.position);
        });

        ownerBrain=GetComponentInParent<CharacterBrain>();

        pathFinding.Speed = ownerBrain.MoveSpeed;
    }


    public void ChangeState(StateType type)
    {
        animator.SetInteger("StateType",(int)type);
        stateType=type;
    }
    
    public void StartTurn()
    {
        
    }
    
    public void EndTurn()
    {
        Target.Value=null;
        pathFinding.Stop();
    }
    
    public bool CalcRouteFromRange(ref Vector3 destPos,ref List<Vector3> corners,out float totalDistance)=>pathFinding.CalcRouteFromRange(ref destPos,ref corners,ownerBrain.Param.ActionRange,out totalDistance);
    public void DrawGuizmosCalcCorners(Vector3 destPos,float limitRange,Color color)=>pathFinding.DrawGuizmosCalcCorners(destPos,limitRange,color);


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
        public override void OnEnter()
        {
            base.OnEnter();

            
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            AIInputProvider.moveVec=Vector3.zero;
            if(AIInputProvider.pathFinding.IsAlived==false)
            {
                AIInputProvider.ChangeState(StateType.Move);
                return;
            }

            if(AIInputProvider.Target.Value!=null && AIInputProvider.CheckAttack())
            {
                AIInputProvider.ChangeState(StateType.Wait);
                return;
            }

            if(AIInputProvider.IsDamage)
            {
                AIInputProvider.ChangeState(StateType.Damage);
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
                AIInputProvider.ownerBrain.EndActiveControl();
                AIInputProvider.moveVec=Vector3.zero;
                return;
            }

            //残り移動可能距離計算
            float moveDistanceNow=
                (AIInputProvider.RemainingPathDistance) -AIInputProvider.pathFinding.CalcPathDistance();
            AIInputProvider.movedPathDistance += moveDistanceNow;

            AIInputProvider.ownerBrain.Param.ActionRange-= moveDistanceNow;

            base.OnUpdate();

            var dir = AIInputProvider.pathFinding.DesiredVelecity;
            dir.y = 0f;
            if (dir.sqrMagnitude >= 1.0f) dir.Normalize();

            AIInputProvider.moveVec = dir;

            //攻撃可能であれば自動で行う
            if(AIInputProvider.CheckAttack())
            {
                AIInputProvider.IsAttack=true;
                AIInputProvider.ChangeState(StateType.MeleeAttack);
                 return;
            }
        }
    }

    [System.Serializable]
    public class AISMeleeAttack : AISBase
    {
        public override void OnEnter()
        {
            base.OnEnter();

            AIInputProvider.pathFinding.Stop();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            if(AIInputProvider.IsAttack==false)
            {
                AIInputProvider.ChangeState(StateType.Wait);
                AIInputProvider.ownerBrain.EndActiveControl();
                return;
            }
        }
    }

    [Serializable]
    public class AISDead : AISBase
    {
        
    }

    [Serializable]
    public class AISDamage : AISBase
    {
        public override void OnUpdate()
        {
            base.OnUpdate();
            if(AIInputProvider.IsDamage==false)
            {
                AIInputProvider.ChangeState(StateType.Wait);
                return;
            }
        }
    }
}
