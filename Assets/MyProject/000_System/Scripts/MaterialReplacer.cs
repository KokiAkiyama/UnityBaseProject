using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utility.UnityEngineEx;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

public class MaterialReplacer : MonoBehaviour
{
    [SerializeField]RendererStrage rendererStrage=new(new(),new());
    
    public BoolReactiveProperty ChangeRP=new(false);

    // Start is called before the first frame update
    void Start()
    {
        rendererStrage.SetRenderers(gameObject);

        ChangeRP.
        SkipLatestValueOnSubscribe()
        .Subscribe(isActive=>
        {
            if(isActive)
            {
                rendererStrage.SetMaterials();
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
