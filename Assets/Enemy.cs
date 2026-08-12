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
    private int currentState = 0;
    private int hideIndex = 25;

    // DFA Specification Table
    private int[,] dfaTable = new int[3, 4] 
    { 
        { 1, 0, 1, 2 }, 
        { 1, 0, 1, 2 }, 
        { 1, 0, 1, 2 } 
    };

    // Use this for initialization
    void Start() {
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
            currentState = newState;
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
        print("roaming");
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
