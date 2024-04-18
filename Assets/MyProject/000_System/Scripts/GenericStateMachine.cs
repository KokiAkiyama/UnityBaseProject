using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class GenericStateMachine : MonoBehaviour
{
    public enum UpdateTypes
    {
        [InspectorName("自動Update")]Auto,
        [InspectorName("手動Update")]Manual
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
    //ステートマシン
    //==============================
    public class StateBase
    {
        public GenericStateMachine StateMachine { get; private set; }

        virtual public void Initialize(GenericStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        virtual public void OnEnter() { }
        virtual public void OnExit() { }
        virtual public void OnUpdate() { }
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
