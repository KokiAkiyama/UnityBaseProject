using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    [SerializeField]
    float _time=1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _time-=Time.deltaTime;
        if(_time<=0f)
        {
            Destroy(gameObject);
        }
    }
}
