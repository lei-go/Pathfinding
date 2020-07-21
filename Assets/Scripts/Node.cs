using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum NodeType //enum is like creating a variable type by ourselves
{
    Open = 0,
    Blocked = 1
}

public class Node //we dont need to inherit from the monobehavior
{
    //need to define a few features

    public NodeType nodeType = NodeType.Open;

    public int xIndex = -1; //-1 value for flag
    public int yIndex = -1; 

    public Vector3 position; //position of the node
    public float distanceTraveled = Mathf.Infinity; //for the more advanced method

    public List<Node> neighbors = new List<Node>(); // a list of neighboring nodes
    public Node previous = null; //an indicator in the final computed path that tells algo how to get to the current node

    public Node(int xIndex, int yIndex, NodeType nodeType)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        this.nodeType = nodeType;
    }

    public void Reset()
    {
        previous = null;
    }
}
