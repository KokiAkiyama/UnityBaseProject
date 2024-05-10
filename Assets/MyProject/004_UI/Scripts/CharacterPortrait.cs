using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Triggers;
using UniRx;

public class CharacterPortrait : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Image frame;

    RectTransform rectTransform;
    public RectTransform RectTransform=>rectTransform;

    public ReactiveProperty<CharacterBrain> characterRP=new(null);
    public CharacterBrain Character{get=>characterRP.Value;set=>characterRP.Value=value;}

    void Awake()
    {
        characterRP
        .SkipLatestValueOnSubscribe()
        .Where(Character=>Character!=null)
        .Subscribe(character=>
        {
            image.sprite=character.Param.Sprite;
            frame.color=GameManager.Instance.CharacterManager.GroupColorDic[character.MainObjectData.GroupID];
        });

        rectTransform=GetComponent<RectTransform>();
    }
}
