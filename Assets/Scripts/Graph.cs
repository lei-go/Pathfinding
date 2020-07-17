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
    int m_height;

    public void Init(int[,] mapData)
    {
        m_mapData = mapData;
        m_width = mapData.GetLength(0);
        m_height = mapData.GetLength(1);
        nodes = new Node[m_width, m_height];

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

    }

}
