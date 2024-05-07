using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utility.UnityEngineEx;
using UnityEngine;

public class DestPosGuide : MonoBehaviour
{
    Material defaultMaterial;
    [SerializeField] Material toAttackMaterial;
    [SerializeField]RendererStrage rendererStrage=new(new(),new());
    
    // Start is called before the first frame update
    void Start()
    {
        rendererStrage.SetRenderers(gameObject);
    }

    public void SetToAttackMode(bool isActive)
    {
        if(isActive)
        {
            rendererStrage.SetMaterials(toAttackMaterial);
        }
        else
        {
            rendererStrage.ResetMaterials();
        }
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
