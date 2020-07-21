using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Pathfinder : MonoBehaviour
{
    public enum Mode
    {
        BFS = 0,
        Dijkstra = 1
    }

    public Mode mode = Mode.BFS;
    Node m_startNode;
    Node m_goalNode;
    Graph m_graph;
    GraphView m_graphView;

    PriorityQueue<Node> m_frontierNodes;
    List<Node> m_exploredNodes;
    List<Node> m_pathNodes;

    public Color startColor = Color.green;
    public Color goalColor = Color.red;
    public Color frontierColor = Color.magenta;
    public Color exploredColor = Color.grey;
    public Color pathColor = Color.cyan;

    //was gonna make arrows on the path a diff color but nah overcomplicated
    //public Color32 pathArrowColor = new Color32(255,215,0,255); //gold

    bool isCompleted = false;
    public bool showIterations = true;
    public bool showArrows = true;
    public bool showColor = true;
    public bool exitOnGoal = true;
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
        ColorRendering();

        m_frontierNodes = new PriorityQueue<Node>();
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
        m_startNode.distanceTraveled = 0;
    }
    public IEnumerator SearchRoutine(float timeStep = 0.1f)
    {
        float timeStart = Time.time;
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

                if (mode == Mode.BFS)
                {
                    ExpandFrontierBFS(currentNode);
                }
                else if (mode == Mode.Dijkstra)
                {
                    ExpandFrontierDijkstra(currentNode);
                }

                CheckIfGoalReached();

                if (showIterations)
                {
                    AnimateIterations();
                    yield return new WaitForSeconds(timeStep);
                }
            }
            else
            {
                isCompleted = true;
            }
        }

        FinalRendering(); //just to show the final result
        Debug.Log("PATHFINDER: time elapse: " + (Time.time - timeStart).ToString() + " seconds");
        Debug.Log("PATHFINDER: " + "(" + mode.ToString() + ") " + "total distance: " + m_goalNode.distanceTraveled.ToString());
    }

    private void CheckIfGoalReached()
    {
        if (m_frontierNodes.Contains(m_goalNode))
        {
            m_pathNodes = GetPathNodes(m_goalNode);
            if (exitOnGoal) isCompleted = true;
        }
    }

    private void FinalRendering()
    {
        ColorPathNodes();
        ColorStartAndGoalNodes();
        if (showArrows)
        {
            m_graphView.ShowNodeArrows(m_pathNodes.ToList());
        }   
    }

    private void AnimateIterations()
    {
            if (showColor)
            {
                ColorRendering();
            }
            if (showArrows)
            {
                m_graphView.ShowNodeArrows(m_frontierNodes.ToList());
            }   
    }

    void ColorRendering()
    {
        ColorRendering(m_graphView,m_startNode,m_goalNode);
    }
    private void ColorRendering(GraphView graphView, Node start, Node goal)
    {
        //color frontier first, then path, then start and goal
        //do not change sequence
        ColorFrontAndExpNodes();
        ColorPathNodes();
        ColorStartAndGoalNodes();
    }

    private void ColorPathNodes()
    {
        if (m_pathNodes != null && m_pathNodes.Count > 0)
        {m_graphView.ColorNodes(m_pathNodes,pathColor);}
    }

    private void ColorStartAndGoalNodes()
    {
        NodeView startNodeView = m_graphView.nodeViews[m_startNode.xIndex, m_startNode.yIndex];
        NodeView goalNodeView = m_graphView.nodeViews[m_goalNode.xIndex, m_goalNode.yIndex];

        if (startNodeView != null)
        {
            startNodeView.ColorNode(startColor);
        }
        if (goalNodeView != null)
        {
            goalNodeView.ColorNode(goalColor);
        }
    }

    private void ColorFrontAndExpNodes()
    {
        if (m_frontierNodes != null)
        {
            m_graphView.ColorNodes(m_frontierNodes.ToList(), frontierColor);
        }
        if (m_exploredNodes != null)
        {
            m_graphView.ColorNodes(m_exploredNodes, exploredColor);
        }
    }

    void ExpandFrontierBFS(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i])
                    && !m_frontierNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    node.neighbors[i].distanceTraveled = distanceToNeighbor + node.distanceTraveled;
                    
                    node.neighbors[i].previous = node;

                    //bc the new priority queue, the old BFS will behave abnormally since no distance used and the priority is messed up
                    //to make it behave like the original BFS, it is necessary to assign fake priority to make the first explored ones always go first
                    node.neighbors[i].priority = m_exploredNodes.Count; 
                    
                    m_frontierNodes.Enqueue(node.neighbors[i]);
                }
            }
        }
    }

    void ExpandFrontierDijkstra(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbor + node.distanceTraveled;
                    
                    //only when the new distance is less then we assigned it to the prev. Make sure it would always be the shortest route.
                    if (newDistanceTraveled < node.neighbors[i].distanceTraveled) 
                    {
                        node.neighbors[i].previous = node;
                        node.neighbors[i].distanceTraveled = newDistanceTraveled;
                    }
                    
                    if (!m_frontierNodes.Contains(node.neighbors[i]))
                    {
                        node.neighbors[i].priority = (int) node.neighbors[i].distanceTraveled;
                        m_frontierNodes.Enqueue(node.neighbors[i]);
                    }
                    
                }
            }
        }
    }
    List<Node> GetPathNodes(Node endNode)
    {
        List<Node> path = new List<Node>();

        if (endNode != null)
        {
            path.Add(endNode);
            Node currentNode = endNode.previous;

            while (currentNode != null)
            {
                path.Insert(0, currentNode);
                currentNode = currentNode.previous;
            }
        }
        return path;
    }





}
