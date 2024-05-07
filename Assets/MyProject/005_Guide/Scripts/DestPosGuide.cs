using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utility.UnityEngineEx;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

public class DestPosGuide : MonoBehaviour
{
    Material defaultMaterial;
    [SerializeField] Material toAttackMaterial;
    [SerializeField]RendererStrage rendererStrage=new(new(),new());
    
    public BoolReactiveProperty ToAttackMode=new(false);

    // Start is called before the first frame update
    void Start()
    {
        rendererStrage.SetRenderers(gameObject);

        ToAttackMode.
        SkipLatestValueOnSubscribe()
        .Subscribe(isActive=>
        {
            if(isActive)
            {
                rendererStrage.SetMaterials(toAttackMaterial);
            }
            else
            {
                rendererStrage.ResetMaterials();
            }

        });

    }


    void OnEnable()
    {
        foreach(var renderer in  rendererStrage.renderers)
        {
            renderer.enabled = true;
        }
    }

    void OnDisable()
    {
        foreach(var renderer in  rendererStrage.renderers)
        {
            renderer.enabled = false;
        }
    }

}
