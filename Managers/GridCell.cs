using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridCell {
    public Vector3Int position;
    public Vector3 realWorldPosition;
    public Vector3 realWorldNormal;
    public bool buildNoNeighbours;
    public List<Vector3> neighbours;
    public bool walkable = false;
    public bool targetable = true;

    // variables for handling 
    public bool processed;
    public bool visited;
    public GridCell parent;
    public int distance;

    public UnitData occupiedBy;
    public GameObject tileObj;

    public GridCell(Vector3Int position, 
                    Vector3 realWorldPosition, 
                    Vector3 realWorldNormal, 
                    List<Vector3> newNeighbours, 
                    bool walkable = true, 
                    bool buildNoNeighbours = false,
                    bool targetable = true)
    {
        this.position = position;
        this.realWorldPosition = realWorldPosition;
        this.realWorldNormal = realWorldNormal;
        this.neighbours = new List<Vector3>();
        this.neighbours.AddRange(newNeighbours);
        this.walkable = walkable;
        this.buildNoNeighbours = buildNoNeighbours;
        this.targetable = targetable;

        processed = false;
        visited = false;
        distance = 0;
        tileObj = null;
    }

    public void ResetCalculations()
    {
        processed = false;
        visited = false;
        distance = 0;
        parent = null;
        tileObj = null;
    }
}