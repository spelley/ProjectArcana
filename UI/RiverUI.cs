using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverUI : MonoBehaviour
{
    BattleManager battleManager;
    [SerializeField]
    GameObject riverUI;
    [SerializeField]
    GameObject riverListContainer;
    [SerializeField]
    GameObject riverCardPrefab;
    List<RiverCard> myRiver;

    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.Instance;
        riverUI.SetActive(false);
        battleManager.OnEncounterStart += OnEncounterStart;
        battleManager.OnEncounterEnd += OnEncounterEnd;
        battleManager.OnUpdateRiver += OnUpdateRiver;
    }

    void OnDestroy()
    {
        battleManager.OnEncounterStart -= OnEncounterStart;
        battleManager.OnEncounterEnd -= OnEncounterEnd;
        battleManager.OnUpdateRiver -= OnUpdateRiver;
    }

    void OnEncounterStart()
    {
        riverUI.SetActive(true);
    }

    void OnEncounterEnd()
    {
        riverUI.SetActive(false);
    }

    void OnUpdateRiver(List<RiverCard> riverCards)
    {
        ClearRiver();
        for(int i = 0; i < riverCards.Count; i++)
        {
            GameObject rcGO = Instantiate(riverCardPrefab, riverListContainer.transform);
            rcGO.GetComponent<RiverCardUI>().SetData(riverCards[i]);
        }
        myRiver = riverCards;
    }

    void ClearRiver()
    {
        foreach(Transform child in riverListContainer.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
