using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using System;

/// <summary>
/// 特定の陣営を指揮するAI
/// </summary>
public class AIBrain : Group
{

    bool isActive=false;


    //===================================================
    //Unityイベント
    //===================================================

    void Start()
    {
        

        GameManager.Instance.TurnManager
        .TurnChangeRP
        .Subscribe(id=>
        {
            isActive=(id==groupID);
        
        }).AddTo(this);
    }

    async void Update()
    {
        if(isActive==false)return;
        
        var turnManager=GameManager.Instance.TurnManager;
        var cancelToken = this.GetCancellationTokenOnDestroy();
        foreach (var character in turnManager.ActionCharacters)
        {
            if(character==null)continue;

            SearchEnemy(character);

            //============================
            //行動終了まで待機
            //============================
            while (actives.CanControl==false)
            {
                try
                {
                    await UniTask.DelayFrame(1, cancellationToken: cancelToken);
                }
                catch (OperationCanceledException oce)
                {
                    Debug.Log(oce.Message);
                    return;
                }
            }

            character.IsTurnEnd.Value = true;

            //controlCharacter.IsTurnEnd.Value=true;
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

    }

    
}
