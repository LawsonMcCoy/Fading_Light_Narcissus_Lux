using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : CompositeNode
{
    private int current;
    protected override void OnStart()
    {
        current = 0;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Node child = children[current];

        switch (child.Update())
        {
            case State.RUNNING:
                return State.RUNNING;
            case State.FAILURE:
                current++;
                break;
            case State.SUCCESS:
                current = 0;
                return State.SUCCESS;
        }

        return current == children.Count ? State.FAILURE : State.SUCCESS;
    }
}
