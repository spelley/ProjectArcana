using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleObstacle : MonoBehaviour, IPointerClickHandler
{
    public List<Vector3Int> cellsToToggle = new List<Vector3Int>();

    void Start()
    {
        GridCell gridCell = MapManager.Instance.GetCell(cellsToToggle[0]);
        gridCell.walkable = false;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        foreach(Vector3Int cellPosition in cellsToToggle)
        {
            GridCell gridCell = MapManager.Instance.GetCell(cellPosition);
            if(gridCell != null)
            {
                gridCell.walkable = !gridCell.walkable;
            }
        }
    }
}
