using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterBox : MonoBehaviour
{
    [SerializeField]
    List<GameObject> enemies = new List<GameObject>();
    List<GameObject> winLeaders = new List<GameObject>();
    List<GameObject> lossLeaders = new List<GameObject>();
    [SerializeField]
    List<EncounterCondition> _winConditions = new List<EncounterCondition>();
    List<EncounterCondition> _lossConditions = new List<EncounterCondition>();

    [SerializeField]
    bool expireOnEncounter = true;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && !BattleManager.Instance.inCombat)
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
            BattleManager.Instance.StartEncounter(turnTakers, winConditions, lossConditions);

            if(expireOnEncounter)
            {
                Expire();
            }
        }
    }

    void Expire()
    {
        Destroy(this.gameObject);
    }
}
