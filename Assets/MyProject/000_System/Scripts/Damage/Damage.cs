using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damage
{
    /// <summary>
    /// ダメージの通知データ
    /// </summary>
    public class DamageParam
    {
        public int DamageValue;//ダメージ値
        public float HitStopDuration;//ヒットストップ時間
        public Vector3 Blow;    //ぶっとばしベクトル
        public Vector3 HitPosition;//ヒット座標
        public void Reset()
        {
            DamageValue = default;
            HitStopDuration = default;
            Blow = default;
            HitPosition= default;
        }
        //オブジェクトプール
        public static UnityEngine.Pool.ObjectPool<DamageParam> s_pool=new(
            createFunc: ()=>new DamageParam(),//クラス作成塩処理
            actionOnGet: target => { target.Reset(); },//Poolから取得時の処理
            actionOnDestroy: target => {  },   //Poolへ戻す時の処理
            collectionCheck: true,           //重複チェック
            defaultCapacity: 100,            //初期の個数
            maxSize: 1000                       //最大個数
            );
    }

    public class DamageReply
    {
        //public bool IsHit;//ヒットしたか
        [System.Flags]//複数チェック可
        public enum ResultTypes
        {
            Hit  =1 << 0,
            Guard=1 << 1,
        }
        public ResultTypes ResultType;
        public int ActualDamageValue;//実際に与えたダメージ値
        public void Reset()
        {
            ResultType = 0;
            ActualDamageValue = 0;
        }
        //オブジェクトプール
        public static UnityEngine.Pool.ObjectPool<DamageReply> s_pool = new(
            createFunc: () => new DamageReply(),//クラス作成塩処理
            actionOnGet: target => { target.Reset(); },//Poolから取得時の処理
            actionOnDestroy: target => { },   //Poolへ戻す時の処理
            collectionCheck: true,           //重複チェック
            defaultCapacity: 100,            //初期の個数
            maxSize: 1000                       //最大個数
            );
    }
    /// <summary>
    /// ダメージ関係の窓口
    /// </summary>
    public interface IDamageApplicable
    {
        /// <summary>
        /// ダメージの適用
        /// </summary>
        /// <param name="param">与えるダメージの詳細</param>
        /// <param name="rep">結果受信用</param>
        void ApplyDamege(DamageParam param,DamageReply rep);
        /// <summary>
        /// 親へのヒット通知
        /// </summary>
        /// <param name="param"></param>
        /// <param name="rep"></param>
        void ApplyHit(DamageParam param,DamageReply rep);
    }
    /// <summary>
    /// ヒット済みリスト
    /// </summary>
    public class HitList
    {
        //ヒットした奴の情報
        public class Node
        {
            public MainObjectData Targrt { get; set; }
            //残り時間
            public float RemainingTime;
        }
        //無視リスト
        Dictionary<MainObjectData, Node> _objectDic = new();
        List<MainObjectData> deleteList = new(10);
        public void Clear()
        {
            _objectDic.Clear();
        }

        public bool Exist(MainObjectData target)
        {
            if(_objectDic.TryGetValue(target, out Node node))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// ヒット済みとして登録
        /// </summary>
        /// <param name="target">登録する奴</param>
        /// <param name="duration">解除されるまでの時間</param>
        public void Register(MainObjectData target,float duration)
        {
            //既に存在する？
           if(_objectDic.TryGetValue(target,out var exist))
           {
                exist.RemainingTime = duration;
           }
           else
           {
                Node node= new Node();
                node.Targrt = target;
                node.RemainingTime = duration;
                _objectDic.Add(target, node);
           }

        }
        

        public void AdvanceTime()
        {
            
            //時間を進める
            foreach(var node in _objectDic.Values)
            {
                //時間を進める
                node.RemainingTime -= Time.deltaTime;
            
                if(node.RemainingTime <= 0f)
                {
                    //削除予約
                    deleteList.Add(node.Targrt);
                }
            }
            //削除処理
            //UnityEngine.Pool.LinkedPool<MainObjectData>()
            foreach(var target in deleteList)
            {
                _objectDic.Remove(target);
            }
            deleteList.Clear();
        }
    }

}
