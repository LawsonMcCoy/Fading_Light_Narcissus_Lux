using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNode : CompositeNode
{
    int current;

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
                return State.FAILURE;
            case State.SUCCESS:
                current++;
                break;
        }

        return current == children.Count ? State.SUCCESS : State.RUNNING;
    }


}
