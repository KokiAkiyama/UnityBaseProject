using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
namespace Damage
{
    public class DamegeApplicant : MonoBehaviour
    {
        [Header("�q�b�g�������̃G�t�F�N�g")]
        [Tooltip("�q�b�g�������ɁA�G�t�F�N�g���������܂�(*�L�ցM)")]
        [SerializeField] GameObject _assetOnHitEffect;
        MainObjectData _mainObject;
        HitList _hitList=new();
        [SerializeField] float _hitInterval=1.0f;
        //�q�b�g�X�g�b�v����
        [SerializeField] float _hitStopDuration = 0.0f;
        public MainObjectData MainObject => this.GetComponentLazy(ref _mainObject);
        // Start is called before the first frame update
        void Start()
        {
            //Collider���ڐG���Ă����
            this.OnCollisionStayAsObservable()
                .Subscribe(collision =>
                {
                    //�_���[�W�ʒm�@�\���������Ă��邩
                    var dmgApp = collision.rigidbody.GetComponent<IDamageApplicable>();
                    if (dmgApp == null) { return; }
                    //�����O���[�v�͖���
                    var targetMainObj = collision.rigidbody.GetComponent<MainObjectData>();
                    if (targetMainObj == null) { return; }
                    if(MainObject.GroupID== targetMainObj.GroupID) { return; }
                    //���Ƀq�b�g�ς݂��ǂ���
                    if(_hitList.Exist(targetMainObj)) { return; }
                    //�o�^
                    _hitList.Register(targetMainObj, _hitInterval);


                    //����Ƀ_���[�W�K�p
                    //DamageParam param = DamageParam.s_pool.Get();
                    //���������[�X
                    using (DamageParam.s_pool.Get(out var param))
                    {


                        using (DamageReply.s_pool.Get(out var rep))
                        {
                            param.DamageValue = 3;
                            param.HitStopDuration = _hitStopDuration;
                            param.HitPosition = collision.contacts[0].point;
                            dmgApp.ApplyDamege(param, rep);

                            //���ʂ����Ƃɏ���
                            if (rep.ResultType.HasFlag(DamageReply.ResultTypes.Hit))
                            {
                                 //�e�ւ̃q�b�g�ʒm
                                if (MainObject.OwnerObject!=null &&
                                   MainObject.OwnerObject.DamageApp.IsUnityNull()==false)
                                {
                                    MainObject.OwnerObject.DamageApp.ApplyHit(param, rep);
                                }

                                //�G�t�F�N�g���o��

                                if (_assetOnHitEffect != null)
                                {

                                    
                                    var effectObj = Instantiate(
                                        _assetOnHitEffect,
                                        position: param.HitPosition,
                                        rotation: Quaternion.identity,
                                        parent:null                                     
                                        );
                                }
                            }

                        }
                        //                    DamageParam.s_pool.Release(param);

                    }
                });
        }

        // Update is called once per frame
        void Update()
        {
            _hitList.AdvanceTime();
        }
    }
}

