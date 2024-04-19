using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterIDs
{
    None,
    ID001_Zombie
}

[Serializable]
public class CharacterData
{
    [SerializeField] CharacterIDs id;
    public CharacterIDs ID
    {
        get=>id;
        set=>id=value;
    }

    [SerializeField] string name;
    [SerializeField] int hp=100;
    [SerializeField] int mp=10;
    [SerializeField] float actionRange=9.0f;
    [SerializeField] int actionPoint=1;
}
[CreateAssetMenu(fileName = "CharacterDatabase",menuName ="My Object/CharacterDatabase")]
public class CharacterDatabase : ScriptableObject
{
    [SerializeField]List<CharacterData> characterData;
    public CharacterData GetChatacrerData(CharacterIDs id)
    {
        return characterData[(int)id];
    }
    void OnValidate()
    {
        for(int i = 0; i < characterData.Count; i++)
        {
            if (characterData[i] == null) continue;
            characterData[i].ID = (CharacterIDs)i;
        }
    }
}

