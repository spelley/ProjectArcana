using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterUI : MonoBehaviour
{
    // cached references
    BattleManager battleManager;
    MapManager mapManager;
    CameraController cameraController;
    CharacterMotor characterMotor;
    public Animator anim { get; private set; }

    [SerializeField]
    GameObject popUpContainer;
    [SerializeField]
    TextMeshPro popUpText;

    Queue<PopUp> popUpQueue = new Queue<PopUp>();
    bool processingQueue = false;

    void Awake()
    {

    }

    void Start()
    {
        mapManager = MapManager.Instance;
        battleManager = BattleManager.Instance;
        characterMotor = GetComponent<CharacterMotor>();
        anim = GetComponent<Animator>();
        cameraController = Camera.main.GetComponent<CameraController>();

        characterMotor.unitData.OnReceiveDamage += OnReceiveDamage;
        characterMotor.unitData.OnReceiveHeal += OnReceiveHeal;
        characterMotor.unitData.OnAddStatusEffect += OnAddStatusEffect;
        characterMotor.unitData.OnRemoveStatusEffect += OnRemoveStatusEffect;
        characterMotor.unitData.OnUnitExperience += OnUnitExperience;
        characterMotor.unitData.OnUnitJobExperience += OnUnitJobExperience;
        characterMotor.unitData.OnUnitLevel += OnUnitLevel;
        characterMotor.unitData.OnUnitJobLevel += OnUnitJobLevel;
    }

    void OnDestroy()
    {
        characterMotor.unitData.OnReceiveDamage -= OnReceiveDamage;
        characterMotor.unitData.OnReceiveHeal -= OnReceiveHeal;
        characterMotor.unitData.OnAddStatusEffect -= OnAddStatusEffect;
        characterMotor.unitData.OnRemoveStatusEffect -= OnRemoveStatusEffect;
        characterMotor.unitData.OnUnitExperience -= OnUnitExperience;
        characterMotor.unitData.OnUnitJobExperience -= OnUnitJobExperience;
        characterMotor.unitData.OnUnitLevel -= OnUnitLevel;
        characterMotor.unitData.OnUnitJobLevel -= OnUnitJobLevel;
    }

    void Update()
    {
        if(!processingQueue && popUpQueue.Count > 0)
        {
            StartCoroutine(PopUpCoroutine());
        }
    }

    public void OnReceiveDamage(IDamageable source, IDamageable target, List<ElementData> elements, int damage)
    {   
        ShowPopUp(damage.ToString(), new Color32(255, 255, 255, 255));
        if(damage > 0 && source != target)
        {
            anim.SetTrigger("ReceiveDamage");
        }
    }

    public void ShowPopUp(string text, Color32 textColor, float timer = 1f, float fadeTimer = 0.5f)
    {
        popUpQueue.Enqueue(new PopUp(text, textColor, timer, fadeTimer));
    }

    IEnumerator PopUpCoroutine()
    {
        processingQueue = true;

        while(popUpQueue.Count > 0)
        {
            PopUp popUp = popUpQueue.Dequeue();

            float curTime = 0f;
            popUpText.faceColor = popUp.textColor;
            popUpText.text = popUp.text;
            popUpContainer.SetActive(true);
            while(curTime < popUp.timer)
            {
                curTime += Time.deltaTime;
                yield return null;
            }
            curTime = 0;
            Color32 fadeOutTextColor = new Color32(popUp.textColor.r, popUp.textColor.g, popUp.textColor.b, 0);
            while(curTime < popUp.fadeTimer)
            {
                curTime += Time.deltaTime;
                popUpText.faceColor = Color32.Lerp(popUp.textColor, fadeOutTextColor, (curTime / popUp.fadeTimer));
                yield return null;
            }
            popUpContainer.SetActive(false);
            yield return new WaitForSeconds(.2f);
        }
        processingQueue = false;
    }

    public void OnReceiveHeal(IDamageable source, IDamageable target, List<ElementData> elements, int heal)
    {
        ShowPopUp(heal.ToString(), new Color32(0, 255, 0, 255));
        if(heal > 0 && source != target)
        {
            anim.SetTrigger("ReceiveHeal");
        }
    }

    public void OnAddStatusEffect(StatusEffect status)
    {
        ShowPopUp('+'+status.statusName, new Color32(0, 255, 0, 255));
    }

    public void OnRemoveStatusEffect(StatusEffect status)
    {
        ShowPopUp('-'+status.statusName, new Color32(255, 0, 0, 255));
    }

    public void OnUnitExperience(int amount)
    {
        ShowPopUp(amount.ToString()+"XP", new Color32(255, 255, 0, 255));
    }

    public void OnUnitJobExperience(UnitJob unitJob, int amount)
    {
        ShowPopUp(amount.ToString()+"CP", new Color32(255, 0, 255, 255));
    }

    public void OnUnitLevel(int amount)
    {
        string lvlText = amount == 1 ? "Level Up!" : "Level Up x"+amount.ToString()+"!";
        ShowPopUp(lvlText, new Color32(255, 255, 0, 255));
    }

    public void OnUnitJobLevel(UnitJob unitJob, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            ShowPopUp(unitJob.jobData.jobName+" Level Up!", new Color32(255, 0, 255, 255));
        }
    }
}