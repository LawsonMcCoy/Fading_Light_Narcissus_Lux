using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionNode : Node
{
    public override Node Clone(BehaviorTree tree)
    {
        ActionNode node = Instantiate(this);
        node.myTree = tree;
        return node;
    }
}
