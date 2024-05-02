using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using Utility.MathEx;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField]
    private ReactiveCollection<CharacterBrain> selectedList=new();

    [SerializeField]
    LayerMask selectLayer;
    [SerializeField]
    LayerMask stageLayer;

    [SerializeField] Material selectMaterial;
    /// <summary>
    /// 現在アクション途中のキャラクター一覧
    /// (要素が存在する時は新たな指示を与えることができない)
    /// </summary>
    [SerializeField]
    List<CharacterBrain> ActiveControls=new();
    void AddActveControl(CharacterBrain character)
    {
        // if(ActiveControls.Contains(character)){return;}
        // ActiveControls.Add(character);
    }

    public void EndActveControl(CharacterBrain character)
    {
        if(ActiveControls.Contains(character)==false){return;}
        ActiveControls.Remove(character);
    }

    public bool CanControl=>ActiveControls.Count<=0;

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
        ActiveControls.RemoveAll(character=>character==null);
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
        if(CanControl==false)return;
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

            
            if (MathEx.ContainsLayerInMask(hit.collider.gameObject.layer, stageLayer))
            {
                foreach(var selected in selectedList)
                {
                    AddActveControl(selected);
                    selected.AIInputProvider.SetDestination(hit.point);
                }
            }
            else if(MathEx.ContainsLayerInMask(hit.collider.gameObject.layer, selectLayer))
            {
                var character=hit.collider.GetComponent<CharacterBrain>();
                if(character.MainObjectData.GroupID==MainObjectData.GroupIDs.Enemy)
                {
                    foreach (var selected in selectedList)
                    {
                        AddActveControl(selected);
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
                 layerMask: stageLayer|selectLayer,
                 queryTriggerInteraction:QueryTriggerInteraction.Collide
             );

        if(isHit==false){return;}
        Vector3 destPos=new();
        if(MathEx.ContainsLayerInMask(hit.collider.gameObject.layer, selectLayer))
        {
            if(hit.collider.GetComponent<MainObjectData>().GroupID==MainObjectData.GroupIDs.Enemy)
            {
                destPos=hit.transform.position;
            }
            else
            {
                return;
            }
            
        }
        else if(MathEx.ContainsLayerInMask(hit.collider.gameObject.layer, stageLayer))
        {
            destPos=hit.point;
            
        }
        else
        {
            return;
        }

        foreach(var selected in selectedList)
        {
            Color color=GameManager.Instance.TurnManager.IsActionCharacter(selected)?Color.blue:Color.red;

            selected.DrawGizmosCalceCorners(destPos,color);
        }

    }
}
