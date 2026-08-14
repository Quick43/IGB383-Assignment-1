using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NavigationAgent {

    //Player Reference
    Player player;

    //Movement Variables
    public float moveSpeed = 10.0f;
    public float minDistance = 0.1f;

    //FSM Variables
    public int newState = 0;
    public int currentState = 0;
    private static int[] hideIndeces = { 3, 11, 44, 51, 54 };
    private int hideIndex = 0;

    // Enemy Type {0 = Chaser, 1 = Stalker, 2 = Shambler, 3 = Fleer}
    public int EnemyType = 0;

    // DFA Specification Tables
    private int[,] dfaTableChaser = new int[3, 4] 
    { 
        { 1, 0, 1, 2 }, 
        { 1, 0, 1, 2 }, 
        { 1, 0, 1, 2 } 
    };

    private int[,] dfaTableStalker = new int[3, 4] 
    { 
        { 1, 0, 1, 2 }, 
        { 1, 1, 1, 2 }, 
        { 1, 1, 1, 2 } 
    };

    private int[,] dfaTableShambler = new int[3, 4] 
    { 
        { 1, 0, 0, 2 }, 
        { 0, 0, 0, 2 }, 
        { 1, 0, 0, 2 } 
    };

    private int[,] dfaTableFleer = new int[3, 4] 
    { 
        { 1, 0, 1, 2 }, 
        { 0, 1, 1, 1 }, 
        { 1, 1, 1, 2 } 
    };

    private int[,] dfaTable;

    // Use this for initialization
    void Start() {
        hideIndex = hideIndeces[Random.Range(0, hideIndeces.Length)];
        if(EnemyType == 0)
        {
            dfaTable = dfaTableChaser;
        }
        else if(EnemyType == 1)
        {
            dfaTable = dfaTableStalker;
        }
        else if(EnemyType == 2)
        {
            dfaTable = dfaTableShambler;
        }
        else if(EnemyType == 3)
        {
            dfaTable = dfaTableFleer;
        }
        //Find waypoint graph
        graphNodes = GameObject.FindGameObjectWithTag("waypoint graph").GetComponent<WaypointGraph>();
        //Initial node index to move to
        currentPath.Add(currentNodeIndex);
        //Establish reference to player game object
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
    }

    // Update is called once per frame
    void Update () 
    {
        if (newState != currentState) 
        {
            if(dfaTable[currentState, 0] == 1)
            {
                hideIndex = hideIndeces[Random.Range(0, hideIndeces.Length)];
                currentState = dfaTable[currentState, newState+1];
            }
        }
        switch (currentState) 
        {
            case 0:
                Roam();
                break;
            case 1:
                Hide();
                break;
            case 2:
                Attack();
                break;
        }   
        Move();
    }

    //Move Enemy
    private void Move() {

        if (currentPath.Count > 0) {

            //Move towards next node in path
            transform.position = Vector3.MoveTowards(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position, moveSpeed * Time.deltaTime);

            //Increase path index
            if (Vector3.Distance(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position) <= minDistance) {

                if (currentPathIndex < currentPath.Count - 1)
                    currentPathIndex++;
            }

            currentNodeIndex = graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNodes>().index;   //Store current node index
        }
    }

    //FSM Behaviour - Roam - Randomly select nodes to travel to using Greedy Search Algorithm
    private void Roam() {
        if(Vector3.Distance(transform.position, graphNodes.graphNodes[currentPath[currentPath.Count-1]].transform.position) <= minDistance) 
        {
            // Randomly select new waypoint
            int randomNode = Random.Range(0, graphNodes.graphNodes.Length);
            // Reset the current path and add first node - needs to be done here because of recursive function of greedy
            currentPath.Clear();
            greedyPaintList.Clear();
            currentPathIndex = 0;
            currentPath.Add(currentNodeIndex);

            // Greedy Search - navigate towards the random node
            currentPath = GreedySearch(currentPath[currentPathIndex], randomNode, currentPath);

            // reverse the path so that the first node is the current node and the last node is the random node
            currentPath.Reverse();

            // remove the first node from the path since it is the current node and we don't want to move to it
            currentPath.RemoveAt(currentPath.Count - 1);
        }
    }
    
    //FSM Behaviour - Move towards hide location using A* Search Algorithm
    private void Hide() {
        currentPath = AStarSearch(currentPath[currentPathIndex], hideIndex);
        currentPathIndex = 0;
    }

    //FSM Behaviour - Move towards node closest to player using A* Search Algorithm
    private void Attack() {
        // Calculate path towards the node nearest the player
        if (Vector3.Distance(transform.position, graphNodes.graphNodes[player.currentNodeIndex].transform.position) > minDistance && currentPath[currentPath.Count - 1] != player.currentNodeIndex) 
        {
            // A* Search - navigate towards the player
            currentPath = AStarSearch(currentPath[currentPathIndex], player.currentNodeIndex);
            currentPathIndex = 0;
        }
    }
}
