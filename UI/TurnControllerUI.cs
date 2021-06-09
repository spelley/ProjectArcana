using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnControllerUI : MonoBehaviour
{
    BattleManager battleManager;

    [SerializeField]
    Button endTurnButton;
    [SerializeField]
    Button endEncounterButton;

    void Awake()
    {
        endTurnButton.onClick.AddListener(EndTurn);
        endEncounterButton.onClick.AddListener(EndEncounter);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.Instance;
    }

    public void EndTurn()
    {
        if(battleManager.turnManager != null)
        {
            battleManager.turnManager.EndTurn();
        }
    }

    public void EndEncounter()
    {
        if(battleManager.turnManager != null)
        {
            battleManager.EndEncounter();
        }
    }
}