using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
    [SerializeField] AssetReferenceGameObject prefab;
    public AssetReferenceGameObject Prefab =>prefab;
    [SerializeField] Sprite sprite;
    public Sprite Sprite => sprite;
    [SerializeField] string name;
    public string Name => name;
    [SerializeField] int hp=100;
    public int HP =>hp;
    [SerializeField] int mp=10;
    public int MP =>mp;
    [SerializeField] float actionRange=9.0f;
    public float ActionRange => actionRange;
    [SerializeField] int actionPoint=1;
    public int ActionPoint =>actionPoint;
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

