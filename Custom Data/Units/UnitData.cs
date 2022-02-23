using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Custom Data/Unit Data", order = 2)]
public class UnitData : ScriptableObject, ITurnTaker, IDamageable, ILoadable<UnitSaveData>
{
    [SerializeField] string _id;
    public string id { get { return _id; } }
    string _loadType = "Unit";
    public virtual string loadType { get { return _loadType; } }
    
    [SerializeField] string _unitName;
    public string unitName { get { return _unitName; } } 
    [SerializeField] ArcanaData _arcana;
    public ArcanaData arcana { get { return _arcana; } }
    [SerializeField] GameObject _unitModel;
    public GameObject unitModel { get { return _unitModel; } }
    public GameObject unitGO { get; set; }
    [SerializeField] Faction _faction;
    public Faction faction { get { return _faction; } }
    [SerializeField] AIBrain _aiBrain;
    public AIBrain aiBrain { get { return _aiBrain; } }
    [SerializeField] StatBlock _stats;
    public StatBlock stats { get { return _stats; } }
    [SerializeField] EquipmentBlock _equipmentBlock;
    public EquipmentBlock equipmentBlock { get { return _equipmentBlock; } }
    [SerializeField] JobData _baseJob;
    public JobData baseJob { get { return _baseJob; } }
    [SerializeField] JobData _activeJob;
    public JobData activeJob { get { return _activeJob; } set { _activeJob = value; } }
    [SerializeField] List<UnitJob> _availableJobs = new List<UnitJob>();
    public List<UnitJob> availableJobs { get { return _availableJobs; } }

    bool loaded = false;

    public int maxHP
    {
        get
        {
            return stats.CalculateMaxHP();
        }
        set 
        {
            stats.maxHP = value;
        }
    } 
    public int hp
    {
        get
        {
            return stats.hp;
        }
        set 
        {
            stats.hp = value;
        }
    }

    public int maxMP
    {
        get
        {
            return stats.CalculateMaxMP();
        }
        set 
        {
            stats.maxHP = value;
        }
    } 
    public int mp
    {
        get
        {
            return stats.mp;
        }
        set 
        {
            stats.mp = value;
        }
    }

    public int ct { get; set; }
    public bool incapacitated
    { 
        get
        {
            return CalculateIsIncapacitated();
        }
    }

    public bool isPlayerControlled
    {
        get
        {
            return aiBrain == null && faction == Faction.ALLY;
        }
    }
    public bool moved { get; set; }
    public bool acted { get; set; }
    public bool usedBonus { get; set; }
    public bool canAct
    {
        get
        {
            return CalculateCanAct();
        }
    }

    public bool canMove
    {
        get
        {
            return CalculateCanMove();
        }
    }

    public bool canUseBonus
    {
        get
        {
            return CalculateCanUseBonus();
        }
    }
    
    public int speed
    {
        get
        {
            return stats.CalculateSpeed();
        }
    }

    [SerializeField]
    Sprite _sprite;
    public Sprite sprite
    {
        get
        {
            return _sprite;
        }
        set
        {
            _sprite = value;
        }
    }
    public DivinationData divinationSkill {
        get {
            return activeJob.divinationSkill;
        }
    }
    [SerializeField] List<SkillData> learnedSkills = new List<SkillData>();
    [SerializeField] List<SkillData> assignedSkills = new List<SkillData>();
    [SerializeField] List<PassiveData> learnedPassives = new List<PassiveData>();
    [SerializeField] List<PassiveData> assignedPassives = new List<PassiveData>();

    [SerializeField]
    private Vector3Int _curPosition;
    public Vector3Int curPosition { get { return _curPosition; } set { _curPosition = value; } }
    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    public event Action OnUnitTurnStart;
    public event Action OnUnitTurnEnd;

    public event Action<IDamageable, IDamageable, ModInt> OnPredictDealDamage;
    public event Action<IDamageable, IDamageable, ModInt> OnDealDamageCalculation;
    public event Action<IDamageable, IDamageable, ModInt> OnPredictReceiveDamage;
    public event Action<IDamageable, IDamageable, ModInt> OnReceiveDamageCalculation;
    public event Action<IDamageable, IDamageable, List<ElementData>, int> OnReceiveDamage;

