using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterTrigger : MonoBehaviour
{
    BoxCollider boxCollider;
    [SerializeField]
    List<GameObject> enemies = new List<GameObject>();
    List<GameObject> winLeaders = new List<GameObject>();
    List<GameObject> lossLeaders = new List<GameObject>();
    [SerializeField]
    List<EncounterCondition> _winConditions = new List<EncounterCondition>();
    List<EncounterCondition> _lossConditions = new List<EncounterCondition>();

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void StartEncounter()
    {
        if(!BattleManager.Instance.inCombat)
        {
            List<ITurnTaker> turnTakers = new List<ITurnTaker>();
            foreach(GameObject unit in enemies)
            {
                turnTakers.Add(unit.GetComponent<CharacterMotor>().unitData);
            }
            List<EncounterCondition> winConditions = new List<EncounterCondition>();
            foreach(EncounterCondition winCondition in _winConditions)
            {
                EncounterCondition winConditionCopy = Instantiate(winCondition);
                foreach(GameObject winLeader in winLeaders)
                {
                    winConditionCopy.leaders.Add(winLeader.GetComponent<CharacterMotor>().unitData);
                }
                winConditions.Add(winConditionCopy);
            }

            List<EncounterCondition> lossConditions = new List<EncounterCondition>();
            foreach(EncounterCondition lossCondition in _lossConditions)
            {
                EncounterCondition lossConditionCopy = Instantiate(lossCondition);
                foreach(GameObject lossLeader in lossLeaders)
                {
                    lossConditionCopy.leaders.Add(lossLeader.GetComponent<CharacterMotor>().unitData);
                }
                lossConditions.Add(lossConditionCopy);
            }
            if(boxCollider != null)
            {
                boxCollider.enabled = false;
            }
            BattleManager.Instance.StartEncounter(turnTakers, winConditions, lossConditions);
        }
    }
}
