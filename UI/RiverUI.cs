using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RiverUI : MonoBehaviour
{
    BattleManager battleManager;
    [SerializeField]
    GameObject riverUI;
    [SerializeField]
    GameObject riverListContainer;
    [SerializeField]
    GameObject riverCardPrefab;

    [SerializeField] GameObject divinationHighlightPanel;
    [SerializeField] TextMeshProUGUI divinationInstructionText;
    [SerializeField] Button divinationConfirmButton;

    List<RiverCard> myRiver;
    public bool targeting { get; private set; }
    public List<RiverCard> targetedCards = new List<RiverCard>();

    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.Instance;
        riverUI.SetActive(false);
        battleManager.OnEncounterStart += OnEncounterStart;
        battleManager.OnEncounterEnd += OnEncounterEnd;
        battleManager.OnUpdateRiver += OnUpdateRiver;
        battleManager.OnDivinationTarget += OnDivinationTarget;
        battleManager.OnDivinationTargetCancel += OnDivinationTargetCancel;
        battleManager.OnDivinationConfirm += OnDivinationConfirm;
        battleManager.OnDivinationClear += OnDivinationClear;
    }

    void OnDestroy()
    {
        battleManager.OnEncounterStart -= OnEncounterStart;
        battleManager.OnEncounterEnd -= OnEncounterEnd;
        battleManager.OnUpdateRiver -= OnUpdateRiver;
        battleManager.OnDivinationTarget -= OnDivinationTarget;
        battleManager.OnDivinationTargetCancel -= OnDivinationTargetCancel;
        battleManager.OnDivinationConfirm -= OnDivinationConfirm;
        battleManager.OnDivinationClear -= OnDivinationClear;
    }

    void OnEncounterStart()
    {
        riverUI.SetActive(true);
    }

    void OnEncounterEnd()
    {
        riverUI.SetActive(false);
    }

    void OnDivinationTarget(DivinationData divinationData, UnitData unitData)
    {
        targetedCards.Clear();
        divinationHighlightPanel.SetActive(true);
        divinationInstructionText.gameObject.SetActive(true);
        divinationInstructionText.text = divinationData.instructionsText;
        divinationConfirmButton.gameObject.SetActive(true);
        targeting = true;
    }

    void OnDivinationTargetCancel(DivinationData divinationData, UnitData unitData)
    {
        ClearDivinationUI();
        targetedCards.Clear();
    }

    void ClearDivinationUI()
    {
        divinationHighlightPanel.SetActive(false);
        divinationInstructionText.text = "";
        divinationInstructionText.gameObject.SetActive(false);
        divinationConfirmButton.gameObject.SetActive(false);
        targeting = false;
    }

    void OnDivinationConfirm(DivinationData divinationData, UnitData unitData, List<RiverCard> riverCards)
    {
        ClearDivinationUI();
    }

    void OnDivinationClear(DivinationData divinationData)
    {
        ClearDivinationUI();
        targetedCards.Clear();
    }

    void OnDivinationConfirmButton()
    {
        battleManager.DivinationConfirm(battleManager.curDivination, targetedCards);
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
        if(Input.GetKeyDown(KeyCode.Escape) && targeting)
        {
            battleManager.DivinationTargetCancel(battleManager.curDivination);
        }
    }
}
