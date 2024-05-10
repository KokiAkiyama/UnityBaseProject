using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using System;
using UnityEngine.TextCore.Text;
using Utility.MathEx;
using Utility.UnityEngineEx;
using Utility.SystemEx;

/// <summary>
/// 特定の陣営を指揮するAI
/// </summary>
public class AIBrain : Group
{

    bool isActive=false;
    /// <summary>
    /// ターン開始時の1フレーム目か
    /// </summary>
    bool isWaiting = false;

    //===================================================
    //Unityイベント
    //===================================================

    protected override void Start()
    {
        base.Start();

        GameManager.Instance.TurnManager
        .TurnChangeRP
        .Subscribe(id=>
        {
            isActive=(id==groupID);
            if(isActive)
            {
                isWaiting=false;
                idx=0;
            }
        }).AddTo(this);
        //行動が終了した場合、即ターンエンドフラグをオンに
        actives.OnEndActiveControlRP
        .SkipLatestValueOnSubscribe()
        .Where(character=>character!=null)
        .Subscribe(character=>
        {
            if(character.MainObjectData.GroupID==groupID && isActive)
            {
                isWaiting=false;
                
            }
        }).AddTo(this);
    }
    int idx=0;
    void Update()
    {
        if(isActive==false)return;


        var turnManager=GameManager.Instance.TurnManager;


        while (true)
        {
            
            ////============================
            ////行動終了まで待機
            ////============================
            if(WaitForEndActive())
            {
                return;
            }
            else if(idx>0)
            {
                selectedCharacter.Value=GameManager.Instance.TurnManager.ActionCharacters[SystemEx.ClampRangeIndex(idx-1,GameManager.Instance.TurnManager.ActionCharacters)];
                selectedCharacter.Value.IsTurnEnd.Value=true;
            }
            
            
            
            if(idx>=turnManager.ActionCharacters.Count)
            {
                
                idx=0;
            
                break;
            }

            
            selectedCharacter.Value=turnManager.ActionCharacters[idx];

            ++idx;

            if (selectedCharacter.Value.IsDead) continue;
            
            if(isWaiting)return;

            SearchEnemy(selectedCharacter.Value);
            
            return;
        }

        isActive=false;
        
    }

    //===================================================
    //固有
    //===================================================
    void SearchEnemy(CharacterBrain character)
    {
        var gameManager=GameManager.Instance;
        List<CharacterBrain> enemies=new();

        gameManager.CharacterManager.Search(
        gameManager.GetEnemyFlag(character.MainObjectData.GroupID),
        character,
        ref enemies);


        if(enemies.Count==0)
        {
            return;
        }

        character.AIInputProvider.Target.Value=enemies.First();

        isWaiting=true;
        
    }


    bool WaitForEndActive()
    {
        if (isWaiting==false)
        {   
            return false;
        }

        return true;
            
    }
}
