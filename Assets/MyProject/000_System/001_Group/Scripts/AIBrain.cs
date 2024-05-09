using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using System;
using UnityEngine.TextCore.Text;

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

    void Start()
    {
        

        GameManager.Instance.TurnManager
        .TurnChangeRP
        .Subscribe(id=>
        {
            isActive=(id==groupID);

        }).AddTo(this);
        //行動が終了した場合、即ターンエンドフラグをオンに
        actives.OnEndActiveControlRP
        .SkipLatestValueOnSubscribe()
        .Where(character=>character!=null)
        .Subscribe(character=>
        {
            if(character.MainObjectData.GroupID==groupID && isActive)
            {
                character.IsTurnEnd.Value=true;
                
            }
        }).AddTo(this);
    }
    
    async void Update()
    {
        if(isActive==false)return;


        var turnManager=GameManager.Instance.TurnManager;
        CharacterBrain character=null;
        int idx=0;
        while (true)
        {
            ////============================
            ////行動終了まで待機
            ////============================
            await WaitForEndActive(character);
            
            if(idx>=turnManager.ActionCharacters.Count)
            {
                break;
            }

            character=turnManager.ActionCharacters[idx];
            
            ++idx;

            if(character==null)continue;
            
            if(isWaiting==false && character.IsTurnEnd.Value==false)
            {
                SearchEnemy(character);
            }
        }


        
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


    async UniTask WaitForEndActive(CharacterBrain character)
    {
        if(character==null){return;}

        var cancelToken = this.GetCancellationTokenOnDestroy();

        while (cancelToken.IsCancellationRequested == false)
        {
            if (character.IsTurnEnd.Value)
            {
                isWaiting=false;
                break;
            }

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
    }
}
