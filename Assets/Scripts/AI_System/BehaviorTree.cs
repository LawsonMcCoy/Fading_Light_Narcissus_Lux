using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class BehaviorTree : ScriptableObject
{
    public Node rootNode;
    public Node.State treeState = Node.State.RUNNING;

    public List<Node> nodes = new List<Node>();
    private GameObject thisAI;
    public Node.State Update()
    {
        if(rootNode.state == Node.State.RUNNING)
        {
            treeState = rootNode.Update();
        }

        return treeState;
    }

    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;

        #if (UNITY_EDITOR)
        node.guid = GUID.Generate().ToString();
        #endif
        node.myTree = this;

        nodes.Add(node);
        #if (UNITY_EDITOR)
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        #endif

        return node;
    }

    public void DeleteNode(Node node)
    {
        nodes.Remove(node);
        #if (UNITY_EDITOR)
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
        #endif
    }

    public void AddChild(Node parent, Node child)
    {
        DecoratorNode dec = parent as DecoratorNode;
        if (dec)
        {
            dec.child = child;
        }
        
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            rootNode.child = child;
        }

        CompositeNode comp = parent as CompositeNode;
        if (comp)
        {
            comp.children.Add(child);
        }

    }
    public void RemoveChild(Node parent, Node child)
    {
        DecoratorNode dec = parent as DecoratorNode;
        if (dec)
        {
            dec.child = null;
        }

        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            rootNode.child = null;
        }

        CompositeNode comp = parent as CompositeNode;
        if (comp)
        {
            comp.children.Remove(child);
        }
    }
    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();

        DecoratorNode dec = parent as DecoratorNode;
        if (dec && dec.child != null)
        {
            children.Add(dec.child);
        }

        RootNode rootNode = parent as RootNode;
        if (rootNode && rootNode.child != null)
        {
            children.Add(rootNode.child);
        }

        CompositeNode comp = parent as CompositeNode;
        if (comp)
        {
            return comp.children;
        }
        return children;
    }

    public BehaviorTree Clone()
    {
        BehaviorTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone();
        return tree;
    }
    public void setAI(GameObject ai)
    {
        thisAI = ai;
    }
    public GameObject getAI()
    {
        return thisAI;
    }
    
}
