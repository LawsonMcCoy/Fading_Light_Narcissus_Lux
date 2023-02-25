using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNode : Node
{
    public Node child;
    //public bool loaded = false;
    protected override void OnStart()
    {
        //EventManager.Instance.Subscribe(EventTypes.Events.LOAD_SCENE, finishedLoading);
        //started = true;
        Debug.Log("root is starting");
    }
    public void finishedLoading()
    {
        Debug.Log("finishedLoading was called");
        //loaded = true;
    }
    protected override void OnStop()
    {
        //EventManager.Instance.Unsubscribe(EventTypes.Events.LOAD_SCENE, finishedLoading);
        Debug.Log("root is stopped");
    }

    protected override State OnUpdate()
    {

        //if (loaded) //waits for scene to load before beginning behaviors
        //{
        return child.Update();
        //}
        //return Node.State.RUNNING;
    }

    public override Node Clone()
    {
        RootNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}
