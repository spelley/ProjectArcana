using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetPlayerFocus());
    }

    IEnumerator SetPlayerFocus()
    {
        yield return null;
        yield return null;
        Camera.main.GetComponent<CameraController>().SetFocus(GameManager.Instance.activePlayer.unitGO);
    }
}
