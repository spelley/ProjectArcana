using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterBox : MonoBehaviour
{
    [SerializeField]
    List<GameObject> enemies = new List<GameObject>();

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
            BattleManager.Instance.StartEncounter(turnTakers);

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
