using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField]
    private ReactiveCollection<CharacterBrain> selectedList=new();

    [SerializeField]
    LayerMask selectLayer;
    [SerializeField]
    LayerMask stageLayer;

    [SerializeField] Material selectMaterial;

    //レンダラーの情報を保存する
    public class RendererStrage
    {
        public RendererStrage(List<Material> mats, List<Renderer> rends)
        {
            materials = mats;
            renderers = rends;
        }
        public List<Material> materials;
        public List<Renderer> renderers;

    }


    Dictionary<CharacterBrain, RendererStrage> rendererDic=new();
    // Start is called before the first frame update
    void Start()
    {
        selectedList.
        ObserveAdd().
        Subscribe(characterBrain =>
        {
            SetSelectMaterial(characterBrain.Value);

        }).AddTo(this);

        selectedList.ObserveReset().
            Subscribe(_ =>
            {
                foreach(var pair in rendererDic)
                {
                    ResetMaterials(pair.Key);
                }
                

            }).AddTo(this);

        selectedList.
        ObserveRemove().
        Subscribe(characterBrain =>
        {
            ResetMaterials(characterBrain.Value);

        }).AddTo(this);

    }

    // Update is called once per frame
    void Update()
    {
        SelectCharacter();
        MoveCharacter();
    }

    void SelectCharacter()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedList.Clear();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool isHit = Physics.Raycast(
                 ray: ray,
                 hitInfo: out RaycastHit hit,
                 maxDistance: Mathf.Infinity,
                 layerMask: selectLayer
             );

            if (isHit)
            {
                var character= hit.collider.GetComponent<CharacterBrain>();
                if(character && character.MainObjectData.GroupID==MainObjectData.GroupIDs.Player 
                && selectedList.Contains(character)==false)
                {
                    selectedList.Add(character);
                }


            }
        }
    }

    void MoveCharacter()
    {
        if(selectedList.Count<=0)return;
        if(GameManager.Instance.InputManager.Game["CamRotButton"].IsPressed()){return;}
        if (Input.GetMouseButtonDown(1))
        {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool isHit = Physics.Raycast(
                 ray: ray,
                 hitInfo: out RaycastHit hit,
                 maxDistance: Mathf.Infinity,
                 layerMask: stageLayer | selectLayer,
                 queryTriggerInteraction:QueryTriggerInteraction.Collide
             );
            
            if(isHit==false){return;}

            
            if (Utility.MathEx.MathEx.ContainsLayerInMask(hit.collider.gameObject.layer, stageLayer))
            {
                foreach(var selected in selectedList)
                {
                    selected.AIInputProvider.SetDestination(hit.point);
                }
            }
            else if(Utility.MathEx.MathEx.ContainsLayerInMask(hit.collider.gameObject.layer, selectLayer))
            {
                var character=hit.collider.GetComponent<CharacterBrain>();
                if(character.MainObjectData.GroupID==MainObjectData.GroupIDs.Enemy)
                {
                    foreach (var selected in selectedList)
                    {
                        selected.AIInputProvider.Target.Value= character;
                    }
                }
            }
        }

    }

    //アウトライン用のレイヤー切り替え
    void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform transform in go.transform)
        {
            if (transform.gameObject.name == "Other") { continue; }
            SetLayer(transform.gameObject, layer);
        }
    }

    void SetSelectMaterial(CharacterBrain characterBrain)
    {
        List<Renderer> renderers = null;
        List<Material> prevMaterials = new();
        bool inDic = rendererDic.ContainsKey(characterBrain);
        if (inDic)
        {
            renderers = rendererDic[characterBrain].renderers;
        }
        else
        {
            renderers = characterBrain.GetComponentsInChildren<Renderer>().ToList();

        }
        
        foreach(var renderer in renderers)
        {
            if(inDic==false)
            {
                prevMaterials.Add(renderer.material);
            }
            renderer.material=selectMaterial;
        }
        rendererDic[characterBrain] = new RendererStrage(inDic==false?prevMaterials: rendererDic[characterBrain].materials, renderers);
        
    }
    void ResetMaterials(CharacterBrain characterBrain)
    {
        if (rendererDic.ContainsKey(characterBrain) == false) { return; }

        var renderers = rendererDic[characterBrain].renderers;
        var prevMaterials = rendererDic[characterBrain].materials;
        for(int i=0;i<renderers.Count;++i)
        {
            renderers[i].material=prevMaterials[i];
        }
    }


    private void OnDrawGizmos()
    {
        if(selectedList.Count<=0){return;}

        Gizmos.color = Color.blue;

        var ray=Camera.main.ScreenPointToRay(Input.mousePosition);

        bool isHit = Physics.Raycast(
                 ray: ray,
                 hitInfo: out RaycastHit hit,
                 maxDistance: Mathf.Infinity,
                 layerMask: stageLayer,
                 queryTriggerInteraction:QueryTriggerInteraction.Collide
             );

        if(isHit==false){return;}

        if(Utility.MathEx.MathEx.ContainsLayerInMask(hit.collider.gameObject.layer, stageLayer)==false)
        {
            return;
        }

        foreach(var selected in selectedList)
        {
            selected.DrawGizmosCalceCorners(hit.point);
        }

    }
}
