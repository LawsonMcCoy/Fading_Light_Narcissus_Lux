using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecoratorNode : Node
{
    [HideInInspector] public Node child;

    public override Node Clone(BehaviorTree tree)
    {
        DecoratorNode node = Instantiate(this);
        node.myTree = tree;

        node.child = child.Clone(tree);
        return node;
    }
}