    public event Action<IDamageable, IDamageable, ModInt> OnPredictHealOther;
    public event Action<IDamageable, IDamageable, ModInt> OnHealOtherCalculation;
    public event Action<IDamageable, IDamageable, ModInt> OnPredictReceiveHeal;
    public event Action<IDamageable, IDamageable, ModInt> OnReceiveHealCalculation;
    public event Action<IDamageable, IDamageable, List<ElementData>, int> OnReceiveHeal;

    public event Action<UnitData, UnitData, ModFloat> OnCalculateHitChance;
    public event Action OnUnitMiss;
    
    public event Action<ModBool> OnCalculateIsIncapacitated;
    public event Action<ModBool> OnCalculateCanMove;
    public event Action<ModBool> OnCalculateCanAct;
    public event Action<ModBool> OnCalculateCanUseBonus;

    public event Action<StatusEffect> OnAddStatusEffect;
    public event Action<StatusEffect> OnRemoveStatusEffect;
    
    public event Action<GridCell, Action<Vector3Int>> OnUnitPush;

    public event Action<int> OnUnitExperience;
    public event Action<int> OnUnitLevel;
    public event Action<UnitJob, int> OnUnitJobExperience;
    public event Action<UnitJob, int> OnUnitJobLevel;

    public event Action<ModInt, UnitData, List<ElementData>> OnUnitMatches;

    public void GainedExperience(int amount)
    {
        OnUnitExperience?.Invoke(amount);
    }

    public void GainedLevel(int amount)
    {
        OnUnitLevel?.Invoke(amount);
    }

    public void GainedJobExperience(UnitJob unitJob, int amount)
    {
        OnUnitJobExperience?.Invoke(unitJob, amount);
    }

    public void GainedJobLevel(UnitJob unitJob, int amount)
    {
        OnUnitJobLevel?.Invoke(unitJob, amount);
    }

    public void Load()
    {
        if(loaded)
        {
            return;
        }

        // Re-apply statuses
        List<StatusEffect> curStatuses = new List<StatusEffect>(statusEffects);
        statusEffects.Clear();
        for(int i = 0; i < curStatuses.Count; i++)
        {
            AddStatus(curStatuses[i]);
        }

        // Re-apply equipment
        equipmentBlock.Load(this);
        activeJob.Load(this);

        // Re-apply passives
        foreach(PassiveData passive in assignedPassives)
        {
            passive.Assign(this);
        }

        // Re-apply passives from jobs
        List<PassiveData> jobPassives = GetJobPassives();
        foreach(PassiveData passive in jobPassives)
        {
            if(!assignedPassives.Contains(passive))
            {
                passive.Assign(this);
            }
        }

        loaded = true;
    }

    public void StartTurn()
    {
        // Debug.Log"Started turn: "+this.unitName);
        acted = false;
        moved = false;
        usedBonus = false;
        OnUnitTurnStart?.Invoke();
    }

    public void EndTurn()
    {
        OnUnitTurnEnd?.Invoke();
    }

    public void AddStatus(StatusEffect statusEffect)
    {
        if(statusEffect == null)
        {
            return;
        }
        if(!HasStatus(statusEffect))
        {
            StatusEffect status = Instantiate(statusEffect);
            statusEffects.Add(status);
            status.Apply(this);

            OnAddStatusEffect?.Invoke(status);
        }
    }

    public void RemoveStatus(StatusEffect statusEffect)
    {
       int index = GetStatusIndex(statusEffect);
       if(index > -1)
       {
           StatusEffect status = statusEffects[index];
           statusEffects[index].Remove(this);
           statusEffects.RemoveAt(index);
           OnRemoveStatusEffect?.Invoke(status);
       }
    }

    int GetStatusIndex(StatusEffect statusEffect)
    {
        for(int i = 0; i < statusEffects.Count; i++)
        {
            if(statusEffects[i].statusName == statusEffect.statusName)
            {
                return i;
            }
        }
        return -1;
    }

