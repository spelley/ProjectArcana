using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    #region Initialization
    static MapManager _instance;
    public static MapManager Instance 
    { 
        get 
        { 
            return _instance; 
        }
    }

    BattleManager battleManager;
    #endregion

    #region Private Variables
    Camera cam;
    Ray camRay;
    RaycastHit hit;

    [SerializeField]
    MapData mapData;

    [SerializeField]
    Transform mapHolder;

    [SerializeField]
    GameObject tilePrefab;
    [SerializeField]
    GameObject targetTilePrefab;
    #endregion

    #region Grid Data/Positioning
    public GridCell[,,] grid { get; private set; }
    Vector3Int gridAnchor = new Vector3Int(0, 0, 0);
    Vector3Int gridSize = new Vector3Int(24, 24, 12);
    Vector3Int gridOffset = new Vector3Int(-12, -12, 0);

    [SerializeField]
    int _cellSize = 2;
    public int cellSize
    {
        get
        {
            return _cellSize;
        }
        private set
        {
            _cellSize = value;
        }
    }

    [SerializeField]
    float _tileSize = 2f;
    public float tileSize
    {
        get
        {
            return _tileSize;
        }
        private set
        {
            _tileSize = value;
        }
    }
    #endregion

    #region Cell Lists & Filtering
    List<GridCell> cellList = new List<GridCell>();
    List<GridCell> walkableCells = new List<GridCell>();
    List<GridCell> targetableCells = new List<GridCell>();
    List<GridCell> targetedCells = new List<GridCell>();
    Dictionary<Vector3Int, Stack<GridCell>> cachedPaths = new Dictionary<Vector3Int, Stack<GridCell>>();
    #endregion

    #region Tile Rendering
    public int startPooledSize = 100;
    List<GameObject> pooledTiles;
    Dictionary<GridCell, GameObject> renderedTiles = new Dictionary<GridCell, GameObject>();
    #endregion

    #region Events
    public event Action<Stack<GridCell>> OnTravelPath;
    public event Action OnTravelPathEnd;
    public event Action<UnitData, GridCell> OnTravelEnter;
    #endregion

    void Awake()
    {
        cam = Camera.main;
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } 
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {

        ResetGrid();
        if(mapData)
        {
            LoadFromMapData();
        }

        battleManager = BattleManager.Instance;

        pooledTiles = new List<GameObject>();
        for(int i = 0; i < startPooledSize; i++)
        {
            GameObject tileObj = (GameObject)Instantiate(tilePrefab, mapHolder);
            tileObj.SetActive(false);
            pooledTiles.Add(tileObj);
        }

        BindEvents();
    }
    
    #region Event Bindings
    void BindEvents()
    {
        battleManager.OnEncounterStart += OnEncounterStart;
        battleManager.OnEncounterEnd += OnEncounterEnd;
        battleManager.OnSkillTarget += OnSkillTarget;
        battleManager.OnSkillTargetCancel += OnSkillTargetCancel;
        battleManager.OnSkillSelectTarget += OnSkillSelectTarget;
        battleManager.OnSkillSelectTargetCancel += OnSkillSelectTargetCancel;
        battleManager.OnSkillConfirmPrep += OnSkillConfirmPrep;
    }

    void UnbindEvents()
    {
        battleManager.OnEncounterStart -= OnEncounterStart;
        battleManager.OnEncounterEnd -= OnEncounterEnd;
        if(battleManager.turnManager != null)
        {
            battleManager.turnManager.OnTurnEnd -= OnTurnEnd;
        }
        battleManager.OnSkillTarget -= OnSkillTarget;
        battleManager.OnSkillTargetCancel -= OnSkillTargetCancel;
        battleManager.OnSkillSelectTarget -= OnSkillSelectTarget;
        battleManager.OnSkillSelectTargetCancel -= OnSkillSelectTargetCancel;
        battleManager.OnSkillConfirmPrep -= OnSkillConfirmPrep;
    }
    #endregion

    #region Encounter Events
    public void OnEncounterStart()
    {
        ClearAllTiles();
        ResetWalkableZoneCalculations();
        ResetTargetableCells();
        ResetTargetedCells();

        battleManager.turnManager.OnTurnEnd += OnTurnEnd;
    }

    public void OnTurnEnd(ITurnTaker curTurnTaker)
    {
        ClearAllTiles();
        ResetWalkableZoneCalculations();
        ResetTargetableCells();
        ResetTargetedCells();
    }

    public void OnEncounterEnd()
    {
        ClearAllTiles();
        ResetWalkableZoneCalculations();
        ResetTargetableCells();
        ResetTargetedCells();
        ClearOccupiedBy();
        battleManager.turnManager.OnTurnEnd -= OnTurnEnd;
    }
    #endregion

    #region Skill Targeting Events
    public void OnSkillTarget(SkillData skillData, UnitData userData, GridCell originCell)
    {
        // remove the walkability tiles for now
        ClearWalkabilityTiles();
        // clear out any old targetable cells
        ResetTargetableCells();
        targetableCells.Clear();
        targetableCells.AddRange(GetTargetableCells(skillData, originCell));
        RenderTargetableCells();
    }

    public void OnSkillTargetCancel(SkillData skillData, UnitData unitData)
    {
        ResetTargetableCells();
        ResetTargetedCells();
    }

    public void OnSkillSelectTarget(SkillData skillData, GridCell originCell, GridCell targetCell, bool hideUntargetedCells = false)
    {
        if(skillData == null)
        {
            return;
        }

        ResetTargetedCells();
        targetedCells.AddRange(GetTargetedArea(skillData, originCell, targetCell, targetableCells));
        if(hideUntargetedCells)
        {
            ClearTargetableTiles();
        }
        RenderTargetedCells();
    }

    public List<GridCell> GetTargetedArea(
        SkillData skillData, 
        GridCell originCell, 
        GridCell targetCell, 
        List<GridCell> targetableArea, 
        bool occupiedOnly = false)
    {
        List<GridCell> targetedArea = new List<GridCell>();

        if(skillData == null || !targetableArea.Contains(targetCell))
        {
            return targetedArea;
        }

        List<Vector3Int> offsets = SkillTargetShape.GetTargetShape(skillData, originCell, targetCell, targetableCells);
        foreach(Vector3Int offset in offsets)
        {
            if(CheckIfTargetable(targetCell.position.x + offset.x, targetCell.position.y + offset.y, targetCell.position.z + offset.z))
            {
                GridCell newCell = grid[targetCell.position.x + offset.x, targetCell.position.y + offset.y, targetCell.position.z + offset.z];
                if(!occupiedOnly || newCell.occupiedBy != null) 
                {
                    if(!skillData.requireEmptyTile || newCell.occupiedBy == null)
                    {
                        targetedArea.Add(newCell);
                    }
                }
            }
        }

        return targetedArea;
    }

    public void OnSkillSelectTargetCancel(SkillData skillData, GridCell originCell, GridCell targetCell)
    {
        ResetTargetedCells();
        RenderTargetableCells();
    }

    public void OnSkillConfirmPrep(SkillData skillData, UnitData unitData, List<GridCell> targets)
    {
        ClearAllTiles();
    }

    public void OnSkillClear()
    {
        ResetWalkableZoneCalculations();
        ResetTargetableCells();
        ResetTargetedCells();
    }
    #endregion

    #region Pathfinding
    public void UpdateUnitPosition(Vector3Int position, UnitData unitData)
    {
        grid[unitData.curPosition.x, unitData.curPosition.y, unitData.curPosition.z].occupiedBy = null;
        unitData.curPosition = position;
        grid[position.x, position.y, position.z].occupiedBy = unitData;
    }

    public Vector3Int LoadWalkabilityZone(GameObject player, Vector3 start, bool render = true)
    {
        UnitData unitData = player.GetComponent<CharacterMotor>().unitData;
        bool canMove = unitData.canMove;
        int move = canMove ? unitData.stats.CalculateMove() : 0;
        int jump = canMove ? unitData.stats.CalculateJump() : 0;
        Faction faction = unitData.faction;

        ClearWalkabilityTiles();
        GridCell startingCell = grid[unitData.curPosition.x, unitData.curPosition.y, unitData.curPosition.z]; // GetClosestGridCell(start);
        CalculateWalkableZone(startingCell.position, move, jump, faction);
        if(render)
        {
            RenderWalkableZone();
        }

        return startingCell.position;
    }

    void CalculateWalkableZone(Vector3Int startingGridPosition, int maxDistance, int maxUp, Faction faction)
    {
        ResetWalkableZoneCalculations();
        
        GridCell curCell = grid[startingGridPosition.x, startingGridPosition.y, startingGridPosition.z];

        Queue<GridCell> process = new Queue<GridCell>();
        process.Enqueue(curCell);

        curCell.visited = true;

        while(process.Count > 0)
        {
            GridCell cell = process.Dequeue();
            // you can't walk to yourself
            if(cell.position != startingGridPosition)
            {
                walkableCells.Add(cell);
            }
            
            if(cell.distance < maxDistance && cell.walkable)
            {
                foreach(Vector3 neighbourGridPosition in cell.neighbours)
                {  
                    GridCell neighbourCell = grid[(int)neighbourGridPosition.x, (int)neighbourGridPosition.y, (int)neighbourGridPosition.z];
                    if(neighbourCell != null 
                        && !neighbourCell.visited 
                        && neighbourCell.walkable 
                        && (neighbourCell.occupiedBy == null || neighbourCell.occupiedBy.faction == faction)
                        && IsVerticallyAccessible(cell, neighbourCell))
                    {
                        if(Mathf.Abs(neighbourCell.position.z - cell.position.z) <= maxUp)
                        {
                            neighbourCell.parent = cell;
                            neighbourCell.visited = true;
                            neighbourCell.distance = 1 + neighbourCell.parent.distance;
                            process.Enqueue(neighbourCell);
                        }
                    }
                }
            }
        }
    }

    bool IsVerticallyAccessible(GridCell curCell, GridCell targetCell)
    {
        int curHeight = curCell.position.z;
        int targetHeight = targetCell.position.z;

        if(curHeight == targetHeight)
        {
            return true; // no checks needed
        }
        else if(curHeight < targetHeight) // height is above us, check our current cell to see if we are blocked
        {
            int checkHeight = curHeight;
            while(checkHeight <= targetHeight)
            {
                GridCell checkCell = grid[curCell.position.x, curCell.position.y, checkHeight];
                if(checkCell != null && !checkCell.walkable)
                {
                    return false;
                }
                checkHeight++;
            }
        }
        else // height is below us, check the target column to make sure nothing is in the way
        {
            int checkHeight = curHeight;
            while(checkHeight >= targetHeight)
            {
                GridCell checkCell = grid[targetCell.position.x, targetCell.position.y, checkHeight];
                if(checkCell != null && checkCell != targetCell)
                {
                    return false;
                }
                checkHeight--;
            }
        }
        return true;
    }

    void RenderWalkableZone()
    {
        foreach(GridCell gridCell in walkableCells)
        {
            RenderCell(gridCell, TileType.WALKING_ZONE);
        }
    }

    void ResetWalkableZoneCalculations()
    {
        for(int i = 0; i < cellList.Count; i++)
        {
            cellList[i].ResetCalculations();
        }

        walkableCells.Clear();
        cachedPaths.Clear();
    }

    public List<GridCell> GetForcedMovement(Vector3Int pushOriginPosition, Vector3Int pushTargetPosition, int distance = 1, int maxHeightDiff = 100)
    {
        List<GridCell> forcedPath = new List<GridCell>();
        if(pushOriginPosition == pushTargetPosition || distance == 0)
        {
            forcedPath.Add(GetCell(pushTargetPosition));
            return forcedPath;
            // return GetCell(pushTargetPosition);
        }

        int distMod = distance > 0 ? 1 : -1;

        int xDistance = Mathf.Abs(pushOriginPosition.x - pushTargetPosition.x);
        int yDistance = Mathf.Abs(pushOriginPosition.y - pushTargetPosition.y);

        Vector3Int modVector = Vector3Int.zero;
        if(xDistance > yDistance)
        {
            modVector = (pushOriginPosition.x < pushTargetPosition.x) ? new Vector3Int(distMod, 0, 0) : new Vector3Int(-distMod, 0, 0);
        }
        else if(yDistance > xDistance)
        {
            modVector = (pushOriginPosition.y < pushTargetPosition.y) ? new Vector3Int(0, distMod, 0) : new Vector3Int(0, -distMod, 0);
        }
        else
        {
            modVector = (pushOriginPosition.x < pushTargetPosition.x) ? new Vector3Int(distMod, 0, 0) : new Vector3Int(-distMod, 0, 0);
        }

        Vector3Int lastValidPosition = pushTargetPosition;
        Vector3Int checkPosition = pushTargetPosition;
        for(int move = 1; move <= Mathf.Abs(distance); move++)
        {
            checkPosition += modVector;
            if(CellInBounds(checkPosition.x, checkPosition.y, checkPosition.z))
            {
                GridCell checkCell = GetCell(checkPosition);
                if(checkCell == null)
                {
                    for(int h = 1; h <= maxHeightDiff; h++)
                    {
                        Vector3Int heightPosition = new Vector3Int(checkPosition.x, checkPosition.y, checkPosition.z - h);
                        if(!CellInBounds(heightPosition.x, heightPosition.y, heightPosition.z))
                        {
                            forcedPath.Add(GetCell(lastValidPosition));
                            return forcedPath;
                            // return GetCell(lastValidPosition);
                        }
                        
                        GridCell heightCell = GetCell(heightPosition);
                        if(heightCell == null)
                        {
                            continue;
                        }
                        if(heightCell.buildNoNeighbours)
                        {
                            forcedPath.Add(GetCell(lastValidPosition));
                            return forcedPath;
                            // return GetCell(lastValidPosition);
                        }
                        if(heightCell.walkable)
                        {
                            forcedPath.Add(heightCell);
                            return forcedPath;
                            // return heightCell;
                        }
                    }
                    forcedPath.Add(GetCell(lastValidPosition));
                    return forcedPath;
                    // return GetCell(lastValidPosition);
                }
                else if(checkCell.buildNoNeighbours || !checkCell.walkable || checkCell.occupiedBy != null)
                {
                    forcedPath.Add(GetCell(lastValidPosition));
                    return forcedPath;
                    // return GetCell(lastValidPosition);
                }

                lastValidPosition = checkCell.position;
            }
            else
            {
                forcedPath.Add(GetCell(lastValidPosition));
                return forcedPath;
                // return GetCell(lastValidPosition);
            }
        }

        forcedPath.Add(GetCell(lastValidPosition));
        return forcedPath;
        // return GetCell(lastValidPosition);
    }

    public Stack<GridCell> CalculatePath(GridCell endCell)
    {
        if(cachedPaths.ContainsKey(endCell.position))
        {
            return cachedPaths[endCell.position];
        }

        Stack<GridCell> path = new Stack<GridCell>();

        if(endCell != null && walkableCells.Count > 0)
        {
            GridCell next = endCell;
            while(next != null)
            {
                path.Push(next);
                next = next.parent;
            }
        }

        cachedPaths.Add(endCell.position, path);

        return path;
    }

    public void RenderPath(Stack<GridCell> path)
    {
        ResetRenderedPath();
        foreach(GridCell cell in path)
        {
            if(cell.tileObj != null)
            {
                cell.tileObj.GetComponent<Tile>().TileType = TileType.PATH;
            }
        }
    }

    public Stack<GridCell> GetAndRenderPath(GridCell endCell)
    {
        Stack<GridCell> path = GetPath(endCell);
        RenderPath(path);
        return path;
    }

    public Stack<GridCell> GetAndRenderPath(Vector3Int endPosition)
    {
        Stack<GridCell> path = GetPath(endPosition);
        RenderPath(path);
        return path;
    }

    public void ResetRenderedPath()
    {
        foreach(GridCell cell in walkableCells)
        {
            if(cell.tileObj != null)
            {
                cell.tileObj.GetComponent<Tile>().TileType = TileType.WALKING_ZONE;
            }
        }
    }

    public void TravelPath(Stack<GridCell> path)
    {
        OnTravelPath?.Invoke(path);
    }

    public void TravelEnter(UnitData unitData, GridCell cell)
    {
        OnTravelEnter?.Invoke(unitData, cell);
    }

    public void TravelPathEnd()
    {
        ModBool cancelExecution = new ModBool(false);
        Action<ModBool> callback = (ModBool cancelled) => {
            OnTravelPathEnd?.Invoke();
        };
        battleManager.ResolveInterrupts(cancelExecution, callback);
    }
    #endregion

    #region Getters
    GameObject GetPooledTile()
    {
        for(int i = 0; i < pooledTiles.Count; i++)
        {
            if(!pooledTiles[i].activeInHierarchy)
            {
                return pooledTiles[i];
            }
        }

        GameObject tileObj = Instantiate(tilePrefab, mapHolder);
        tileObj.SetActive(false);
        pooledTiles.Add(tileObj);
        return tileObj;
    }

    public List<GridCell> GetTargetableCells()
    {
        return targetableCells;
    }
    
    public List<GridCell> GetTargetedCells()
    {
        return targetedCells;
    }

    public List<GridCell> GetTargetableCells(SkillData skillData, GridCell originCell)
    {
        List<GridCell> cells = new List<GridCell>();
        if(skillData.targetType == TargetType.SELF)
        {
            cells.Add(originCell);
        }
        else
        {
            bool targetSelf = skillData.IsSelfTargeting();
            
            switch(skillData.rangeType)
            {
                case RangeType.AREA:
                    cells.AddRange(GetTargetableArea(originCell, skillData.range, skillData.height, targetSelf));
                break;
                case RangeType.LINE:
                    cells.AddRange(GetTargetableLine(originCell, skillData.range, skillData.height, targetSelf));
                break;
                default:
                    // nothing
                break;
            }
        }

        return cells;
    }

    public GridCell GetClosestUnoccupiedGridCell(Vector3Int targetPosition, UnitData unitData)
    {
        GridCell closestCell = cellList
                                .Where(gc => gc.walkable && (gc.occupiedBy == null || gc.occupiedBy == unitData))
                                .OrderBy(gc =>  Vector3Int.Distance(gc.position, targetPosition))
                                .FirstOrDefault();
        return closestCell;
    }

    public GridCell GetClosestUnoccupiedGridCell(Vector3 targetPosition, UnitData unitData)
    {
        GridCell closestCell = cellList
                                .Where(gc => gc.walkable && (gc.occupiedBy == null || gc.occupiedBy == unitData))
                                .OrderBy(gc =>  Vector3.Distance(gc.realWorldPosition, targetPosition))
                                .FirstOrDefault();
        return closestCell;
    }

    public GridCell GetClosestGridCell(Vector3 targetPosition, bool walkable = true)
    {
        if(walkable)
        {
            GridCell closestCell = cellList.Where(gc => gc.walkable).OrderBy(gc => Vector3.Distance(gc.realWorldPosition, targetPosition)).FirstOrDefault();
            return closestCell;
        }
        else
        {
            GridCell closestCell = cellList.OrderBy(gc => Vector3.Distance(gc.realWorldPosition, targetPosition)).FirstOrDefault();
            return closestCell;
        }
    }

    public GridCell GetClosestGridCell(Vector3Int targetPosition, List<GridCell> customList)
    {
        GridCell closestCell = customList.OrderBy(gc => Vector3Int.Distance(gc.position, targetPosition)).FirstOrDefault();
        return closestCell;
    }

    public Stack<GridCell> GetPath(Vector3Int endPosition)
    {
        Stack<GridCell> path = new Stack<GridCell>();
        GridCell endCell = grid[endPosition.x, endPosition.y, endPosition.z];

        return CalculatePath(endCell);
    }

    public Stack<GridCell> GetPath(GridCell endCell)
    {
        return CalculatePath(endCell);
    }

    public GridCell GetCell(Vector3Int cellPosition)
    {
        return grid[cellPosition.x, cellPosition.y, cellPosition.z];
    }

    public List<GridCell> GetWalkableCells()
    {
        return walkableCells;
    }
    #endregion

    #region Map Data
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

            this.transform.position = gridAnchor;
            
            // reset the grid with the new parameters
            ResetGrid();

            foreach(GridCell gridCell in mapInstance.gridData)
            {
                grid[gridCell.position.x, gridCell.position.y, gridCell.position.z] = gridCell;
                cellList.Add(gridCell);
            }
        }
    }

    public void SetMapData(MapData newMapData)
    {
        mapData = newMapData;
        ResetGrid();
        if(mapData)
        {
            LoadFromMapData();
        }
    }

    void ResetGrid()
    {
        grid = new GridCell[gridSize.x, gridSize.y, gridSize.z];
        cellList = new List<GridCell>();
    }

    bool CellInBounds(int x, int y, int z)
    {
        if(x < 0 || y < 0 || z < 0)
        {
            return false;
        }
        if(x >= gridSize.x || y >= gridSize.y || z >= gridSize.z)
        {
            return false;
        }
        return true;
    }

    void RenderCell(GridCell gridCell, TileType tileType)
    {
        if(renderedTiles.ContainsKey(gridCell))
        {
            renderedTiles[gridCell].GetComponent<Tile>().TileType = tileType;
            return;
        }

        GameObject tileObj = GetPooledTile();
        
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

        tileObj.transform.position = gridCell.realWorldPosition;
        tileObj.transform.localScale = new Vector3(tileSizeX, .01f, tileSizeZ);
        tileObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, gridCell.realWorldNormal);
        gridCell.tileObj = tileObj;
        tileObj.SetActive(true);
        tileObj.GetComponent<Tile>().SetGridCell(gridCell, tileType);

        renderedTiles.Add(gridCell, tileObj);
    }
    #endregion

    #region Skill Targeting
    List<GridCell> GetTargetableArea(GridCell originCell, int maxDistance, int maxUp, bool includeSelf = false)
    {
        List<GridCell> cells = new List<GridCell>();
        for(int x = 0; x <= maxDistance; x++)
        {
            int targetXA = originCell.position.x + x;
            int targetXB = originCell.position.x - x;
            for(int y = 0; y <= maxDistance; y++)
            {
                // must be within the max distance, which will cut off corners
                if((x + y) <= maxDistance)
                {
                    int targetYA = originCell.position.y + y;
                    int targetYB = originCell.position.y - y;
                    for(int z = 0; z <= maxUp; z++)
                    {
                        // checking the origin square, since +0/-0 means it just checks the same square 8 times
                        if(x == 0 && y == 0)
                        {
                            if(z == 0 && includeSelf)
                            {
                                cells.Add(originCell);
                            }
                            continue;
                        }

                        int targetZA = originCell.position.z + z;
                        int targetZB = originCell.position.z - z;

                        if(CheckIfTargetable(targetXA, targetYA, targetZA))
                        {
                            cells.Add(grid[targetXA, targetYA, targetZA]);
                        }

                        if(x > 0)
                        {
                            if(CheckIfTargetable(targetXB, targetYA, targetZA))
                            {
                                cells.Add(grid[targetXB, targetYA, targetZA]);
                            }
                            
                            if(y > 0)
                            {
                                if(CheckIfTargetable(targetXB, targetYB, targetZA))
                                {
                                    cells.Add(grid[targetXB, targetYB, targetZA]);
                                }

                                if(z > 0)
                                {
                                    if(CheckIfTargetable(targetXB, targetYB, targetZB))
                                    {
                                        cells.Add(grid[targetXB, targetYB, targetZB]);
                                    }
                                }
                            }

                            if(z > 0)
                            {
                                if(CheckIfTargetable(targetXB, targetYA, targetZB))
                                {
                                    cells.Add(grid[targetXB, targetYA, targetZB]);
                                }
                            }
                        }

                        if(y > 0)
                        {
                            if(CheckIfTargetable(targetXA, targetYB, targetZA))
                            {
                                cells.Add(grid[targetXA, targetYB, targetZA]);
                            }

                            if(z > 0)
                            {
                                if(CheckIfTargetable(targetXA, targetYB, targetZB))
                                {
                                    cells.Add(grid[targetXA, targetYB, targetZB]);
                                }
                            }
                        }

                        if(z != 0)
                        {
                            if(CheckIfTargetable(targetXA, targetYA, targetZB))
                            {
                                cells.Add(grid[targetXA, targetYA, targetZB]);
                            }
                        }
                    }
                }
            }
        }

        return cells;
    }

    List<GridCell> GetTargetableLine(GridCell originCell, int maxDistance, int maxUp, bool includeSelf = false)
    {
        List<GridCell> cells = new List<GridCell>();
        for(int d = 0; d <= maxDistance; d++)
        {
            int targetXA = originCell.position.x + d;
            int targetXB = originCell.position.x - d;
            int targetYA = originCell.position.y + d;
            int targetYB = originCell.position.y - d;

            for(int z = 0; z <= maxUp; z++)
            {
                if(d == 0 && z == 0)
                {
                    if(includeSelf)
                    {
                        cells.Add(originCell);
                    }
                    continue;
                }
                int targetZA = originCell.position.z + z;
                int targetZB = originCell.position.z - z;

                // check along the X axis
                if(CheckIfTargetable(targetXA, originCell.position.y, targetZA))
                {
                    cells.Add(grid[targetXA, originCell.position.y, targetZA]);
                }
                if(CheckIfTargetable(targetXB, originCell.position.y, targetZA))
                {
                    cells.Add(grid[targetXB, originCell.position.y, targetZA]);
                }
                if(targetZB != 0 && CheckIfTargetable(targetXA, originCell.position.y, targetZB))
                {
                    cells.Add(grid[targetXA, originCell.position.y, targetZB]);
                }
                if(targetZB != 0 && CheckIfTargetable(targetXB, originCell.position.y, targetZB))
                {
                    cells.Add(grid[targetXB, originCell.position.y, targetZB]);
                }

                // check along the Y axis
                if(CheckIfTargetable(originCell.position.x, targetYA, targetZA))
                {
                    cells.Add(grid[originCell.position.x, targetYA, targetZA]);
                }
                if(CheckIfTargetable(originCell.position.x, targetYB, targetZA))
                {
                    cells.Add(grid[originCell.position.x, targetYB, targetZA]);
                }
                if(targetZB != 0 && CheckIfTargetable(originCell.position.x, targetYA, targetZB))
                {
                    cells.Add(grid[originCell.position.x, targetYA, targetZB]);
                }
                if(targetZB != 0 && CheckIfTargetable(originCell.position.x, targetYB, targetZB))
                {
                    cells.Add(grid[originCell.position.x, targetYB, targetZB]);
                }
            }
        }
        return cells;
    }

    public bool CheckIfTargetable(int x, int y, int z)
    {
        return CellInBounds(x, y, z) && grid[x, y, z] != null && !grid[x, y, z].buildNoNeighbours && grid[x, y, z].targetable;
    }

    public bool CheckIfTargetable(Vector3Int position)
    {
        int x = position.x;
        int y = position.y;
        int z = position.z;

        return CheckIfTargetable(x, y, z);
    }
    
    void RenderTargetableCells()
    {
        foreach(GridCell targetCell in targetableCells)
        {
            RenderCell(targetCell, TileType.TARGETABLE);
        }
    }

    public void RenderTargetableTiles(List<GridCell> cells)
    {
        ResetTargetableCells();
        foreach(GridCell gridCell in cells)
        {
            RenderCell(gridCell, TileType.TARGETABLE);
        }
    }

    void RenderTargetedCells()
    {
        foreach(GridCell targetCell in targetedCells)
        {
            RenderCell(targetCell, TileType.TARGETED);
        }
    }
    public void RenderTargetedTiles(List<GridCell> cells)
    {
        ResetTargetedCells();
        foreach(GridCell gridCell in cells)
        {
            RenderCell(gridCell, TileType.TARGETED);
        }
    }

    #endregion

    #region Clear Functions
    public void ClearAllTiles()
    {
        ClearWalkabilityTiles();
        ClearTargetableTiles();
        ClearTargetedTiles();
    }

    public void ClearOccupiedBy()
    {
        for(int i = 0; i < cellList.Count; i++)
        {
            cellList[i].occupiedBy = null;
        }
    }

    public void ClearWalkabilityTiles()
    {
        foreach(GridCell gridCell in walkableCells)
        {
            GameObject tileObj = null;
            if(renderedTiles.TryGetValue(gridCell, out tileObj))
            {
                renderedTiles.Remove(gridCell);
                tileObj.SetActive(false);
            }
        }
    }

    public void ClearTargetableTiles()
    {
        foreach(GridCell gridCell in targetableCells)
        {
            GameObject tileObj = null;
            if(renderedTiles.TryGetValue(gridCell, out tileObj))
            {
                renderedTiles.Remove(gridCell);
                tileObj.SetActive(false);
            }
        }
    }

    public void ClearTargetableTiles(List<GridCell> tiles)
    {
        foreach(GridCell gridCell in tiles)
        {
            GameObject tileObj = null;
            if(renderedTiles.TryGetValue(gridCell, out tileObj))
            {
                renderedTiles.Remove(gridCell);
                tileObj.SetActive(false);
            }
        }
    }

    public void ResetTargetableCells()
    {
        ClearTargetableTiles();
        targetableCells.Clear();
    }

    public void ClearTargetedTiles()
    {
        foreach(GridCell gridCell in targetedCells)
        {
            GameObject tileObj = null;
            if(renderedTiles.TryGetValue(gridCell, out tileObj))
            {
                // if this tile exists as part of the targetable grid
                // we don't want to get rid of it, just toggle it on off
                if(targetableCells.Contains(gridCell))
                {
                    tileObj.GetComponent<Tile>().TileType = TileType.TARGETABLE;
                }
                else // we only exist because the target 
                {
                    renderedTiles.Remove(gridCell);
                    tileObj.SetActive(false);
                }
            }
        }
    }

    public void ClearTargetedTiles(List<GridCell> tiles)
    {
        foreach(GridCell gridCell in tiles)
        {
            GameObject tileObj = null;
            if(renderedTiles.TryGetValue(gridCell, out tileObj))
            {
                // if this tile exists as part of the targetable grid
                // we don't want to get rid of it, just toggle it on off
                if(targetableCells.Contains(gridCell))
                {
                    tileObj.GetComponent<Tile>().TileType = TileType.TARGETABLE;
                }
                else // we only exist because the target 
                {
                    renderedTiles.Remove(gridCell);
                    tileObj.SetActive(false);
                }
            }
        }
    }

    public void ResetTargetedCells()
    {
        ClearTargetedTiles();
        targetedCells.Clear();
    }
    #endregion    

    void OnDestroy()
    {
        UnbindEvents();
    }
}
