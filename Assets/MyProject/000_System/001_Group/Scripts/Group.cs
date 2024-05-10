using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using UnityEngine.TextCore.Text;


public class Group:MonoBehaviour
{
    [SerializeField] protected MainObjectData.GroupIDs groupID = MainObjectData.GroupIDs.Enemy;
    [SerializeField] protected ActiveSelector actives;
    public MainObjectData.GroupIDs GroupID => groupID;

    [SerializeField]
    public ReactiveProperty<CharacterBrain> selectedCharacter = new(null);
    


    public void AddActveControl(CharacterBrain character)=>actives.AddActveControl(character);

    public void EndActveControl(CharacterBrain character)=>actives.EndActveControl(character);


    protected virtual void Start()
    {
        //選択されているキャラクターをUIに反映
        selectedCharacter
        .SkipLatestValueOnSubscribe()
        .Subscribe(character=>
        {
            if(GameManager.Instance.TurnManager.ActiveGroupID!=groupID)return;
            GameManager.Instance.UIManager.PortraitController
            .SelectedPortraitCharacter.Value=character;

        }).AddTo(this);

        GameManager.Instance.TurnManager.TurnChangeRP
        .SkipLatestValueOnSubscribe()
        .Subscribe(turnGroupID=>
        {
            if(turnGroupID!=groupID)return;

            GameManager.Instance.UIManager.PortraitController
            .SelectedPortraitCharacter.Value=selectedCharacter.Value;
        }).AddTo(this);
    }
}
