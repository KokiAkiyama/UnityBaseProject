using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;

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

    async void Update()
    {
        if(!isActive==false)return;
        
        var turnManager=GameManager.Instance.TurnManager;

        foreach(var character in turnManager.ActionCharacters)
        {
            if(character==null)continue;
            
            controlCharacter=character;







        }
    }


    void SearchEnemy()
    {

    }



}
