using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObjectData : MonoBehaviour
{
    public enum GroupIDs
    {
        Player,
        Enemy,
        Neutral,
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
    //���O
    [SerializeField]string _name;
    public string Name => _name;
    //�I�[�i�[
    public MainObjectData OwnerObject { get; set; }

    //�_���[�W�֌W�C���^�[�t�F�[�X
    public Damage.IDamageApplicable DamageApp { get; set; }

    void Awake()
    {
        DamageApp = GetComponent<Damage.IDamageApplicable>();

    }

    
}
