using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UniRx.Triggers;
using UniRx;
using TMPro;
using UnityEngine.UI;
using Utility.SystemEx;
public class PortraitController : MonoBehaviour
{
    List<CharacterPortrait> list=new();
    [SerializeField] ScrollRect scrollRect;
    [SerializeField]AssetReferenceGameObject portraitAssetPrefab;

    AsyncOperationHandle<GameObject> assetHandle=new();
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

    // Update is called once per frame
    void Update()
    {
        list.RemoveAll(portrait=>portrait==null);
    }


}
