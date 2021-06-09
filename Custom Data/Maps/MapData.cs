using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Custom Data/Map Data", order = 1)]
public class MapData : ScriptableObject
{
    public string mapName;
    public string description;
    public string lastUpdated;

    public Vector3Int gridAnchor = new Vector3Int(0, 0, 0);
    public Vector3Int gridSize = new Vector3Int(24, 24, 12);
    public Vector3Int gridOffset = new Vector3Int(-12, -12, 0);

    public int cellSize = 2;
    public float tileSize = 1.95f;

    public List<GridCell> gridData;
}
