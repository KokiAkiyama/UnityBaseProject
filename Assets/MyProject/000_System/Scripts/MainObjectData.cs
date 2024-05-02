using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObjectData : MonoBehaviour
{
    [Flags]
    public enum GroupIDs
    {
        Player =1<<0,
        Enemy  =1<<1,
        Neutral=1<<2,
    }
    /// <summary>
    /// フラグに自身の属するグループが含まれているか
    /// </summary>
    /// <param name="checkFlg"></param>
    public bool Check(GroupIDs checkFlg)=>checkFlg.HasFlag(groupID);
    //ID
    [SerializeField]GroupIDs groupID;
    public GroupIDs GroupID /*=> _groupID;*/
    {
        get 
        {
            if(OwnerObject != null) { return OwnerObject.GroupID; }
            return groupID; 
        }
    }
    //���O
    [SerializeField]string name_data;
    public string Name => name_data;
    //�I�[�i�[
    public MainObjectData OwnerObject { get; set; }

    //�_���[�W�֌W�C���^�[�t�F�[�X
    public Damage.IDamageApplicable DamageApp { get; set; }

    void Awake()
    {
        DamageApp = GetComponent<Damage.IDamageApplicable>();

    }

    
}
