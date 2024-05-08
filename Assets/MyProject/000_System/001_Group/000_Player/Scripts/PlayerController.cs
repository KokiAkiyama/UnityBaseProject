using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility.MathEx;
using UniRx;
using Utility.UnityEngineEx;
using System;

public class PlayerController : Group
{

    [SerializeField]
    Camera activeCamera;

    [SerializeField]
    LayerMask controlLayer;
    [SerializeField]
    LayerMask stageLayer;

    [SerializeField]
    private ReactiveProperty<CharacterBrain> selectedCharacter = new(null);

    [SerializeField] Material controlMaterial;
    //�����_���[�̏���ۑ�����
    
    Dictionary<CharacterBrain, RendererStrage> rendererDic = new();

    [SerializeField]GameObject destPosGuidePrefab;
    MaterialReplacer destPosGuide=null;

    Vector3 mouseRayHitPos=new();

    RaycastEx mouseRaycast=new();

    [SerializeField]LineRenderer routeGuideRenderer;


    //===================================================
    //Unityイベント関数
    //===================================================
    void Start()
    {
        destPosGuide=Instantiate(destPosGuidePrefab, transform).GetComponent<MaterialReplacer>();
        
        destPosGuide.enabled=false;

        selectedCharacter
        .Zip(selectedCharacter.Skip(1),(Old,New)=>new{Old,New})
        .Subscribe(selectCharacterPair=>
        {
            if(selectCharacterPair.New)
            {
                SetSelectMaterial(selectCharacterPair.New);
                
            }

            if(selectCharacterPair.Old==null){return;}

            ResetMaterials(selectCharacterPair.Old);
        });

        // selectedCharacter.
        // Where(character=>character!=null).
        // Subscribe(characterBrain =>
        // {
        //     SetSelectMaterial(characterBrain);

        // }).AddTo(this);

        // selectedCharacter.
        // SkipLatestValueOnSubscribe().
        // Where(character=>character==null).
        // Scan((prev,now)=>
        // {
        //     if(prev!=null)
        //     {
        //         ResetMaterials(prev);
        //     }

        //     return prev;

        // });

    }

    //===================================================
    //Unity�C�x���g
    //===================================================
    void Update()
    {
        UpdateMouseRay();
        SelectCharacter();
        MoveCharacter();
        UpdateDestPosGuide();
    }

    //===================================================
    //独自
    //===================================================

    void UpdateMouseRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        mouseRaycast.Raycast(ray,Mathf.Infinity,stageLayer | controlLayer);

        if (mouseRaycast.IsHit == false) { return; }


        List<RaycastHit> hitsInfo=null;

        if (mouseRaycast.GetHitInfoFromLayer(controlLayer,out hitsInfo))
        {
            
            var character = hitsInfo.First().collider.GetComponent<CharacterBrain>();
            mouseRayHitPos=character.transform.position;
        }

        else if (mouseRaycast.GetHitInfoFromLayer(stageLayer,out hitsInfo))
        {
            mouseRayHitPos=hitsInfo.First().point;
        }

        
    }

    void SelectCharacter()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            if (mouseRaycast.IsHit==false)
            {
                selectedCharacter.Value=null;
                return;
            }

            if(mouseRaycast.GetHitInfoFromLayer(controlLayer,out var hitCharacters)==false)
            {
                selectedCharacter.Value=null;
                return;
            }
            
            var character = hitCharacters.First().collider.GetComponent<CharacterBrain>();
            if (character && character.MainObjectData.GroupID == groupID)
            {
                selectedCharacter.Value=character;
            }


            
        }
    }

    void MoveCharacter()
    {
        if (selectedCharacter==null) return;
        if (actives.CanControl == false) return;
        if (GameManager.Instance.TurnManager.ActiveGroupID != groupID) { return; }
        if (GameManager.Instance.InputManager.Game["CamRotButton"].IsPressed()) { return; }
        if (Input.GetMouseButtonDown(1))
        {
            

            if (mouseRaycast.IsHit == false) { return; }

            
            var hitNear=mouseRaycast.HitsInfo.First();
            
            if (MathEx.ContainsLayerInMask(hitNear.collider.gameObject.layer,controlLayer))
            {
                var character = hitNear.collider.GetComponent<CharacterBrain>();
                if (character.MainObjectData.IsEnemies(groupID))
                {

                    selectedCharacter.Value.AIInputProvider.Target.Value = character;
                }
            }
            
            else if (MathEx.ContainsLayerInMask(hitNear.collider.gameObject.layer,stageLayer))
            {
                
                selectedCharacter.Value.AIInputProvider.SetDestination(hitNear.point);
            }
            
        }

    }

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

    void UpdateDestPosGuide()
    {
        routeGuideRenderer.positionCount = 0;

        if(selectedCharacter.Value==null || mouseRaycast.IsHit==false)
        {
            if(destPosGuide.enabled)
            {
                destPosGuide.enabled=false;
            }
            return;
        }

        

        if(destPosGuide.enabled==false)
        {
            destPosGuide.enabled=true;
        }
        
        

        Vector3 destPos=mouseRayHitPos;
        List<Vector3> corners=new();
        bool isArrive=selectedCharacter.Value
        .AIInputProvider.CalcRouteFromRange(ref destPos,ref corners,out float totalDistance);
        destPosGuide.transform.position=destPos;
        //経路描画
        //corners.Reverse();
        corners.Select(i=>i+=new Vector3(0f,0.01f,0f));//ラインの埋没を回避

        routeGuideRenderer.positionCount = corners.Count;
        routeGuideRenderer.SetPositions(corners.ToArray());


        var target=mouseRaycast.HitInfoNear;

        //敵に照準が合っているか
        if(MathEx.ContainsLayerInMask(target.collider.gameObject.layer, controlLayer))
        {
            var character=target.collider.GetComponent<CharacterBrain>();
            destPosGuide.ChangeFlg.Value=character.MainObjectData.IsEnemies(GroupID);
        }
        else
        {
            destPosGuide.ChangeFlg.Value=false;
        }
    }

    //===================================================
    //Gizmo
    //===================================================
    private void OnDrawGizmos()
    {
        if (selectedCharacter.Value==null) { return; }

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


        Color color = Color.blue;
        bool isActionCharacters = GameManager.Instance.TurnManager.IsActionCharacter(selectedCharacter.Value);
        if (isActionCharacters == false)
        {
            color = Color.red;
        }
        else if (actives.CanControl == false && actives.ActiveControls.Contains(selectedCharacter.Value) == false)
        {
            color = Color.red;
        }
        selectedCharacter.Value.DrawGizmosCalceCorners(destPos, color);
        

    }


}
