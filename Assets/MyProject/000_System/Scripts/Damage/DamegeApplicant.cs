using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
namespace Damage
{
    public class DamegeApplicant : MonoBehaviour
    {
        [Header("ヒットした時のエフェクト")]
        [Tooltip("ヒットした時に、エフェクトが発生します(*´ω｀)")]
        [SerializeField] GameObject _assetOnHitEffect;
        MainObjectData _mainObject;
        HitList _hitList=new();
        [SerializeField] float _hitInterval=1.0f;
        //ヒットストップ時間
        [SerializeField] float _hitStopDuration = 0.0f;
        public MainObjectData MainObject => this.GetComponentLazy(ref _mainObject);
        // Start is called before the first frame update
        void Start()
        {
            //Colliderが接触している間
            this.OnCollisionStayAsObservable()
                .Subscribe(collision =>
                {
                    //ダメージ通知機能を所持しているか
                    var dmgApp = collision.rigidbody.GetComponent<IDamageApplicable>();
                    if (dmgApp == null) { return; }
                    //同じグループは無視
                    var targetMainObj = collision.rigidbody.GetComponent<MainObjectData>();
                    if (targetMainObj == null) { return; }
                    if(MainObject.GroupID== targetMainObj.GroupID) { return; }
                    //既にヒット済みかどうか
                    if(_hitList.Exist(targetMainObj)) { return; }
                    //登録
                    _hitList.Register(targetMainObj, _hitInterval);


                    //相手にダメージ適用
                    //DamageParam param = DamageParam.s_pool.Get();
                    //自動リリース
                    using (DamageParam.s_pool.Get(out var param))
                    {


                        using (DamageReply.s_pool.Get(out var rep))
                        {
                            param.DamageValue = 3;
                            param.HitStopDuration = _hitStopDuration;
                            param.HitPosition = collision.contacts[0].point;
                            dmgApp.ApplyDamege(param, rep);

                            //結果をもとに処理
                            if (rep.ResultType.HasFlag(DamageReply.ResultTypes.Hit))
                            {
                                 //親へのヒット通知
                                if (MainObject.OwnerObject!=null &&
                                   MainObject.OwnerObject.DamageApp.IsUnityNull()==false)
                                {
                                    MainObject.OwnerObject.DamageApp.ApplyHit(param, rep);
                                }

                                //エフェクトを出す

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

