using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    MapManager mapManager;
    BattleManager battleManager;
    
    [SerializeField]
    GridCell gridCell;

    [SerializeField]
    MeshRenderer tileMaterial;

    [SerializeField]
    Material defaultMat;

    [SerializeField]
    Material pathMat;

    [SerializeField]
    Material targetableMat;

    [SerializeField]
    Material targetedMat;

    [SerializeField]
    Material unwalkableMat;

    [SerializeField]
    Material inactiveMat;

    TileType _tileType;
    public TileType TileType
    {
        get
        {
            return _tileType;
        }
        set
        {
            _tileType = value;
            switch(value)
            {
                case TileType.WALKING_ZONE:
                    tileMaterial.material = gridCell != null && gridCell.walkable ? defaultMat : unwalkableMat;
                break;
                case TileType.PATH:
                    tileMaterial.material = pathMat;
                break;
                case TileType.TARGETABLE:
                    tileMaterial.material = targetableMat;
                break;
                case TileType.TARGETED:
                    tileMaterial.material = targetedMat;
                break;
                case TileType.VISUAL_ONLY:
                default:
                    tileMaterial.material = inactiveMat;
                break;
            }
        }
    }

    void Start()
    {
        mapManager = MapManager.Instance;
        battleManager = BattleManager.Instance;
    }

    public void SetGridCell(GridCell gridCell, TileType tileType = TileType.WALKING_ZONE)
    {
        this.gridCell = gridCell;
        this.TileType = tileType;
    }

    public void OnMouseEnter()
    {
        if(battleManager.curUnit == null || battleManager.curUnit.faction != Faction.ALLY || battleManager.previewingSkill)
        {
            return;
        }
        if(this.TileType == TileType.WALKING_ZONE || this.TileType == TileType.PATH)
        {
            mapManager.GetAndRenderPath(gridCell);
        }
        else if(this.TileType == TileType.TARGETABLE || this.TileType == TileType.TARGETED)
        {
            battleManager.SkillSelectTarget(battleManager.curSkill, gridCell);
        }
    }

    public void OnMouseExit()
    {
        if(battleManager.curUnit == null || battleManager.curUnit.faction != Faction.ALLY)
        {
            return;
        }
        if(this.TileType == TileType.PATH)
        {
            mapManager.ResetRenderedPath();
            return;
        }
        if(this.TileType == TileType.TARGETED)
        {
            //battleManager.SkillSelectTargetCancel(battleManager.curSkill, gridCell);
        }
    }

    public void OnMouseDown()
    {
        if(battleManager.curUnit == null || battleManager.curUnit.faction != Faction.ALLY)
        {
            return;
        }

        if(gridCell.occupiedBy == null && (this.TileType == TileType.WALKING_ZONE || this.TileType == TileType.PATH))
        {
            Stack<GridCell> path = mapManager.GetAndRenderPath(gridCell);
            if(path.Count > 0)
            {
                mapManager.TravelPath(path);
            }
        }
        else if(this.TileType == TileType.TARGETABLE || this.TileType == TileType.TARGETED)
        {
            UnitData unitData = battleManager.curUnit;
            if(unitData == null)
            {
                unitData = GameObject.FindWithTag("Player").GetComponent<CharacterMotor>().unitData;
            }
            battleManager.SkillPreview(battleManager.curSkill, unitData, mapManager.GetTargetedCells());
        }
    }
}
