using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NavigationAgent : MonoBehaviour {

    //Navigation Variables
    public WaypointGraph graphNodes;
    public List<int> openList = new List<int>();
    public List<int> closedList = new List<int>();
    public List<int> currentPath = new List<int>();
    public List<int> greedyPaintList = new List<int>();
    public int currentPathIndex = 0;
    public int currentNodeIndex = 0;

    public Dictionary<int, int> cameFrom = new Dictionary<int, int>();

    // Use this for initialization
    void Start () {
        //Find waypoint graph
        graphNodes = GameObject.FindGameObjectWithTag("waypoint graph").GetComponent<WaypointGraph>();

        //Initial node index to move to
        currentPath.Add(currentNodeIndex);
    }

    public float Heuristic(int a, int b) {
        return Vector3.Distance(graphNodes.graphNodes[a].transform.position, graphNodes.graphNodes[b].transform.position);
    }

    public int bestOpenListFScore(int start, int goal)
    {
        int bestIndex = 0;
        for(int i = 0; i < openList.Count; i++)
        {
            if((Heuristic(openList[i], start) + Heuristic(openList[i], goal)) < (Heuristic(openList[bestIndex], start) + Heuristic(openList[bestIndex], goal)))
            {
                bestIndex = i;
            }
        }
        int bestNode = openList[bestIndex];
        return bestNode;
    }

    public List<int> ReconstructPath(Dictionary<int, int> CF, int current)
    {
        List<int> finalPath = new List<int>();
        finalPath.Add(current);
        while(CF.ContainsKey(current))
        {
            current = CF[current];
            finalPath.Add(current);
        }
        finalPath.Reverse();
        return finalPath;
    }

    //A-Star Search
    public List<int> AStarSearch(int start, int goal) 
    {
        // Clear everything at start
        openList.Clear();
        closedList.Clear();
        cameFrom.Clear();

        // Begin
        openList.Add(start);
        
        float gScore = 0;
        float fScore = gScore + Heuristic(start, goal);

        while(openList.Count > 0)
        {
            int currentNode = bestOpenListFScore(start, goal);
            if(currentNode == goal)
            {
                return ReconstructPath(cameFrom, currentNode);
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            for(int i = 0; i < graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex.Length; i++)
            {
                int thisNeighbourNode = graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex[i];
                // Ignore if neighbour node is attatched
                if(!closedList.Contains(thisNeighbourNode))
                {
                    // Distance from current to the next node
                    float tentativeGScore = Heuristic(start, currentNode) + Heuristic(currentNode, thisNeighbourNode);

                    // Check to see if in openList or if new GScore is more sensible
                    if(!openList.Contains(thisNeighbourNode) || tentativeGScore < gScore)
                    {
                        openList.Add(thisNeighbourNode);
                    }
                    
                    // Add to dictionary - this neighbour came from this parent
                    if(!cameFrom.ContainsKey(thisNeighbourNode))
                    {
                        cameFrom.Add(thisNeighbourNode, currentNode);
                    }
                    gScore = tentativeGScore;
                    fScore = Heuristic(start, thisNeighbourNode) + Heuristic(thisNeighbourNode, goal);
                }
            }
        }
        return null;
    }

    //Greedy Search
    public List<int> GreedySearch(int currentNode, int goal, List<int> path) {

        //Code here

        return path;
    }
}
