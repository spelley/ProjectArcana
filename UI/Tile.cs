using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    MapManager mapManager;
    BattleManager battleManager;
    
    [SerializeField]
    public GridCell gridCell;

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
                    if(!gridCell.walkable && gridCell.targetable)
                    {
                        tileMaterial.material = inactiveMat;
                    }
                    else
                    {
                        tileMaterial.material = gridCell != null && gridCell.walkable ? defaultMat : unwalkableMat;
                    } 
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
        if(battleManager.curUnit == null 
            || battleManager.curUnit.faction != Faction.ALLY 
            || battleManager.curUnit.aiBrain != null 
            || battleManager.curUnit.unitGO.GetComponent<TacticsMotor>().isMoving)
        {
            return;
        }
        if(this.TileType == TileType.WALKING_ZONE || this.TileType == TileType.PATH)
        {
            mapManager.GetAndRenderPath(gridCell);
        }
        else if(battleManager.preparedSkill != null && !battleManager.targetLocked 
                && (this.TileType == TileType.TARGETABLE || this.TileType == TileType.TARGETED))
        {
            battleManager.PreparedSkillPreviewTarget(gridCell);
        }
    }

    public void OnMouseExit()
    {
        if(battleManager.curUnit == null 
            || battleManager.curUnit.faction != Faction.ALLY 
            || battleManager.curUnit.aiBrain != null 
            || battleManager.curUnit.unitGO.GetComponent<TacticsMotor>().isMoving)
        {
            return;
        }
        if(this.TileType == TileType.PATH)
        {
            mapManager.ResetRenderedPath();
            return;
        }
    }

    public void OnMouseDown()
    {
        if(battleManager.curUnit == null 
            || battleManager.curUnit.faction != Faction.ALLY 
            || battleManager.curUnit.aiBrain != null 
            || battleManager.curUnit.unitGO.GetComponent<TacticsMotor>().isMoving
            || battleManager.targetLocked)
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
        else if((this.TileType == TileType.TARGETABLE || this.TileType == TileType.TARGETED) 
            && battleManager.preparedSkill != null 
            && ((battleManager.preparedSkill.skill.requireEmptyTile && gridCell.occupiedBy == null)
                || battleManager.preparedSkill.skill.requireUnitTarget))
        {
            UnitData unitData = battleManager.curUnit;
            battleManager.PreparedSkillSelectTarget(gridCell);
        }
    }
}
