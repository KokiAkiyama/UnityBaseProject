using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UniRx.Triggers;
using UniRx;
using UnityEngine.UI; 
using DG.Tweening;
public class PortraitController : MonoBehaviour
{
    List<CharacterPortrait> list=new();
    [SerializeField] ScrollRect scrollRect;
    [SerializeField]AssetReferenceGameObject portraitAssetPrefab;

    AsyncOperationHandle<GameObject> assetHandle=new();

    
    Dictionary<CharacterPortrait,Tween> actionPortraitDic=new();

    [SerializeField] float DOScalePer=1.5f;
    [SerializeField] float DOScaleDuration=0.5f;

    /// <summary>
    /// 
    /// </summary>
    public ReactiveProperty<CharacterBrain> SelectedPortraitCharacter=new();

    [SerializeField]Color SelectedColor=Color.white;

    public void Create(List<List<CharacterBrain>> trunLsit)
    {
        Cear();
        foreach(var characters in trunLsit)
        {
            foreach (CharacterBrain character in characters)
            {
                var portrait=Instantiate(assetHandle.Result,scrollRect.content.transform).GetComponent<CharacterPortrait>();
                portrait.Character=character;

                //scrollRect.horizontalScrollbarSpacing=portrait.RectTransform.sizeDelta.x;

                list.Add(portrait);

            }
        }
    }

    void Cear()
    {
        list.RemoveAll(portrait=>
        {
            Destroy(portrait.gameObject);
            return true;
        });
    }

    
    // Start is called before the first frame update
    void Awake()
    {
        assetHandle=
            Addressables.LoadAssetAsync<GameObject>(portraitAssetPrefab);
        
        assetHandle.WaitForCompletion();
            
       this.OnDestroyAsObservable()
       .Subscribe(_ =>
       {
            Addressables.Release(assetHandle);
       });
       
    }

    void Start()
    {
        //ターン中のキャラクターを強調表示
        GameManager.Instance.TurnManager.TurnChangeRP
        .SkipLatestValueOnSubscribe()
        .Subscribe(_=>
        {
            foreach(var actionPair in actionPortraitDic)
            {
                actionPair.Value.Kill();

                if(actionPair.Key)
                {
                    actionPair.Key.transform.localScale = Vector3.one;
                }
            }
            actionPortraitDic.Clear();

            var trunManager=GameManager.Instance.TurnManager;
            foreach(var portrait in list)
            {
                if(trunManager.IsActionCharacter(portrait.Character)==false)continue;
                Sequence seq = DOTween.Sequence();
                seq.Append(portrait.RectTransform.DOScale(Vector3.one*DOScalePer,DOScaleDuration));
            
                actionPortraitDic[portrait]=seq;
            }


        }).AddTo(this);


        SelectedPortraitCharacter
        .Zip(SelectedPortraitCharacter.Skip(1),(Old,New)=>new{Old,New})
        .Subscribe(selectCharacterPair=>
        {



            if(selectCharacterPair.New)
            {
                list.Select(portrait=>
                {
                    if(portrait.Character==selectCharacterPair.New)
                    {
                        portrait.SetSelectedColor(SelectedColor);
                    }
                    return portrait;
                });
                
            }

            if(selectCharacterPair.Old)
            {
                list.Select(portrait=>
                {
                    if(portrait.Character==selectCharacterPair.Old)
                    {
                        portrait.ResetColor();
                    }
                    return portrait;
                });
            }


        });
    }

    // Update is called once per frame
    void Update()
    {
        list.RemoveAll(portrait=>portrait==null);
    }

}
