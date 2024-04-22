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


    [SerializeField] UpdateTypes updateType;
    StateBase nowState = null;
    public void ChangeState(StateBase state)
    {
        nowState?.OnExit();
        nowState = state;
        nowState?.OnEnter();
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

        virtual public void OnCollisionEnter(Collision collision){} 
        virtual public void OnCollisionStay(Collision collision){} 
        virtual public void OnCollisionExit(Collision collision){} 

        virtual public void OnTriggerEnter(Collider other){} 
        virtual public void OnTriggerStay(Collider other){} 
        virtual public void OnTriggerExit(Collider other){} 
    }

    public void DoUpdate()
    {
        nowState?.OnUpdate();
    }
    public void DoFixedUpdate()
    {
        nowState?.OnFixedUpdate();
    }

    public void DoOnCollisionEnter(Collision collision)
    {
        nowState?.OnCollisionEnter(collision);
    }
    
    public void DoOnCollisionStay(Collision collision)
    {
        nowState?.OnCollisionStay(collision);
    }
    
    public void DoOnCollisionExit(Collision collision)
    {
        nowState?.OnCollisionExit(collision);
    }
    
    public void DoOnTriggerEnter(Collider other)
    {
        nowState?.OnTriggerEnter(other);
    }
    
    public void DoOnTriggerStay(Collider other)
    {
        nowState?.OnTriggerStay(other);
    }
    
    public void DoOnTriggerExit(Collider other)
    {
        nowState?.OnTriggerExit(other);
    }


    void Update()
    {
        if (updateType == UpdateTypes.Auto)
        {
            nowState?.OnUpdate();
        }
    }
    void FixedUpdate()
    {
        if (updateType == UpdateTypes.Auto) 
        {
            nowState?.OnFixedUpdate();
        }
        
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if(updateType == UpdateTypes.Auto)
        {
            nowState?.OnCollisionEnter(collision);
        }
    }
    
    void OnCollisionStay(Collision collision)
    {
        if(updateType == UpdateTypes.Auto)
        {
            nowState?.OnCollisionStay(collision);
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
        if(updateType == UpdateTypes.Auto)
        {
            nowState?.OnCollisionExit(collision);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if(updateType == UpdateTypes.Auto)
        {
            nowState?.OnTriggerEnter(other);
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if(updateType == UpdateTypes.Auto)
        {
            nowState?.OnTriggerStay(other);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if(updateType == UpdateTypes.Auto)
        {
            nowState?.OnTriggerExit(other);
        }
    }

}
