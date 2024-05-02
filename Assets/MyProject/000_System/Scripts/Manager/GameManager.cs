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
    /// <summary>
    /// 敵対する陣営を取得
    /// </summary>
    /// <param name="groupID"></param>
    /// <returns></returns>
    public MainObjectData.GroupIDs GetEnemyFlag(MainObjectData.GroupIDs groupID)
    {
        return (~groupID) & (~MainObjectData.GroupIDs.Neutral);
    }
    /// <summary>
    /// 二つの陣営は敵対しているか
    /// </summary>
    /// <param name="fiest"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public bool IsEnemies(MainObjectData.GroupIDs fiest, MainObjectData.GroupIDs second)
    {
        return fiest.HasFlag(GetEnemyFlag(second));
    }
}

public static class MainObjectDataExtensions
{
    /// <summary>
    /// 引数の陣営が敵対的かどうか
    /// </summary>
    /// <param name="self"></param>
    /// <param name="checkID"></param>
    /// <returns></returns>
    public static bool IsEnemies(this MainObjectData self, MainObjectData.GroupIDs checkID)
    {
        return checkID.HasFlag(GameManager.Instance.GetEnemyFlag(self.GroupID));
    }
}
