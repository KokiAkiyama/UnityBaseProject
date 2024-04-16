using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObjectData : MonoBehaviour
{
    public enum GroupIDs
    {
        Group00,
        Group01,
        Group02,
        Group03,
        Group99=99,
    }
    //ID
    [SerializeField]GroupIDs _groupID;
    public GroupIDs GroupID /*=> _groupID;*/
    {
        get 
        {
            if(OwnerObject != null) { return OwnerObject.GroupID; }
            return _groupID; 
        }
    }
    //名前
    [SerializeField]string _name;
    public string Name => _name;
    //オーナー
    public MainObjectData OwnerObject { get; set; }

    //ダメージ関係インターフェース
    public Damage.IDamageApplicable DamageApp { get; set; }

    void Awake()
    {
        DamageApp = GetComponent<Damage.IDamageApplicable>();

    }
}
