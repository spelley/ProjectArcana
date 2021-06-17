using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleUI : MonoBehaviour
{
    BattleManager battleManager;
    Animator animator;

    [SerializeField]
    GameObject battleMenu;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.Instance;
        battleManager.OnEncounterAwake += OnEncounterAwake;
        battleManager.OnEncounterStart += OnEncounterStart;
        battleManager.OnEncounterEnd += OnEncounterEnd;
        battleManager.OnEncounterLost += OnEncounterLost;
    }

    void OnDestroy()
    {
        battleManager.OnEncounterAwake -= OnEncounterAwake;
        battleManager.OnEncounterStart -= OnEncounterStart;
        battleManager.OnEncounterEnd -= OnEncounterEnd;
        battleManager.OnEncounterLost -= OnEncounterLost;
    }

    void OnEncounterLost()
    {
        SceneManager.LoadSceneAsync("GameOver", LoadSceneMode.Additive);
    }

    void OnEncounterAwake()
    {
        animator.SetTrigger("StartEncounter");
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
