using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorEvents : MonoBehaviour
{
    [SerializeField] DictionaryEx<string, Transform> tarnsformDic;
    CharacterBrain Brain { get; set; }
    Animator _animator;
    private void Awake()
    {
        Brain=GetComponentInParent<CharacterBrain>();
        _animator=GetComponent<Animator>();
    }
    //RootMotionを取得
    void OnAnimatorMove()
    {
        var deltaPos = _animator.deltaPosition;
        Brain.AddRootMotionDelta(ref deltaPos);
    }

    public class CharaEventBase : AnimatorEvents.EventNodeBase
    {
        CharacterAnimatorEvents _owner;
        public CharacterAnimatorEvents GetOwner(Animator animator)
        {
            if (_owner == null) _owner = animator.GetComponent<CharacterAnimatorEvents>();
            return _owner;
        }
    }


        [System.Serializable]
    public class CharaEvent_Tset : CharaEventBase
    {
        [SerializeField] string _text;
        [SerializeField] int _value;

        public override void OnEvent(Animator animator)
        {
            Debug.LogWarning(_text);
        }
        public override void OnExit(Animator animator)
        {

        }

    }

        [System.Serializable]
    public class CharaEvent_OutputPrefab : CharaEventBase
    {
        [SerializeField] GameObject _prefab;
        [SerializeField] string _parentName;
        public override void OnEvent(Animator animator)
        {
            if(_prefab ==null)
            {
                return;
            }

            var owner=GetOwner(animator);

            Transform parent = owner.Brain.transform;
            if(string.IsNullOrEmpty(_parentName)==false)
            {
                owner.tarnsformDic.TryGetValue(_parentName,out parent);
            }

            var newObj=
            Instantiate(
                _prefab,
                parent: owner.Brain.transform
                );
            var mainObj=newObj.GetComponent<MainObjectData>();

            if(mainObj!=null)
            {
                mainObj.OwnerObject = owner.Brain.MainObjectData;
            }

        }
        public override void OnExit(Animator animator)
        {

        }

    }
    [Serializable]
    public class CharacterEvent_Attack : CharaEventBase
    {
        public override void OnEvent(Animator animator)
        {
            var owner = GetOwner(animator);

        }
    }

}
