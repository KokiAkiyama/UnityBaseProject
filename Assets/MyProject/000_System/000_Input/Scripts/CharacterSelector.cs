using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField]
    private List<CharacterBrain> selectedList;

    [SerializeField]
    LayerMask selectLayer;
    [SerializeField]
    LayerMask stageLayer;
    // Start is called before the first frame update
    void Start()
    {
        
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
                if(character && selectedList.Contains(character)==false)
                {
                    selectedList.Add(character);
                }


            }
        }
    }

    void MoveCharacter()
    {
        if(selectedList.Count<=0)return;
        if(GameManager.Instance.InputManager.Game["CamRotButton"].WasPerformedThisFrame()){return;}
        if (Input.GetMouseButtonDown(1))
        {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool isHit = Physics.Raycast(
                 ray: ray,
                 hitInfo: out RaycastHit hit,
                 maxDistance: Mathf.Infinity,
                 layerMask: stageLayer
             );

            if (isHit)
            {
                foreach(var character in selectedList)
                {
                    character.AIInputProvider.SetDestination(hit.point);
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
            SetLayer(transform.gameObject, stageLayer);
        }
    }
}
