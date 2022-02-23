using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public MapData mapData;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetPlayerFocus());
    }

    IEnumerator SetPlayerFocus()
    {
        yield return null;
        MapManager.Instance.SetMapData(mapData);
        yield return null;
        Camera.main.GetComponent<CameraController>().SetFocus(GameManager.Instance.activePlayer.unitGO);
    }
}
