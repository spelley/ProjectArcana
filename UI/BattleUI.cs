using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    void OnDestroy()
    {
        battleManager.OnEncounterStart -= OnEncounterStart;
        battleManager.OnEncounterEnd -= OnEncounterEnd;
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