    public bool HasStatus(StatusEffect statusEffect)
    {
        foreach(StatusEffect _statusEffect in statusEffects)
        {
            if(_statusEffect.statusName == statusEffect.statusName)
            {
                return true;
            }
        }
        return false;
    }

    public int GetHitChance(float baseHitChance, UnitData target, List<ElementData> elements, bool ignoreEvasion = false)
    {
        ModFloat modifiedHitChance = new ModFloat(baseHitChance);
        modifiedHitChance.elements = elements;
        OnCalculateHitChance?.Invoke(this, target, modifiedHitChance);

        int evadeChance = ignoreEvasion ? 0 : target.stats.GetByStat(Stat.EVASION);

        return Mathf.RoundToInt(Mathf.Clamp(modifiedHitChance.GetCalculated() - evadeChance, 0, 100f));
    }

    public void Miss()
    {
        OnUnitMiss?.Invoke();
    }

    public void PushTo(GridCell targetCell)
    {
        OnUnitPush?.Invoke(targetCell, PushComplete);
    }

    public void PushComplete(Vector3Int targetPosition)
    {
        MapManager.Instance.UpdateUnitPosition(targetPosition, this);
    }

    public int PredictDealDamage(int damage, IDamageable target, List<ElementData> elements)
    {
        ModInt modifiedDamage = new ModInt(damage);
        modifiedDamage.elements = elements;
        OnPredictDealDamage?.Invoke(this, target, modifiedDamage);

        return target.PredictReceiveDamage(modifiedDamage.GetCalculated(), this, modifiedDamage.elements);
    }

    public int PredictReceiveDamage(int damage, IDamageable source, List<ElementData> elements)
    {
        ModInt modifiedDamage = new ModInt(damage);
        modifiedDamage.elements = elements;
        OnPredictReceiveDamage?.Invoke(source, this, modifiedDamage);

        return modifiedDamage.GetCalculated();
    }

    public int DealDamage(int damage, IDamageable target, List<ElementData> elements)
    {
        ModInt modifiedDamage = new ModInt(damage);
        modifiedDamage.elements = elements;
        OnDealDamageCalculation?.Invoke(this, target, modifiedDamage);
        int totalDamage = modifiedDamage.GetCalculated();
        return target.ReceiveDamage(totalDamage, this, modifiedDamage.elements);
    }

    public int ReceiveDamage(int damage, IDamageable source, List<ElementData> elements)
    {
        ModInt modifiedDamage = new ModInt(damage);
        modifiedDamage.elements = elements;
        OnReceiveDamageCalculation?.Invoke(source, this, modifiedDamage);
        int totalDamage = modifiedDamage.GetCalculated();

        hp = Mathf.Clamp(hp - totalDamage, 0, stats.CalculateMaxHP());
        OnReceiveDamage?.Invoke(source, this, modifiedDamage.elements, totalDamage);
        return totalDamage;
    }

    public int PredictHealOther(int heal, IDamageable target, List<ElementData> elements)
    {
        ModInt modifiedHeal = new ModInt(heal);
        modifiedHeal.elements = elements;
        OnPredictHealOther?.Invoke(this, target, modifiedHeal);

        return target.PredictReceiveHeal(modifiedHeal.GetCalculated(), this, modifiedHeal.elements);
    }

    public int PredictReceiveHeal(int heal, IDamageable source, List<ElementData> elements)
    {
        ModInt modifiedHeal = new ModInt(heal);
        modifiedHeal.elements = elements;
        OnPredictReceiveHeal?.Invoke(source, this, modifiedHeal);

        int totalHeal = modifiedHeal.GetCalculated();

        return Mathf.Min(stats.CalculateMaxHP() - stats.hp, totalHeal);
    }

    public int HealOther(int heal, IDamageable target, List<ElementData> elements)
    {
        ModInt modifiedHeal = new ModInt(heal);
        modifiedHeal.elements = elements;
        OnHealOtherCalculation?.Invoke(this, target, modifiedHeal);
        int totalHeal = modifiedHeal.GetCalculated();
        return target.ReceiveHeal(totalHeal, this, modifiedHeal.elements);
    }

