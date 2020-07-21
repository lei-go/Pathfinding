using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Graph))]
public class GraphView : MonoBehaviour
{
    public GameObject nodeViewPrefab;
    public NodeView[,] nodeViews;
    

    //versus the single ColorNode in the NodeView class, this one colors a bunch nodes at the same time
    public void ColorNodes(List<Node> nodes, Color color, bool lerpColor = false, float lerpValue = 0.5f)
    {
        foreach (Node n in nodes)
        {
            if (n != null)
            {
                NodeView nodeView = nodeViews[n.xIndex,n.yIndex];
                Color newColor = color;

                if (lerpColor)
                {
                    Color originalColor = MapData.GetColorFromNodeType(n.nodeType);
                    newColor = Color.Lerp(originalColor, newColor, lerpValue);

                }

                if (nodeView != null)
                {
                    nodeView.ColorNode(newColor);
                }
            }
        }
    }

    public void Init(Graph graph)
    {
        if (graph == null)
        {
            Debug.LogWarning("GRAPHVIEW no graph initialized");
            return;
        }

        //otherwise
        nodeViews = new NodeView[graph.Width, graph.Height];

        foreach (Node n in graph.nodes)
        {
            GameObject instance = Instantiate(nodeViewPrefab, Vector3.zero, Quaternion.identity);
            NodeView nodeView = instance.GetComponent<NodeView>();

            if (nodeView != null)
            {
                nodeView.Init(n);
                nodeViews[n.xIndex,n.yIndex] = nodeView;

                Color tileColor = MapData.GetColorFromNodeType(n.nodeType);
                nodeView.ColorNode(tileColor);
            }
        }
    }

    public void ShowNodeArrows(Node node)
    {
        if (node != null)
        {
            NodeView nodeView = nodeViews[node.xIndex,node.yIndex];
            if (nodeView != null) nodeView.ShowArrow();
        }
    }

    public void ShowNodeArrows(List<Node> nodes)
    {
        foreach (Node n in nodes)
        {
            ShowNodeArrows(n);
        }
    }



}
