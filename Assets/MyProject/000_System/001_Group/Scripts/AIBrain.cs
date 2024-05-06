using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;

/// <summary>
/// 特定の陣営を指揮するAI
/// </summary>
public class AIBrain : Group
{
    
    [SerializeField] CharacterBrain controlCharacter;

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

    void Update()
    {
        if(isActive==false)return;
        
        var turnManager=GameManager.Instance.TurnManager;

        foreach(var character in turnManager.ActionCharacters)
        {
            if(character==null)continue;
    
            controlCharacter=character;
            
            SearchEnemy();
            //============================
            //行動終了
            //============================

            //controlCharacter.IsTurnEnd.Value=true;
        }
        
        isActive=false;
        
    }

    //===================================================
    //固有
    //===================================================
    void SearchEnemy()
    {
        var gameManager=GameManager.Instance;
        List<CharacterBrain> enemies=new();

        gameManager.CharacterManager.Search(
        gameManager.GetEnemyFlag(controlCharacter.MainObjectData.GroupID),
        controlCharacter,
        ref enemies);


        if(enemies.Count==0)
        {
            return;
        }

        controlCharacter.AIInputProvider.Target.Value=enemies.First();

    }

    
}
