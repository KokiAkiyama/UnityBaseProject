using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UniRx.Triggers;
using UniRx;

public class AIEyeSight : MonoBehaviour
{
    [SerializeField] float _radius = 3f;
    public List<MainObjectData> Founds { get; } = new();
    public MainObjectData Get()
    {
        if (Founds.Count == 0) return null;
        return Founds[0];
    }
    
    // Start is called before the first frame update
    void Start()
    {
        SearchUpdateAsync();
    }

    async void SearchUpdateAsync()
    {
        var cancelToken=this.GetCancellationTokenOnDestroy();
        int layer = LayerMask.GetMask("SearchTarget");
        var MyMainObject=GetComponentInParent<MainObjectData>();
        while(cancelToken.IsCancellationRequested==false)
        {
            if(isActiveAndEnabled==false)
            {
                await UniTask.Delay(200);
                continue;
            }
            //��������
            Founds.Clear();
            Collider[] colliders=Physics.OverlapSphere(transform.position, _radius, layer);
            
            foreach(var collider in colliders)
            {
                //Rigidbody������H
                if (collider.attachedRigidbody == null) continue;
                //MainObjectData�������Ă�H
                var mainObject=collider.attachedRigidbody.GetComponentInParent<MainObjectData>();
                if (mainObject == null) continue;

                //�������g�͖���
                if(MyMainObject == mainObject) continue;

                //�ǉ�
                Founds.Add(mainObject);
            }

            Debug.Log(Founds.Count);
            await UniTask.Delay(200);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position,_radius); 
    }
}