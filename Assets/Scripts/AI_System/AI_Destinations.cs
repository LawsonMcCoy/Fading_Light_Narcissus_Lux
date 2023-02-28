using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Destinations: MonoBehaviour
{
    //is a singleton that all AI will use to access data;
    /*
    public static AI_Actions Instance
    {
        get;
        private set;
    }
    private void Awake()
    {
        //set the singleton instance
        Instance = this;

        //set the singleton to not destroy on load
        DontDestroyOnLoad(this);

        //make listener for when there is a new scene
        EventManager.Instance.Subscribe(EventTypes.Events.LOAD_SCENE, updateDestinations);
    }
    */
    public enum Dest
    {
        IKA,
        TREE
    }

    //data
    
    [SerializeField]private GameObject Ika;
    [SerializeField]private GameObject tree;

    public void updateDestinations()
    {
        Ika = GameObject.FindGameObjectWithTag("Player");
    }
    public static GameObject getGameObjectFromDestination(Dest destination)
    {
        if (destination == Dest.IKA)
        {
           
            Debug.Log("giving Ika to node");
            return GameObject.FindGameObjectWithTag("Player");
            
        }
        return null;
    }
    // Start is called before the first frame update
    void Start()
    {
        updateDestinations();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe(EventTypes.Events.LOAD_SCENE, updateDestinations);
        Instance = null;

    }
    */
}
