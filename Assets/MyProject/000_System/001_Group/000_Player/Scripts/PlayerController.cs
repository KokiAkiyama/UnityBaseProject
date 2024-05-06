using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility.MathEx;
using UniRx;

public class PlayerController : Group
{

    [SerializeField]
    Camera activeCamera;

    [SerializeField]
    ActiveSelector actives;

    [SerializeField]
    LayerMask controlLayer;
    [SerializeField]
    LayerMask stageLayer;

    [SerializeField]
    private ReactiveCollection<CharacterBrain> selectedList = new();

    [SerializeField] Material controlMaterial;
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
    Dictionary<CharacterBrain, RendererStrage> rendererDic = new();


    //===================================================
    //継承
    //===================================================
    public override void AddActveControl(CharacterBrain character) => actives.AddActveControl(character);

    public override void EndActveControl(CharacterBrain character)=>actives.EndActveControl(character);
    //===================================================
    //Unityイベント
    //===================================================
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
                foreach (var pair in rendererDic)
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

    //===================================================
    //Unityイベント
    //===================================================
    void Update()
    {
        SelectCharacter();
        MoveCharacter();
    }

    //===================================================
    //固有
    //===================================================
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
                 layerMask: controlLayer
             );

            if (isHit)
            {
                var character = hit.collider.GetComponent<CharacterBrain>();
                if (character && character.MainObjectData.GroupID == groupID
                && selectedList.Contains(character) == false)
                {
                    selectedList.Add(character);
                }


            }
        }
    }

    void MoveCharacter()
    {
        if (selectedList.Count <= 0) return;
        if (actives.CanControl == false) return;
        if (GameManager.Instance.TurnManager.ActiveGroupID != groupID) { return; }
        if (GameManager.Instance.InputManager.Game["CamRotButton"].IsPressed()) { return; }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool isHit = Physics.Raycast(
             ray: ray,
             hitInfo: out RaycastHit hit,
             maxDistance: Mathf.Infinity,
             layerMask: stageLayer | controlLayer,
             queryTriggerInteraction: QueryTriggerInteraction.Collide
         );

            if (isHit == false) { return; }


            if (MathEx.ContainsLayerInMask(hit.collider.gameObject.layer, stageLayer))
            {
                foreach (var selected in selectedList)
                {
                    selected.AIInputProvider.SetDestination(hit.point);
                }
            }
            else if (MathEx.ContainsLayerInMask(hit.collider.gameObject.layer, controlLayer))
            {
                var character = hit.collider.GetComponent<CharacterBrain>();
                if (character.MainObjectData.IsEnemies(groupID))
                {
                    foreach (var selected in selectedList)
                    {
                        selected.AIInputProvider.Target.Value = character;
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

        foreach (var renderer in renderers)
        {
            if (inDic == false)
            {
                prevMaterials.Add(renderer.material);
            }
            renderer.material = controlMaterial;
        }
        rendererDic[characterBrain] = new RendererStrage(inDic == false ? prevMaterials : rendererDic[characterBrain].materials, renderers);

    }
    void ResetMaterials(CharacterBrain characterBrain)
    {
        if (rendererDic.ContainsKey(characterBrain) == false) { return; }

        var renderers = rendererDic[characterBrain].renderers;
        var prevMaterials = rendererDic[characterBrain].materials;
        for (int i = 0; i < renderers.Count; ++i)
        {
            renderers[i].material = prevMaterials[i];
        }
    }

    //===================================================
    //ギズモ
    //===================================================
    private void OnDrawGizmos()
    {
        if (selectedList.Count <= 0) { return; }

        Gizmos.color = Color.blue;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool isHit = Physics.Raycast(
                 ray: ray,
                 hitInfo: out RaycastHit hit,
                 maxDistance: Mathf.Infinity,
                 layerMask: stageLayer | controlLayer,
                 queryTriggerInteraction: QueryTriggerInteraction.Collide
             );

        if (isHit == false) { return; }
        Vector3 destPos = new();
        if (MathEx.ContainsLayerInMask(hit.collider.gameObject.layer, controlLayer))
        {
            if (hit.collider.GetComponent<MainObjectData>().GroupID == MainObjectData.GroupIDs.Enemy)
            {
                destPos = hit.transform.position;
            }
            else
            {
                return;
            }

        }
        else if (MathEx.ContainsLayerInMask(hit.collider.gameObject.layer, stageLayer))
        {
            destPos = hit.point;

        }
        else
        {
            return;
        }

        foreach (var selected in selectedList)
        {
            Color color = Color.blue;
            bool isActionCharacters = GameManager.Instance.TurnManager.IsActionCharacter(selected);
            if (isActionCharacters == false)
            {
                color = Color.red;
            }
            else if (actives.CanControl == false && actives.ActiveControls.Contains(selected) == false)
            {
                color = Color.red;
            }

            selected.DrawGizmosCalceCorners(destPos, color);
        }

    }


}
