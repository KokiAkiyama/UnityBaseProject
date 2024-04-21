using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] CharacterDatabase database;

    public CharacterData GetChatacrerData(CharacterIDs id) => database.GetChatacrerData(id);

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
