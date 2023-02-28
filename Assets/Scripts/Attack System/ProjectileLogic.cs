using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]


public class ProjectileLogic : MonoBehaviour
{
    // Start is called before the first frame update
    public UnityEvent<GameObject> hit;
    private LayerMask mask;
    void Start()
    {
    }
    private void Awake()
    {
        gameObject.GetComponent<SphereCollider>().isTrigger = true;
        hit = new UnityEvent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //had to get string name of the layer mask
        if (((1 << other.gameObject.layer) & mask) != 0)
        {
            hit.Invoke(other.gameObject);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void setLayerMask(LayerMask mask)
    {
        this.mask = mask;
    }
}
