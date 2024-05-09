using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility.SystemEx;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;
using System;
public class TurnManager : MonoBehaviour
{

    List<List<CharacterBrain>> turnList=new();
    /// <summary>
    /// 行動中のキャラクター
    /// </summary>
    [SerializeField] List<CharacterBrain> actionCharacters=new();

    public List<CharacterBrain> ActionCharacters=>actionCharacters;

    [SerializeField,Tooltip("ターン一周ごとに発生する行動数")] int actionCount=0;
    /// <summary>
    /// 強制ターンエンド
    /// </summary>
    [SerializeField,Tooltip("強制ターンエンド")]
    BoolReactiveProperty turnEndFlg=new(false); 
    /// <summary>
    /// ターンの終了を通知するフラグ
    /// </summary>
    public ReactiveProperty<MainObjectData.GroupIDs> TurnChangeRP{get;private set;}=new();

    /// <summary>
    /// インスペクター表示用
    /// </summary>
    [SerializeField,Tooltip("行動中の陣営")] MainObjectData.GroupIDs activeGroupID;
    public MainObjectData.GroupIDs ActiveGroupID=>activeGroupID;
    /// <summary>
    /// 行動中のキャラクターかどうか
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public bool IsActionCharacter(CharacterBrain character)=> character && actionCharacters.Contains(character) && character.IsDead==false;
    /// <summary>
    /// ゲーム開始直後の不具合を避けるための待機時間が終了しているか
    /// </summary>
    bool isEndWaitTime=false;

    /// <summary>
    /// 陣営ごとの行動順を作成
    /// (敏捷力ソート済みのキャラクターリストを陣営ごとの要素で二次元配列(turnList)に纏める)
    /// </summary>
    public void CreateTurnList()
    {
        if(GameManager.Instance.CharacterManager.Characters.Count<=0){return;}
       
        turnList.Clear();

        //キャラクターリストから死亡したものを除外
        GameManager.Instance.CharacterManager.RemoveDeadCharacter();


        //キャラクターリストを取得（敏捷力ソート済み）
        var characters=GameManager.Instance.CharacterManager.Characters;
        List<CharacterBrain> turn=new();

        MainObjectData.GroupIDs sideID=characters.First().MainObjectData.GroupID;
        
        turn.Add(characters.First());
        
        foreach(var character in characters)
        {
            if(character==null){continue;}

            if(character.MainObjectData.GroupID!=sideID)
            {
                turnList.Add(turn);
                turn=new();
                sideID=character.MainObjectData.GroupID;
            }

            if(turn.Contains(character)==false)
            {
                turn.Add(character);

                if(characters.IndexOf(character)==(characters.Count-1))
                {
                    turnList.Add(turn);
                }
            }

        }

        actionCount=turnList.Count;
    }
    /// <summary>
    /// ターンを進める
    /// </summary>
    public void AdvanceTurn()
    {
        if(GameManager.Instance.CharacterManager.Characters.Count<=0){return;}

        if(actionCharacters==null)
        {
            CreateTurnList();
            actionCharacters=turnList.First();
        }
        else
        {
            //ターンエンドしていないキャラクターがいる限りターンを進めない
            foreach(var character in actionCharacters)
            {
                if(character.IsTurnEnd.Value==false){return;}
            }


            int nextIdx=SystemEx.RangeLoopIndex(turnList.IndexOf(actionCharacters)+1,ref turnList);
            
            //リストの更新はターンが一周する際に行う
            if(nextIdx==0)
            {
                CreateTurnList();
            }
            
            actionCharacters=turnList[nextIdx];
            
        }
        
        //ターンエンドフラグをリセット
        foreach (var character in actionCharacters)
        {
            character.IsTurnEnd.Value = false;
        }

        //ターンの進行を通知
        //(一周したときに同じ陣営のターンになることを想定し、同じ値でも通知する）
        TurnChangeRP.SetValueAndForceNotify(actionCharacters.First().MainObjectData.GroupID);
    }

    async void Start()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));


        //参照先が存在しない要素を削除
        this.UpdateAsObservable()
        .Subscribe(_=>
        {
            
            actionCharacters.RemoveAll(character=>character==null);

        }).AddTo(this);
        //ターンの終了を通知するフラグ
        turnEndFlg
        .Where(flg=>flg)
        .Subscribe(flg=>{
            
            foreach(var character in actionCharacters)
            {
                if(character.IsTurnEnd.Value==false)
                {
                    character.IsTurnEnd.Value=true;
                }
            }

            turnEndFlg.Value=false;
        });

        TurnChangeRP
        .SkipLatestValueOnSubscribe()
        .Subscribe(flg=>{

            activeGroupID=flg;

        });


        isEndWaitTime=true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isEndWaitTime==false){return;}
        AdvanceTurn();
    }
}