    public int ReceiveHeal(int heal, IDamageable source, List<ElementData> elements)
    {
        ModInt modifiedHeal = new ModInt(heal);
        modifiedHeal.elements = elements;
        OnReceiveHealCalculation?.Invoke(source, this, modifiedHeal);
        int totalHeal = modifiedHeal.GetCalculated();

        totalHeal = Mathf.Min(stats.CalculateMaxHP() - stats.hp, totalHeal);

        hp = Mathf.Clamp(hp + totalHeal, 0, stats.CalculateMaxHP());
        OnReceiveHeal?.Invoke(source, this, modifiedHeal.elements, totalHeal);
        return totalHeal;
    }

    bool CalculateCanMove()
    {
        if(incapacitated || moved)
        {
            return false;
        }
        ModBool boolMod = new ModBool(true);
        OnCalculateCanMove?.Invoke(boolMod);

        return boolMod.GetCalculated();
    }

    bool CalculateCanAct()
    {
        if(incapacitated)
        {
            return false;
        }
        ModBool boolMod = new ModBool(!acted);
        OnCalculateCanAct?.Invoke(boolMod);

        return boolMod.GetCalculated();
    }

    bool CalculateCanUseBonus()
    {
        if(incapacitated)
        {
            return false;
        }
        ModBool boolMod = new ModBool(!usedBonus);
        OnCalculateCanUseBonus?.Invoke(boolMod);

        return boolMod.GetCalculated();
    }

    bool CalculateIsIncapacitated()
    {
        bool baseIncapacitated = (hp <= 0);
        ModBool boolMod = new ModBool(baseIncapacitated);
        OnCalculateIsIncapacitated?.Invoke(boolMod);

        return boolMod.GetCalculated();
    }

    public void RecalculateResources()
    {
        hp = Mathf.Min(hp, maxHP);
        mp = Mathf.Min(mp, maxMP);
    }

    public void RefreshUnit()
    {
        hp = maxHP;
        mp = maxMP;
    }

    public UnitJob GetUnitJob(JobData job)
    {
        foreach(UnitJob unitJob in availableJobs)
        {
            if(unitJob.jobData == job)
            {
                return unitJob;
            }
        }

        return new UnitJob();
    }

    public List<SkillData> GetJobSkills(bool filterByUsable = false)
    {
        List<SkillData> availableSkills = new List<SkillData>();

        if(activeJob != null)
        {
            UnitJob activeUnitJob = GetUnitJob(activeJob);
            if(activeUnitJob.jobData != null)
            {
                foreach(JobSkill jobSkill in activeUnitJob.jobData.skills)
                {
                    if(jobSkill.learnLevel <= activeUnitJob.level)
                    {
                        if((!filterByUsable || jobSkill.skill.IsUsable(this)) && !availableSkills.Contains(jobSkill.skill))
                        {
                            availableSkills.Add(jobSkill.skill);
                        }
                    }
                }
            }
        }

        return availableSkills;
    }

    public List<PassiveData> GetJobPassives()
    {
        List<PassiveData> availableSkills = new List<PassiveData>();

        if(activeJob != null)
        {
            UnitJob activeUnitJob = GetUnitJob(activeJob);
            if(activeUnitJob.jobData != null)
            {
                foreach(JobPassive jobSkill in activeUnitJob.jobData.passives)
                {
                    if(jobSkill.learnLevel <= activeUnitJob.level && !availableSkills.Contains(jobSkill.skill))
                    {
                        availableSkills.Add(jobSkill.skill);
                    }
                }
            }
        }

        return availableSkills;
    }

