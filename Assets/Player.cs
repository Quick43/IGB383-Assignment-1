using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : NavigationAgent {

    //Movement Variables
    public float moveSpeed = 10.0f;
	public float minDistance = 0.1f;

	//Mouse Clicking
	private Vector3 mousePosition;
	
	// Update is called once per frame
	void Update () {

		//Right-click - Move via Greedy
		if (Input.GetMouseButtonDown (0)) {

            //Reset current path and add first node - needs to be done here because of recursive function of greedy
            currentPath.Clear();
            greedyPaintList.Clear();
            currentPathIndex = 0;
            currentPath.Add(currentNodeIndex);

            //Greedy Search
            //currentPath = GreedySearch (currentPath [currentPathIndex], findClosestWaypoint(), currentPath);

            //Reverse path and remove final (i.e. initial) position
            currentPath.Reverse ();
			currentPath.RemoveAt (currentPath.Count-1);
		}

		//Left-click - Move via A*
		else if (Input.GetMouseButtonDown (1)) {

            //currentPath = AStarSearch (currentPath[currentPathIndex], findClosestWaypoint());
			currentPathIndex = 0;
		}
	
		//Move player
		if (currentPath.Count > 0) {

			transform.position = Vector3.MoveTowards (transform.position, graphNodes.graphNodes [currentPath [currentPathIndex]].transform.position, moveSpeed * Time.deltaTime);
		
			//Increase path index
			if (Vector3.Distance (transform.position, graphNodes.graphNodes [currentPath [currentPathIndex]].transform.position) <= minDistance) {
				
				if (currentPathIndex < currentPath.Count - 1)
					currentPathIndex ++;
            }

            currentNodeIndex = graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNodes>().index;   //Store current node index
        }
    }


	//Find the waypoint that is the closest to where we have clicked the mouse
	private int findClosestWaypoint(){

		//Get mouse coordinates to world position
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit)){
			mousePosition = hit.point;
		}

		Debug.DrawLine (Camera.main.transform.position, mousePosition, Color.green);

		float distance = 1000.0f;
		int closestWaypoint = 0;

		//Find the waypoint closest to this position
		for (int i = 0; i < graphNodes.graphNodes.Length; i++) {
			if (Vector3.Distance (mousePosition, graphNodes.graphNodes[i].transform.position) <= distance){
				distance = Vector3.Distance (mousePosition, graphNodes.graphNodes[i].transform.position);
				closestWaypoint = i;
			}
		}

		print ("Closest Waypoint: " + closestWaypoint);
		
		return closestWaypoint;
	}
}


