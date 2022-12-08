// Written by Kevin Chao

using System.Collections.Generic;
using UnityEngine;
using System;


// Singleton
public class EventManager : MonoBehaviour
{
    private static Dictionary<EventTypes.Events, Action> subscriberDict;

    public static EventManager Instance
    {
        get;
        private set;
    }
    public int ID;



    private void Awake()
    {      
        Instance = this;
        subscriberDict = new Dictionary<EventTypes.Events, Action>();

        DontDestroyOnLoad(gameObject);

        ID = gameObject.GetInstanceID();
    }


    private void OnDestroy()
    {
        //Instance = null;
    }


    public void Subscribe(EventTypes.Events eventType, Action listener)
    {
        Action existingListeners;
        
        // If eventType has listeners already in subscriberDict
        if (subscriberDict.TryGetValue(eventType, out existingListeners))
        {
            // Add new listener to existingListeners
            existingListeners += listener;

            // Update subscriberDict
            subscriberDict[eventType] = existingListeners;
        }

        // If eventType has no listeners in subscriberDict
        else
        {
            // Add event to subscriberDict
            existingListeners += listener;
            subscriberDict.Add(eventType, existingListeners);
        }
    }


    public void Unsubscribe(EventTypes.Events eventType, Action listener)
    {
        // If EventManager is already destroyed, no reason to unsubscribe
        if (Instance == null) return;

        Action existingListeners;

        // If eventType has listeners already in subscriberDict
        if (subscriberDict.TryGetValue(eventType, out existingListeners))
        {
            // Remove listener from existingListeners
            existingListeners -= listener;

            // Update subscriberDict
            subscriberDict[eventType] = existingListeners;
        }
    }


    public void Notify(EventTypes.Events eventType)
    {
        // If eventType is in the subscriberDict, invoke all listeners of that eventType
        Action existingListeners = null;
        if (subscriberDict.TryGetValue(eventType, out existingListeners))
            existingListeners.Invoke();
    }
}