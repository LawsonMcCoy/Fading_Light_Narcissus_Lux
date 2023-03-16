using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    public List<Node> children = new List<Node>();

    public override Node Clone(BehaviorTree tree)
    {
        CompositeNode node = Instantiate(this);
        node.myTree = tree;

        node.children = children.ConvertAll(c => c.Clone(tree));

        return node;
    }
}
