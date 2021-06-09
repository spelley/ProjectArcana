using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;

[ExecuteInEditMode]
public class MapBuilder : MonoBehaviour
{
    static MapBuilder _instance;
    public static MapBuilder Instance 
    { 
        get 
        { 
            return _instance; 
        }
    }

    [Header("Data Store")]
    [SerializeField]
    MapData mapData;

    [Header("Grid Settings")]
    [Tooltip("Grid anchor and offset are set using real-world units")]
    [SerializeField]
    Vector3Int gridAnchor = new Vector3Int(0, 0, 0);

    [SerializeField]
    Vector3Int gridOffset = new Vector3Int(-12, 0, -12);

    [Tooltip("Grid Size is X = row, Y = column and Z = height")]
    [SerializeField]
    Vector3Int gridSize = new Vector3Int(24, 24, 12);

    [Range(1, 10)]
    [SerializeField]
    int cellSize = 2;

    [Header("Tile Settings")]
    [SerializeField]
    GameObject tilePrefab;

    [SerializeField]
    float tileSize = 1.95f;

    [SerializeField]
    float raycastDistance = 20f;

    GridCell[,,] grid;

    void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } 
        else
        {
            _instance = this;
            if(Application.isPlaying)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }

    public void BuildMap()
    {
        ResetGrid();
        BuildGrid(true); // when using the manual build, let's override heights and see if it matches up
        BuildNeighbours();
        AssignMapData();
        RenderGrid();
    }

    public void ClearMap()
    {
        ClearTiles();
    }

    public void RefreshMapFromData()
    {
        LoadFromMapData();
        RenderGrid();
    }

    void BuildGrid(bool overrideHeights = false)
    {
        int columns = gridSize.x;
        int rows = gridSize.y;
        int heights = gridSize.z;

        float startColumnPos = gridAnchor.x + gridOffset.x;
        float startRowPos = gridAnchor.z + gridOffset.z;
        float startHeightPos = gridAnchor.y + gridOffset.y;
        float endHeightPos = startHeightPos + ((heights - 1) * cellSize);

        int heightIdx = 0;
        float heightPos = startHeightPos + (heightIdx * cellSize);
        for(int rowIdx = 0; rowIdx < rows; rowIdx++)
        {
            float rowPos = startRowPos + (rowIdx * cellSize);
            for(int colIdx = 0; colIdx < columns; colIdx++)
            {
                float colPos = startColumnPos + (colIdx * cellSize);
                RaycastHit[] hits;
                hits = Physics.RaycastAll(new Vector3(colPos, (endHeightPos + 1f), rowPos), -transform.up, raycastDistance).OrderBy(h=>h.distance).ToArray();
                Debug.DrawRay(new Vector3(colPos, (endHeightPos + 1f), rowPos), (-transform.up * raycastDistance), Color.green);
                
                int tileGridHeight = 0;
                bool blockFurther = false;

                for (int i = 0; i < hits.Length; i++) {
                    RaycastHit hit = hits[i];
                    GridPositioner gridPositioner = hit.transform.GetComponent<GridPositioner>();

                    if(gridPositioner != null)
                    {
                        if(!blockFurther && gridPositioner.blockUnderneath)
                        {
                            // prevent tiles from showing up underneath us
                            blockFurther = true;
                        }
                        else if(blockFurther)
                        {

                            // we stop processing because we are blocking everything underneath us
                            // this is helpful for large structures like buildings
                            continue;
                        }
                        
                        Vector3Int gridPosition = new Vector3Int(colIdx, rowIdx, gridPositioner.height);
                        Vector3 realWorldPosition = hit.point;

                        if(overrideHeights && !gridPositioner.preventOverride)
                        {
                            float estimatedOverrideHeight = hit.point.y + gridOffset.y;
                            estimatedOverrideHeight = estimatedOverrideHeight == 0 ? 0 : estimatedOverrideHeight / cellSize;
                            tileGridHeight = Mathf.RoundToInt(estimatedOverrideHeight);
                            gridPositioner.height = tileGridHeight;
                        }
                        else
                        {
                            tileGridHeight = gridPositioner.height;
                        }

                        Vector3 tileNormal = gridPositioner.preventNormalOverride ? gridPositioner.normal : hit.normal;

                        // this is the actual position of the item in world-space, as opposed to grid-space
                        if(grid[colIdx, rowIdx, tileGridHeight] != null && !grid[colIdx, rowIdx, tileGridHeight].walkable && grid[colIdx, rowIdx, tileGridHeight].buildNoNeighbours)
                        {
                            // we have an obstacle blocking this tile, but since the obstacle isn't static, we need to populate our neighbours in case we are ever removed
                            grid[colIdx, rowIdx, tileGridHeight] = new GridCell(
                                new Vector3Int(colIdx, rowIdx, tileGridHeight), 
                                hit.point,
                                tileNormal,
                                new List<Vector3>(), 
                                true,
                                false,
                                false);
                        }
                        else
                        {
                            bool walkable = gridPositioner.isWalkable;
                            // we are obstructed by something already, but build the tile regardless just make sure to set it as it is
                            if(grid[colIdx, rowIdx, tileGridHeight] != null 
                                && !grid[colIdx, rowIdx, tileGridHeight].walkable 
                                && !grid[colIdx, rowIdx, tileGridHeight].buildNoNeighbours)
                            {
                                walkable = false;
                            }
                            // just set it
                            grid[colIdx, rowIdx, tileGridHeight] = new GridCell(
                                new Vector3Int(colIdx, rowIdx, tileGridHeight), 
                                hit.point, 
                                tileNormal,
                                new List<Vector3>(), 
                                walkable,
                                false,
                                true);
                        }

                        if(gridPositioner.obstructionHeight == 0)
                        {
                            gridPositioner.obstructionHeight = 1;
                        }
                        
                        if(gridPositioner.obstructionHeight > 0)
                        {
                            // offset by 1 because we don't take into account the "top" of the obstruction, ie: our current height
                            for(int obIdx = 1; obIdx <= gridPositioner.obstructionHeight; obIdx++)
                            {
                                if(obIdx > 0) {
                                    int obHeight = tileGridHeight - obIdx;
                                    if(obHeight >= 0 && grid[colIdx, rowIdx, obHeight] == null)
                                    {
                                        grid[colIdx, rowIdx, obHeight] = new GridCell(
                                            new Vector3Int(colIdx, rowIdx, obHeight),
                                            Vector3.zero, 
                                            new Vector3(0, 1f, 0), 
                                            new List<Vector3>(), 
                                            false,
                                            false,
                                            false);
                                    }
                                }
                            }
                        }
                    }     
                }
            }
        }
    }

    void BuildNeighbours()
    {
        int columns = gridSize.x;
        int rows = gridSize.y;
        int heights = gridSize.z;

        for(int heightIdx = 0; heightIdx < heights; heightIdx++)
        {
            for(int rowIdx = 0; rowIdx < rows; rowIdx++)
            {
                for(int colIdx = 0; colIdx < columns; colIdx++)
                {
                    CalculateCellNeighbours(colIdx, rowIdx, heightIdx);
                }
            }
        }
    }

    void CalculateCellNeighbours(int colIdx, int rowIdx, int heightIdx)
    {
        // this cell is empty
        if(grid[colIdx, rowIdx, heightIdx] == null)
        {
            return;
        }

        int columns = gridSize.x;
        int rows = gridSize.y;
        int heights = gridSize.z;

        if(colIdx != 0 || rowIdx != 0) // dont run uselessly on the bottom left corner
        {
            for(int neighbourHeightIdx = 0; neighbourHeightIdx < heights; neighbourHeightIdx++)
            {
                // now look to the left
                if(colIdx != 0 && grid[colIdx - 1, rowIdx, neighbourHeightIdx] != null && !grid[colIdx - 1, rowIdx, neighbourHeightIdx].buildNoNeighbours)
                {
                    // add my neighbour to my list
                    grid[colIdx, rowIdx, heightIdx].neighbours.Add(new Vector3(colIdx - 1, rowIdx, neighbourHeightIdx));
                    // add myself to my neighbour
                    grid[colIdx - 1, rowIdx, neighbourHeightIdx].neighbours.Add(new Vector3(colIdx, rowIdx, heightIdx));
                }

                // now look to down a row
                if(rowIdx != 0 && grid[colIdx, rowIdx - 1, neighbourHeightIdx] != null && !grid[colIdx, rowIdx - 1, neighbourHeightIdx].buildNoNeighbours)
                {
                    // add my neighbour to my list
                    grid[colIdx, rowIdx, heightIdx].neighbours.Add(new Vector3(colIdx, rowIdx - 1, neighbourHeightIdx));
                    // add myself to my neighbour
                    grid[colIdx, rowIdx - 1, neighbourHeightIdx].neighbours.Add(new Vector3(colIdx, rowIdx, heightIdx));
                }
            }
        }
    }

    void AssignMapData()
    {
        if(mapData)
        {
            mapData.gridData.Clear();

            int columns = gridSize.x;
            int rows = gridSize.y;
            int heights = gridSize.z;
            for(int heightIdx = 0; heightIdx < heights; heightIdx++)
            {
                for(int rowIdx = 0; rowIdx < rows; rowIdx++)
                {
                    for(int colIdx = 0; colIdx < columns; colIdx++)
                    {
                        if(grid[colIdx, rowIdx, heightIdx] != null)
                        {
                            mapData.gridData.Add(grid[colIdx, rowIdx, heightIdx]);
                        }
                    }
                }
            }

            // set all the data for helping us build the map
            mapData.gridAnchor = gridAnchor;
            mapData.gridSize = gridSize;
            mapData.gridOffset = gridOffset;
            mapData.cellSize = cellSize;
            mapData.tileSize = tileSize;

            // give me an updated timestamp
            mapData.lastUpdated = System.DateTime.Now.ToString();

            EditorUtility.SetDirty(mapData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Assigned");
        }
    }

    void RenderGrid()
    {
        int columns = gridSize.x;
        int rows = gridSize.y;
        int heights = gridSize.z;

        for(int heightIdx = 0; heightIdx < heights; heightIdx++)
        {
            for(int rowIdx = 0; rowIdx < rows; rowIdx++)
            {
                for(int colIdx = 0; colIdx < columns; colIdx++)
                {
                    if(grid[colIdx, rowIdx, heightIdx] != null)
                    {
                        GridCell gridCell = grid[colIdx, rowIdx, heightIdx];
                        RenderCell(gridCell);
                    }
                }
            }
        }
    }

    void RenderCell(GridCell gridCell)
    {
        GameObject tileObj = Instantiate(tilePrefab, gridCell.realWorldPosition, Quaternion.identity, transform);
        
        float tileSizeX = tileSize;
        if(gridCell.realWorldNormal.x != 0f)
        {
            tileSizeX = tileSize / gridCell.realWorldNormal.y;
        }

        float tileSizeZ = tileSize;
        if(gridCell.realWorldNormal.z != 0f)
        {
            tileSizeZ = tileSize / gridCell.realWorldNormal.y;
        }

        tileObj.transform.localScale = new Vector3(tileSizeX, .1f, tileSizeZ);
        tileObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, gridCell.realWorldNormal);
        gridCell.tileObj = tileObj;
        tileObj.GetComponent<Tile>().SetGridCell(gridCell);
    }

    void LoadFromMapData()
    {
        if(mapData)
        {
            // make a copy of the map data so we don't tamper with the original
            MapData mapInstance = Instantiate(mapData);

            // get all the data for helping us build the map
            gridAnchor = mapInstance.gridAnchor;
            gridSize = mapInstance.gridSize;
            gridOffset = mapInstance.gridOffset;
            cellSize = mapInstance.cellSize;
            tileSize = mapInstance.tileSize;
            
            // reset the grid with the new parameters
            ResetGrid();

            foreach(GridCell gridCell in mapInstance.gridData)
            {
                grid[gridCell.position.x, gridCell.position.y, gridCell.position.z] = gridCell;
            }
        }
    }

    void ResetGrid()
    {
        ClearTiles();
        grid = new GridCell[gridSize.x, gridSize.y, gridSize.z];
    }

    void ClearTiles()
    {
        var tempList = transform.Cast<Transform>().ToList();
        foreach(var child in tempList)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}