using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CellUI : MonoBehaviour
{
    Camera cam;
    Ray camRay;
    RaycastHit hit;
    GridCell curCell;

    [SerializeField]
    GameObject positionContainer;
    [SerializeField]
    TextMeshProUGUI positionText;
    [SerializeField]
    bool showHighlighter = false;
    [SerializeField]
    GameObject cellHighlighterPrefab;
    GameObject cellHighlighter;

    Vector3 lastPosition;

    void Start()
    {
        cam = Camera.main;
        cellHighlighter = Instantiate(cellHighlighterPrefab);
        cellHighlighter.SetActive(false);
    }

    void Update()
    {
        if(!BattleManager.Instance.inCombat || Input.mousePosition == lastPosition)
        {
            curCell = null;
            if(cellHighlighter != null && !BattleManager.Instance.inCombat)
            {
                cellHighlighter.SetActive(false);
            }
            return;
        }

        lastPosition = Input.mousePosition;
        camRay = cam.ScreenPointToRay(Input.mousePosition);
        
        if(Physics.Raycast(camRay, out hit))
        {
            CharacterMotor charMotor = hit.transform.gameObject.GetComponent<CharacterMotor>();
            if(charMotor != null)
            {
                GridCell newCell = MapManager.Instance.GetCell(charMotor.unitData.curPosition);
                UpdateUI(newCell);
                return;
            }
            else
            {
                GridCell newCell = MapManager.Instance.GetClosestGridCell(hit.point, false);
                UpdateUI(newCell);
            }
        }
    }

    void UpdateUI(GridCell newCell)
    {
        if(newCell == curCell)
        {
            return;
        }

        if(newCell != null)
        {
            positionContainer.SetActive(true);
            positionText.text = "X: "+newCell.position.x+" / Y: "+newCell.position.y+"\nHeight: "+newCell.position.z;
            if(showHighlighter)
            {
                float tileSize = MapManager.Instance.tileSize;
                float tileSizeX = tileSize;
                if(newCell.realWorldNormal.x != 0f)
                {
                    tileSizeX = tileSize / newCell.realWorldNormal.y;
                }

                float tileSizeZ = tileSize;
                if(newCell.realWorldNormal.z != 0f)
                {
                    tileSizeZ = tileSize / newCell.realWorldNormal.y;
                }
                cellHighlighter.transform.position = newCell.realWorldPosition;
                cellHighlighter.transform.localScale = new Vector3(tileSizeX, .01f, tileSizeZ);
                cellHighlighter.transform.rotation = Quaternion.FromToRotation(Vector3.up, newCell.realWorldNormal);
                cellHighlighter.SetActive(true);
            }
            curCell = newCell;
        }
        else
        {
            positionContainer.SetActive(false);
            cellHighlighter.SetActive(false);
            curCell = newCell;
        }
    }
}
