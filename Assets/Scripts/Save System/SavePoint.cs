using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private SaveScrtiptableObject save;
    void Start()
    {
        save.setSpawnPoint(gameObject.GetComponent<Transform>().position);
        //save.setScene(gameObject.scene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
