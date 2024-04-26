using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelector : MonoBehaviour
{
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
               bool isZombie = Physics.Raycast(
                    ray: ray,
                    hitInfo: out RaycastHit hit,
                    maxDistance: Mathf.Infinity,
                    layerMask: layer
                );
                
               
            }
        }
}
