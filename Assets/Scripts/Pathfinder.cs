using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour
{
    Node m_startNode;
    Node m_goalNode;
    Graph m_graph;
    GraphView m_graphView;

    Queue<Node> m_frontierNodes;
    List<Node> m_exploredNodes;
    List<Node> m_pathNodes;

    public Color startColor = Color.green;
    public Color goalColor = Color.red;
    public Color frontierColor = Color.magenta;
    public Color exploredColor = Color.grey;
    public Color pathColor = Color.cyan;

    public bool isCompleted = false;
    int m_iterations = 0;


    public void Init(Graph graph, GraphView graphView, Node start, Node goal)
    {
        if (graph == null || graphView == null || start == null || goal == null)
        {
            Debug.LogWarning("PATHFINDER Init missing elements");
            return;
        }
        else if (start.nodeType == NodeType.Blocked || goal.nodeType == NodeType.Blocked)
        {
            Debug.LogWarning("PATHFINDER Init start/end nodes blocked");
            return;
        }

        m_graph = graph;
        m_graphView = graphView;
        m_startNode = start;
        m_goalNode = goal;
        SecondaryColoring();

        m_frontierNodes = new Queue<Node>();
        m_frontierNodes.Enqueue(start);
        m_exploredNodes = new List<Node>();
        m_pathNodes = new List<Node>();

        for (int y = 0; y < m_graph.Height; y++)
        {
            for (int x = 0; x < m_graph.Width; x++)
            {
                m_graph.nodes[x, y].Reset();
            }
        }

        isCompleted = false;
        m_iterations = 0;
    }

    void SecondaryColoring()
    {
        SecondaryColoring(m_graphView,m_startNode,m_goalNode);
    }
    private void SecondaryColoring(GraphView graphView, Node start, Node goal)
    {
        if (m_frontierNodes != null)
        {
            graphView.ColorNodes(m_frontierNodes.ToList(), frontierColor);
        }
        if (m_exploredNodes != null)
        {
            graphView.ColorNodes(m_exploredNodes, exploredColor);
        }

        NodeView startNodeView = graphView.nodeViews[start.xIndex, start.yIndex];
        NodeView goalNodeView = graphView.nodeViews[goal.xIndex, goal.yIndex];

        if (startNodeView != null)
        {
            startNodeView.ColorNode(startColor);
        }
        if (goalNodeView != null)
        {
            goalNodeView.ColorNode(goalColor);
        }
    }

    public IEnumerator SearchRoutine(float timeStep = 0.1f)
    {
        yield return null;
        while (!isCompleted)
        {
            if (m_frontierNodes.Count > 0)
            {
                Node currentNode = m_frontierNodes.Dequeue();
                m_iterations++;

                if (!m_exploredNodes.Contains(currentNode))
                {
                    m_exploredNodes.Add(currentNode);
                }

                ExpandFrontier(currentNode);
                SecondaryColoring();

                yield return new WaitForSeconds(timeStep);
            }
            else
            {
                isCompleted = true;
            }
        }
    }

    void ExpandFrontier(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i])
                    && !m_frontierNodes.Contains(node.neighbors[i]))
                {
                    node.neighbors[i].previous = node;
                    m_frontierNodes.Enqueue(node.neighbors[i]);
                }
            }
        }
    }







}
