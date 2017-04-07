using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node> {
	
    //used for grid walkable/unwalkable calculation
	public bool walkable; //if the player can walk in the area
	public Vector3 worldPosition; //tracks position
	public int gridX;               // grid's x
	public int gridY;                   //grid's y
	public int movementPenalty;

    //vars used to calculate the A* algorithm 
	public int gCost; 
	public int hCost;
	public Node parent;
	int heapIndex;
	
    //asigning vars
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty) {
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		movementPenalty = _penalty;
	}

    //algorithm calculations 
    //calculating fCost
	public int fCost {
		get {
			return gCost + hCost;
		}
	}
    //give heapindex a value
	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}
    //comparing heap costs
	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
