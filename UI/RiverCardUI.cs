using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RiverCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Animator animator;
    RiverCard riverCard;
    [SerializeField]
    Outline outline;
    [SerializeField]
    Image icon;
    [SerializeField]
    Color defaultOutlineColor;
    [SerializeField]
    Color highlightOutlineColor;

    public bool highlighted { get; private set; }

    void Awake()
    {
        animator = GetComponent<Animator>();
        outline.effectColor = defaultOutlineColor;
    }

    void Start()
    {
        BattleManager.Instance.OnSkillTarget += OnSkillTarget;
        BattleManager.Instance.OnSkillTargetCancel += OnSkillTargetCancel;
    }

    void OnDestroy()
    {
        BattleManager.Instance.OnSkillTarget -= OnSkillTarget;
        BattleManager.Instance.OnSkillTargetCancel -= OnSkillTargetCancel;
    }

    void OnSkillTarget(SkillData skillData, UnitData unitData, GridCell originCell)
    {
        foreach(ElementData element in skillData.elements)
        {
            if(element == riverCard.element)
            {
                highlighted = true;
                animator.SetBool("Highlighted", true);
                break;
            }
        }
    }

    void OnSkillTargetCancel(SkillData skillData, UnitData unitData)
    {
        if(highlighted)
        {
            highlighted = false;
            animator.SetBool("Highlighted", false);
        }
    }

    public void SetData(RiverCard newCard)
    {
        riverCard = newCard;
        icon.sprite = riverCard.element.icon;
        icon.color = riverCard.element.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.effectColor = highlightOutlineColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.effectColor = defaultOutlineColor;
    }
}
