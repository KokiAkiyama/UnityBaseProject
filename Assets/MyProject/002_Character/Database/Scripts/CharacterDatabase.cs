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
    public AssetReferenceGameObject Prefab{get=>prefab;set=>prefab=value;}
    [SerializeField] Sprite sprite;
    public Sprite Sprite{get=>sprite;set=>sprite=value;}
    [SerializeField] string name;
    public string Name{get=>name;set=>name=value;}
    [SerializeField] int hp=100;
    public int HP{get=>hp;set=>hp=value;}
    [SerializeField] int mp=10;
    public int MP{get=>mp;set=>mp=value;}
    [SerializeField] float actionRange=9.0f;
    public float ActionRange{get=>actionRange;set=>actionRange=value;}
    [SerializeField,Tooltip("行動リソース")] int actionPoint=1;
    public int ActionPoint{get=>actionPoint;set=>actionPoint=value;}

    //ステータス（基礎値10で能力に補正を与える）
    [Space]
    [SerializeField,Tooltip("筋力(近接攻撃)")]
    public int strength =StatusBaseValue;

    [SerializeField,Tooltip("敏捷力（回避率、行動順）")]
    public int dexterity =StatusBaseValue;

    [SerializeField,Tooltip("耐久力（最大HP）")]
    public int constitution =StatusBaseValue;

    [SerializeField,Tooltip("知力（魔法攻撃力）")]
    public int intelligence =StatusBaseValue;

    //補正値
    public int CorrectionValue_Strength=>strength-StatusBaseValue;
    public int CorrectionValue_Dexterity=>dexterity-StatusBaseValue;
    public int CorrectionValue_Constitution=>constitution-StatusBaseValue;
    public int CorrectionValue_Intelligence=>intelligence-StatusBaseValue;

    //ステータス基礎値
    public static readonly int StatusBaseValue=10;
    
}
[CreateAssetMenu(fileName = "CharacterDatabase",menuName ="My Object/CharacterDatabase")]
public class CharacterDatabase : ScriptableObject
{
    [SerializeField]List<CharacterData> characterData;
    public CharacterData GetChatacrerData(CharacterIDs id)
    {
        return characterData[(int)id];
    }
    public void Copy(ref CharacterData copiedData,CharacterIDs id)
    {
        var database=GetChatacrerData(id);
        copiedData.ID=database.ID;
        copiedData.Prefab=database.Prefab;
        copiedData.Sprite=database.Sprite;
        copiedData.Name=database.Name;
        copiedData.HP=database.HP;
        copiedData.MP=database.MP;
        copiedData.ActionRange=database.ActionRange;
        copiedData.ActionPoint=database.ActionPoint;
        copiedData.strength=database.strength;
        copiedData.dexterity=database.dexterity;
        copiedData.constitution=database.constitution;
        copiedData.intelligence=database.intelligence;

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

