using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// 特定の陣営を指揮するAI
/// </summary>
public class AIBrain : MonoBehaviour
{
    [SerializeField]MainObjectData.GroupIDs groupID=MainObjectData.GroupIDs.Enemy;

    [SerializeField] CharacterBrain controlCharacter;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.TurnManager
        .TurnChangeRP
        .Where(id=>id==groupID)
        .Subscribe(id=>
        {
            



        }).AddTo(this);
    }
}
