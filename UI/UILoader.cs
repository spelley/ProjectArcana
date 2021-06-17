using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoader : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadUI(5));
    }

    IEnumerator LoadUI(int delayFrames = 1)
    {
        int i = 0;
        while(i < delayFrames)
        {
            i++;
            yield return null;
        }
        SceneManager.LoadSceneAsync("BattleUI", LoadSceneMode.Additive);
    }
}
