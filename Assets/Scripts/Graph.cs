using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    //this might seem redundant at the moment
    //of having a nodes array and a map 2d array
    //but it allows us more flexibility as seen in the future
    public Node[,] nodes;
    public List<Node> walls = new List<Node>();

    //cache the could-be useful data. reduncdant but ok.
    int[,] m_mapData;
    int m_width;
    public int Width { get {return m_width; } }
    int m_height;
    public int Height { get {return m_height; } } //a read-only public variable
    public static readonly Vector2[] allDirections =
    {
        new Vector2(0f,1f),
        new Vector2(0f,-1f),
        new Vector2(-1f,0f),
        new Vector2(1f,0f),
        
        new Vector2(-1f,1f),
        new Vector2(1f,1f),
        new Vector2(-1f,-1f),
        new Vector2(1f,-1f)
        
    };

    public void Init(int[,] mapData)
    {
        m_mapData = mapData;
        m_width = mapData.GetLength(0);
        m_height = mapData.GetLength(1);
        nodes = new Node[m_width, m_height];

        //first populate the node array
        for (int y = 0; y < m_height; y++)
        {
            for (int x = 0; x < m_width; x++)
            {
                NodeType type = (NodeType) mapData[x,y]; //casting 1,0 to open and blocked
                Node newNode = new Node(x,y,type);
                nodes[x,y] = newNode;

                newNode.position = new Vector3(x, 0, y);

                if (type == NodeType.Blocked)
                {
                    walls.Add(newNode);
                }
            }
        }

        //then find the neighbors
        for (int y = 0; y < m_height; y++)
        {
            for (int x = 0; x < m_width; x++)
            {
                if (nodes[x,y].nodeType != NodeType.Blocked)
                {
                    nodes[x,y].neighbors = GetNeighbors(x,y);
                }
            }
        }

    }

    public bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < m_width && y >= 0 && y < m_height);
    }

    List<Node> GetNeighbors(int x, int y, Node[,] nodeArray, Vector2[] directions)
    {
        List<Node> neighborNodes = new List<Node>();

        foreach (Vector2 dir in directions)
        {
            int newX = x + (int) dir.x;
            int newY = y + (int) dir.y;
            
            //when new calculated coordinate is within bounds and not null, and it is not of type wall/blocked, we say it is a valid neighbor 
            if (IsWithinBounds(newX, newY) && nodeArray[newX,newY] != null && nodeArray[newX,newY].nodeType != NodeType.Blocked)
            {
                neighborNodes.Add(nodeArray[newX,newY]);
            }  
        }

        return neighborNodes;
    }

    //to simply our usage of the Getneighbor functions, make an overloaded version taking only x,y as inputs
    List<Node> GetNeighbors(int x, int y)
    {
        return GetNeighbors(x, y, nodes, allDirections);
    }

    //considered all 8 DOFs
    public float GetNodeDistance(Node source, Node target)
    {
        int dx = Mathf.Abs(target.xIndex - source.xIndex);
        int dy = Mathf.Abs(target.yIndex - source.yIndex);
        
        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);

        int diagonalSteps = min;
        int straightSteps = max - min;
        
        float distance = 1.4f * diagonalSteps + straightSteps;
        return distance;
    }
}
