using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Custom Data/Unit Data", order = 2)]
public class UnitData : ScriptableObject, ITurnTaker, IDamageable
{
    public string unitName;
    public ArcanaData arcana;
    public GameObject unitModel;
    public GameObject unitGO { get; set; }
    public Faction faction;
    public AIBrain aiBrain;
    public StatBlock stats;
    public EquipmentBlock equipmentBlock;
    public JobData baseJob;
    public JobData activeJob;
    public List<UnitJob> availableJobs = new List<UnitJob>();

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

    public List<SkillData> learnedSkills = new List<SkillData>();
    public List<SkillData> assignedSkills = new List<SkillData>();
    public Vector3Int curPosition { get; set; }
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
    
    public event Action<ModBool> OnCalculateIsIncapacitated;
    public event Action<ModBool> OnCalculateCanMove;
    public event Action<ModBool> OnCalculateCanAct;

    public event Action<StatusEffect> OnAddStatusEffect;
    public event Action<StatusEffect> OnRemoveStatusEffect;

    public event Action<int> OnUnitExperience;
    public event Action<int> OnUnitLevel;
    public event Action<UnitJob, int> OnUnitJobExperience;
    public event Action<UnitJob, int> OnUnitJobLevel;

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
        // Re-apply statuses
        List<StatusEffect> curStatuses = new List<StatusEffect>(statusEffects);
        statusEffects.Clear();
        for(int i = 0; i < curStatuses.Count; i++)
        {
            AddStatus(curStatuses[i]);
        }

        // Re-apply equipment
        equipmentBlock.Load(this);
    }

    public void StartTurn()
    {
        Debug.Log("Started turn: "+this.unitName);
        acted = false;
        moved = false;
        OnUnitTurnStart?.Invoke();
    }

    public void EndTurn()
    {
        OnUnitTurnEnd?.Invoke();
    }

    public void AddStatus(StatusEffect statusEffect)
    {
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

    public List<SkillData> GetAvailableSkills(bool filterByUsable = false)
    {
        List<SkillData> availableSkills = new List<SkillData>();
        if(!canAct)
        {
            return availableSkills;
        }

        if(baseJob != null)
        {
            UnitJob unitJob = GetUnitJob(baseJob);
            if(unitJob.jobData != null)
            {
                foreach(JobSkill jobSkill in unitJob.jobData.skills)
                {
                    if(jobSkill.learnLevel <= unitJob.level)
                    {
                        if((!filterByUsable || jobSkill.skill.IsUsable(this)) && !availableSkills.Contains(jobSkill.skill))
                        {
                            availableSkills.Add(jobSkill.skill);
                        }
                    }
                }
            }
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
}
