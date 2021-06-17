using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PredictedTurnsUI : MonoBehaviour
{
    CanvasGroup canvasGroup;
    Animator anim;
    BattleManager bm;

    [SerializeField]
    Transform spriteContainer;
    
    [SerializeField]
    GameObject spritePrefab;

    void Awake()
    {
        anim = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        bm = BattleManager.Instance;
        if(bm != null)
        {
            bm.OnEncounterStart += OnEncounterStart;
            bm.OnEncounterEnd += OnEncounterEnd;
        }
    }

    public void OnEncounterStart()
    {
        canvasGroup.alpha = 100;
        anim.SetBool("Show", true);
        bm.turnManager.OnTurnStart += OnTurnStart;
    }

    public void OnEncounterEnd()
    {
        canvasGroup.alpha = 0;
        bm.turnManager.OnTurnStart -= OnTurnStart;
    }

    void OnTurnStart(ITurnTaker curTurnTaker)
    {
        Queue<ITurnTaker> turnTakers = bm.turnManager.GetPredictedTurns(20);
        ResetSprites();

        CreateSprite(curTurnTaker);
        while(turnTakers.Count > 0)
        {
            ITurnTaker turnTaker = turnTakers.Dequeue();
            CreateSprite(turnTaker);
        }
    }

    void CreateSprite(ITurnTaker turnTaker)
    {
        GameObject spObj = Instantiate(spritePrefab, spriteContainer);
        spObj.GetComponent<Image>().sprite = turnTaker.sprite;
    }

    void ResetSprites()
    {
        foreach(Transform child in spriteContainer) {
            GameObject.Destroy(child.gameObject);
        }
    }

    void OnDestroy()
    {
        if(bm == null)
        {
            return;
        }

        bm.OnEncounterStart -= OnEncounterStart;
        bm.OnEncounterEnd -= OnEncounterEnd;
        if(bm.turnManager != null)
        {
            bm.turnManager.OnTurnStart -= OnTurnStart;
        }
    }
}
