using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum State
    {
        RUNNING,
        FAILURE,
        SUCCESS
    }

    //public variables
    [HideInInspector] public State state = State.RUNNING;
    public bool started = false;

    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    public string NodeName = "";
    [HideInInspector] public BehaviorTree myTree;
    public State Update()
    {
        if (!started)
        {
            OnStart();
            started = true;
        }
        state = OnUpdate();

        if (state == State.FAILURE || state == State.SUCCESS)
        {
            OnStop();
            started = false;
        }
        return state;
    }
    public virtual Node Clone(BehaviorTree tree)
    {
        Node node = Instantiate(this);
        node.myTree = tree;
        return node;
    }
    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();
}
