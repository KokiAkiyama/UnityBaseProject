using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class GenericStateMachine : MonoBehaviour
{
    public enum UpdateTypes
    {
        [InspectorName("自動実行")]Auto,   //Update系自動で実行
        [InspectorName("手動実行")]Manual  //Update系手動で実行
    }
    [SerializeField] UpdateTypes _updateType;
    StateBase _nowState = null;
    public void ChangeState(StateBase state)
    {
        _nowState?.OnExit();
        _nowState = state;
        _nowState?.OnEnter();
    }
    public Animator _animator { get; private set; }
    private void Awake()
    {
        _animator=GetComponent<Animator>();
    }
    //==============================
    //アクション用のステートクラス
    //==============================
    public class StateBase
    {
        public GenericStateMachine StateMachine { get; private set; }

        virtual public void Initialize(GenericStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        //ステート開始時
        virtual public void OnEnter() { }
        //ステート終了時
        virtual public void OnExit() { }
        //毎フレーム
        virtual public void OnUpdate() { }
        //一定間隔
        virtual public void OnFixedUpdate() { }
    }

    public void DoUpdate()
    {
        _nowState?.OnUpdate();
    }
    public void DoFixedUpdate()
    {
        _nowState?.OnFixedUpdate();
    }

    void Update()
    {
        if (_updateType == UpdateTypes.Auto)
        {
            _nowState?.OnUpdate();
        }
    }
    void FixedUpdate()
    {
        if (_updateType == UpdateTypes.Auto) 
        {
            _nowState?.OnFixedUpdate();
        }
        
    }
}
