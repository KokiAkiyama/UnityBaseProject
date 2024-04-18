using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField]
    private List<CharacterBrain> m_selectedList;

    LayerMask layer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SelectCharacter();
    }

    private void SelectCharacter()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool isHit = Physics.Raycast(
                 ray: ray,
                 hitInfo: out RaycastHit hit,
                 maxDistance: Mathf.Infinity,
                 layerMask: layer
             );

            if (isHit)
            {



            }
        }
    }


    //�����̃Q�[���I�u�W�F�N�g�Ƃ��̎q�I�u�W�F�N�g�̃��C���[�ݒ�
    private void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform transform in go.transform)
        {
            if (transform.gameObject.name == "Other") { continue; }
            SetLayer(transform.gameObject, layer);
        }
    }
}
