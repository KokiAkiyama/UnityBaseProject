using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;


public class Group:MonoBehaviour
{
    [SerializeField] protected MainObjectData.GroupIDs groupID = MainObjectData.GroupIDs.Enemy;
    [SerializeField] protected ActiveSelector actives;
    public MainObjectData.GroupIDs GroupID => groupID;

    [SerializeField]
    public ReactiveProperty<CharacterBrain> selectedCharacter = new(null);
    


    public void AddActveControl(CharacterBrain character)=>actives.AddActveControl(character);

    public void EndActveControl(CharacterBrain character)=>actives.EndActveControl(character);

}
