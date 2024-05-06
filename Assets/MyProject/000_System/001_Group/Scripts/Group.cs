using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Group:MonoBehaviour
{
    [SerializeField] protected MainObjectData.GroupIDs groupID = MainObjectData.GroupIDs.Enemy;

    public MainObjectData.GroupIDs GroupID => groupID;

    public virtual void AddActveControl(CharacterBrain character) { }

    public virtual void EndActveControl(CharacterBrain character) { }

}