    public List<SkillData> GetAvailableSkills(bool filterByUsable = false)
    {
        List<SkillData> availableSkills = new List<SkillData>();
        if(!canAct)
        {
            return availableSkills;
        }

        if(activeJob != null)
        {
            UnitJob activeUnitJob = GetUnitJob(activeJob);
            if(activeUnitJob.jobData != null)
            {
                foreach(JobSkill jobSkill in activeUnitJob.jobData.skills)
                {
                    if(jobSkill.learnLevel <= activeUnitJob.level)
                    {
                        if((!filterByUsable || jobSkill.skill.IsUsable(this)) && !availableSkills.Contains(jobSkill.skill))
                        {
                            availableSkills.Add(jobSkill.skill);
                        }
                    }
                }
            }
        }

        foreach(SkillData skill in assignedSkills)
        {
            if((!filterByUsable || skill.IsUsable(this)) && !availableSkills.Contains(skill))
            {
                availableSkills.Add(skill);
            }
        }
        return availableSkills;
    }

    public List<SkillData> GetAttackSkills(bool filterByUsable = false)
    {
        List<SkillData> availableSkills = new List<SkillData>();
        if(!canAct)
        {
            return availableSkills;
        }

        if(equipmentBlock.weapon != null)
        {
            if(equipmentBlock.weapon.weaponSkill != null && (!filterByUsable || equipmentBlock.weapon.weaponSkill.IsUsable(this)))
            {
                availableSkills.Add(equipmentBlock.weapon.weaponSkill);
            }
        }

        if(equipmentBlock.offhand != null)
        {
            if(equipmentBlock.offhand.weaponSkill != null && (!filterByUsable || equipmentBlock.offhand.weaponSkill.IsUsable(this)))
            {
                availableSkills.Add(equipmentBlock.offhand.weaponSkill);
            }
        }

        return availableSkills;
    }

    public List<IAssignableSkill> GetAllLearnedSkills()
    {
        List<IAssignableSkill> learned = new List<IAssignableSkill>();
        foreach(SkillData skill in learnedSkills)
        {
            learned.Add(skill);
        }
        foreach(PassiveData passive in learnedPassives)
        {
            learned.Add(passive);
        }

        return learned;
    }

    public List<IAssignableSkill> GetAllAssignedSkills()
    {
        List<IAssignableSkill> assigned = new List<IAssignableSkill>();
        foreach(SkillData skill in assignedSkills)
        {
            assigned.Add(skill);
        }
        foreach(PassiveData passive in assignedPassives)
        {
            assigned.Add(passive);
        }

        return assigned;
    }

    public List<SkillData> GetLearnedSkills()
    {
        return learnedSkills;
    }

    public List<SkillData> GetAssignedSkills()
    {
        return assignedSkills;
    }

    public void LearnSkill(SkillData skillData)
    {
        learnedSkills.Add(skillData);
    }

    public bool AssignSkill(IAssignableSkill skillData)
    {
        if((stats.maxSP - stats.sp) >= skillData.spCost)
        {
            stats.sp += skillData.spCost;
            if(skillData is SkillData)
            {
                assignedSkills.Add((SkillData)skillData);
            }
            else
            {
                assignedPassives.Add((PassiveData)skillData);
                ((PassiveData)skillData).Assign(this);
            }
            return true;
        }

        return false;
    }

    public void UnassignSkill(IAssignableSkill skill)
    {
        if(skill == null)
        {
            return;
        }

        if(skill is SkillData && assignedSkills.Contains((SkillData)skill))
        {
            assignedSkills.Remove((SkillData)skill);
            stats.sp -= skill.spCost;
        }
        else if(skill is PassiveData && assignedPassives.Contains((PassiveData)skill))
        {
            ((PassiveData)skill).Unassign(this);
            assignedPassives.Remove((PassiveData)skill);
            stats.sp -= skill.spCost;
        }
    }

    public void LearnPassive(PassiveData passiveData)
    {
        learnedPassives.Add(passiveData);
    }

    public List<PassiveData> GetAssignedPassives()
    {
        return assignedPassives;
    }

    public void GetUnitMatches(ModInt modMatches, List<ElementData> elementsToMatch)
    {
        OnUnitMatches?.Invoke(modMatches, this, elementsToMatch);
    }

