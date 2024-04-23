using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility.SystemEx;
using UniRx;
using UniRx.Triggers;
public class TurnManager : MonoBehaviour
{

    public List<List<CharacterBrain>> trunList=new();

    [SerializeField] List<CharacterBrain> actionCharacters=new();

    [SerializeField] int turnCount=0;
    /// <summary>
    /// 陣営ごとの行動順を作成
    /// </summary>
    public void CreateTurnList()
    {
        if(GameManager.Instance.CharacterManager.Characters.Count<=0){return;}
       
        trunList.Clear();
        var characters=GameManager.Instance.CharacterManager.Characters;
        List<CharacterBrain> turn=new();
        MainObjectData.GroupIDs sideID=characters.First().MainObjectData.GroupID;
        turn.Add(characters.First());
        
        foreach(var character in characters)
        {
            
            if(character==null){continue;}
            if(character.MainObjectData.GroupID!=sideID)
            {
                trunList.Add(turn);
                turn=new();
                sideID=character.MainObjectData.GroupID;
            }
            if(turn.Contains(character)==false)
            {
                turn.Add(character);

                if(characters.IndexOf(character)==(characters.Count-1))
                {
                    trunList.Add(turn);
                }
            }

        }

        turnCount=trunList.Count;
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
            actionCharacters=trunList.First();
        }
        else
        {
            //ターンエンドしていないキャラクターがいる限りターンを進めない
            foreach(var character in actionCharacters)
            {
                if(character.IsTurnEnd==false){return;}
            }


            int nextIdx=SystemEx.RangeLoopIndex(trunList.IndexOf(actionCharacters)+1,ref trunList);
            
            //リストの更新はターンが一周する際に行う
            if(nextIdx==0)
            {
                CreateTurnList();
            }
            
            actionCharacters=trunList[nextIdx];

        }
        
        actionCharacters.Select(character => character.IsTurnEnd=false);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.UpdateAsObservable()
        .Subscribe(_=>
        {
            
            actionCharacters.RemoveAll(character=>character==null);

        }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        AdvanceTurn();
    }
}
