using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;

/// <summary>
/// 特定の陣営を指揮するAI
/// </summary>
public class AIBrain : MonoBehaviour
{
    [SerializeField]MainObjectData.GroupIDs groupID=MainObjectData.GroupIDs.Enemy;

    [SerializeField] CharacterBrain controlCharacter;

    bool isActive=false;

    // Start is called before the first frame update
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