    public UnitSaveData GetSaveData()
    {
        UnitSaveData saveData = new UnitSaveData();
        saveData.id = _id;
        saveData.name = _unitName;
        saveData.arcanaID = _arcana.id;
        saveData.unitModelID = _unitModel.GetComponent<UnitModel>().id;
        saveData.faction = _faction.ToString();
        if(_aiBrain != null)
        {
            saveData.aiBrainID = _aiBrain.id;
        }
        saveData.statBlock = _stats.GetSaveData();
        saveData.equipmentBlock = _equipmentBlock.GetSaveData();
        saveData.baseJobID = _baseJob.id;
        saveData.activeJobID = _activeJob.id;

        saveData.availableJobs = new UnitJobSaveData[_availableJobs.Count];
        for(int i = 0; i < _availableJobs.Count; i++)
        {
            saveData.availableJobs[i] = _availableJobs[i].GetSaveData();
        }

        saveData.ct = ct;
        saveData.moved = moved;
        saveData.acted = acted;
        saveData.usedBonus = usedBonus;

        if(_sprite != null) {
            saveData.spritePath = _sprite.name;
        }

        saveData.learnedSkillIDs = new string[learnedSkills.Count];
        for(int i = 0; i < learnedSkills.Count; i++)
        {
            saveData.learnedSkillIDs[i] = learnedSkills[i].id;
        }

        saveData.assignedSkillIDs = new string[assignedSkills.Count];
        for(int i = 0; i < assignedSkills.Count; i++)
        {
            saveData.assignedSkillIDs[i] = assignedSkills[i].id;
        }

        saveData.curPosition = new SimpleVector3Int(curPosition.x, curPosition.y, curPosition.z);
        
        saveData.statusEffectIDs = new string[statusEffects.Count];
        for(int i = 0; i < statusEffects.Count; i++)
        {
            if(statusEffects[i] != null)
            {
                saveData.statusEffectIDs[i] = statusEffects[i].id;
            }
        }
        
        return saveData;
    }

    public bool LoadFromSaveData(UnitSaveData saveData)
    {
        _id = saveData.id;
        _unitName = saveData.name;
        _arcana = SaveDataLoader.Instance.GetArcanaData(saveData.arcanaID);
        _unitModel = SaveDataLoader.Instance.GetUnitModel(saveData.unitModelID);
        _faction = (Faction)System.Enum.Parse(typeof(Faction), saveData.faction);
        if(saveData.aiBrainID != "")
        {
            _aiBrain = SaveDataLoader.Instance.GetAIBrain(saveData.aiBrainID);
        }
        _stats.LoadFromSaveData(saveData.statBlock);
        _equipmentBlock.LoadFromSaveData(saveData.equipmentBlock);
        _baseJob = SaveDataLoader.Instance.GetJobData(saveData.baseJobID);
        _activeJob = SaveDataLoader.Instance.GetJobData(saveData.activeJobID);
        
        _availableJobs.Clear();
        foreach(UnitJobSaveData unitJobSaveData in saveData.availableJobs)
        {
            UnitJob unitJob = new UnitJob();
            unitJob.LoadFromSaveData(unitJobSaveData);
            _availableJobs.Add(unitJob);
        }

        ct = saveData.ct;
        moved = saveData.moved;
        acted = saveData.acted;
        usedBonus = saveData.usedBonus;
        _sprite = Resources.Load<Sprite>("Units/Portraits/" + saveData.spritePath);
        learnedSkills.Clear();
        foreach(string skillID in saveData.learnedSkillIDs)
        {
            SkillData skill = SaveDataLoader.Instance.GetSkillData(skillID);
            if(skill != null)
            {
                learnedSkills.Add(skill);
            }
        }

        assignedSkills.Clear();
        foreach(string skillID in saveData.assignedSkillIDs)
        {
            SkillData skill = SaveDataLoader.Instance.GetSkillData(skillID);
            if(skill != null)
            {
                assignedSkills.Add(skill);
            }
        }

        curPosition = new Vector3Int(saveData.curPosition.x, saveData.curPosition.y, saveData.curPosition.z);
        
        foreach(string statusID in saveData.statusEffectIDs)
        {
            StatusEffect statusEffect = SaveDataLoader.Instance.GetStatusEffect(statusID);
            if(statusEffect != null)
            {
                statusEffects.Add(Instantiate(statusEffect));
            }
        }

        return true;
    }
}
