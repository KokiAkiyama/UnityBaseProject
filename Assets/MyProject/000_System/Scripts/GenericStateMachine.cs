using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class GenericStateMachine : MonoBehaviour
{
    public enum UpdateTypes
    {
        [InspectorName("�������s")]Auto,   //Update�n�����Ŏ��s
        [InspectorName("�蓮���s")]Manual  //Update�n�蓮�Ŏ��s
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
    //�A�N�V�����p�̃X�e�[�g�N���X
    //==============================
    public class StateBase
    {
        public GenericStateMachine StateMachine { get; private set; }

        virtual public void Initialize(GenericStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        //�X�e�[�g�J�n��
        virtual public void OnEnter() { }
        //�X�e�[�g�I����
        virtual public void OnExit() { }
        //���t���[��
        virtual public void OnUpdate() { }
        //���Ԋu
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
