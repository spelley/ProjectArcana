using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RiverCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    Animator animator;
    RiverCard riverCard;
    Action<RiverCard, RiverCardUI> selectCardCallback;
    [SerializeField]
    Outline outline;
    [SerializeField]
    Image icon;
    [SerializeField]
    Color defaultOutlineColor;
    [SerializeField]
    Color highlightOutlineColor;
    [SerializeField]
    GameObject lockIcon;
    [SerializeField]
    GameObject deactivatedIcon;

    public bool highlighted { get; private set; }

    void Awake()
    {
        animator = GetComponent<Animator>();
        outline.effectColor = defaultOutlineColor;
    }

    void Start()
    {
        BattleManager.Instance.OnPreparedSkillInit += OnSkillTarget;
        BattleManager.Instance.OnPreparedSkillCancel += OnSkillTargetCancel;
    }

    void OnDestroy()
    {
        BattleManager.Instance.OnPreparedSkillInit -= OnSkillTarget;
        BattleManager.Instance.OnPreparedSkillCancel -= OnSkillTargetCancel;
    }

    void OnSkillTarget(BattleSkill battleSkill)
    {
        if(riverCard.inactive)
        {
            return;
        }
        
        foreach(ElementData element in battleSkill.skill.elements)
        {
            if(element == riverCard.element)
            {
                Highlight();
                break;
            }
        }
    }

    public void Highlight()
    {
        highlighted = true;
        animator.SetBool("Highlighted", true);
    }

    public void Unhighlight()
    {
        highlighted = false;
        animator.SetBool("Highlighted", false);
    }

    void OnSkillTargetCancel()
    {
        if(highlighted)
        {
            Unhighlight();
        }
    }

    public void SetData(RiverCard newCard, Action<RiverCard, RiverCardUI> callback)
    {
        riverCard = newCard;
        selectCardCallback = callback;
        icon.sprite = riverCard.element.icon;
        icon.color = riverCard.element.color;
        if(riverCard.locked)
        {
            lockIcon.SetActive(true);
        }
        else
        {
            lockIcon.SetActive(false);
        }
        if(riverCard.inactive)
        {
            deactivatedIcon.SetActive(true);
        }
        else
        {
            deactivatedIcon.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.effectColor = highlightOutlineColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        selectCardCallback?.Invoke(riverCard, this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.effectColor = defaultOutlineColor;
    }
}
