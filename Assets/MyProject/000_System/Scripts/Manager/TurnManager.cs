using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility.SystemEx;
using UniRx;
using UniRx.Triggers;
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
    [SerializeField,Tooltip("行動中の陣営")] MainObjectData.GroupIDs TurnIDForInspector;

    /// <summary>
    /// 陣営ごとの行動順を作成
    /// (敏捷力ソート済みのキャラクターリストを陣営ごとの要素で二次元配列(turnList)に纏める)
    /// </summary>
    public void CreateTurnList()
    {
        if(GameManager.Instance.CharacterManager.Characters.Count<=0){return;}
       
        turnList.Clear();
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
            //ターンの進行を通知
            TurnChangeRP.Value=actionCharacters.First().MainObjectData.GroupID;
        }
        
        //ターンエンドフラグをリセット
        foreach (var character in actionCharacters)
        {
            character.IsTurnEnd.Value = false;
        }
    }

    void Start()
    {
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

            TurnIDForInspector=flg;

        });
    }

    // Update is called once per frame
    void Update()
    {
        AdvanceTurn();
    }
}