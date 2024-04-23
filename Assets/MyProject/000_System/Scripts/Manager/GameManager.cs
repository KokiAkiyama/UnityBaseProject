using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] PlayerInputManager inputManager;
    public PlayerInputManager InputManager=>inputManager;
    [SerializeField]CharacterManager characterManager;
    public CharacterManager CharacterManager => characterManager;

    [SerializeField] TurnManager turnManager;
    public TurnManager TurnManager => turnManager;
    // Start is called before the first frame update
    void Awake()
    {
        Instance=this;
    }

    void Start()
    {
        
    }
    void Update()
    {
        var players=characterManager.Characters.FindAll(character=>character.MainObjectData.GroupID==MainObjectData.GroupIDs.Player);
        var enemies=characterManager.Characters.FindAll(character=>character.MainObjectData.GroupID==MainObjectData.GroupIDs.Enemy);
        //勝利
        if(enemies.Count<=0)
        {
            Debug.Log("Win！");
            return;
        }
        //敗北
        if(players.Count<=0)
        {
            Debug.Log("lose！");
            return;
        }


    }
}
