using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleUI : MonoBehaviour
{
    BattleManager battleManager;

    [SerializeField]
    GameObject battleMenu;

    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.Instance;
        battleManager.OnEncounterStart += OnEncounterStart;
        battleManager.OnEncounterEnd += OnEncounterEnd;
        battleManager.OnEncounterLost += OnEncounterLost;
    }

    void OnDestroy()
    {
        battleManager.OnEncounterStart -= OnEncounterStart;
        battleManager.OnEncounterEnd -= OnEncounterEnd;
        battleManager.OnEncounterLost -= OnEncounterLost;
    }

    void OnEncounterLost()
    {
        SceneManager.LoadSceneAsync("GameOver", LoadSceneMode.Additive);
    }

    void OnEncounterStart()
    {
        battleMenu.SetActive(true);
    }

    void OnEncounterEnd()
    {
        battleMenu.SetActive(false);
    }
}
