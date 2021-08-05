using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

[CreateAssetMenu(fileName = "SkillData", menuName = "Custom Data/Skill Data", order = 0)]
public class SkillData: ScriptableObject, IAssignableSkill, ILoadable<SkillSaveData>
{
    [SerializeField] string _id;
    public string id { get { return _id; } }
    string _loadType = "Skill";
    public virtual string loadType { get { return _loadType; } }

    [SerializeField] string _skillName;
    public string skillName
    { 
        get
        {
            return _skillName;
        }
    }

    [SerializeField] int _spCost;
    public int spCost { get { return _spCost; } }

    [SerializeField] string _description;
    public string description
    { 
        get
        {
            return _description;
        } 
        private set
        {
            _description = value;
        }
    }

    [SerializeField] SkillEffect _skillEffect;
    public SkillEffect skillEffect { get { return _skillEffect; } }

    [Header("Resource Costs")]
    [SerializeField] ActionType _actionType;
    public ActionType actionType
    {
        get { return _actionType; }
        private set { _actionType = value; }
    }

    [SerializeField] int _hpCost;
    public int hpCost
    {
        get
        {
            return _hpCost;
        }
        private set
        {
            _hpCost = value;
        }
    }
    [SerializeField] int _mpCost;
    public int mpCost
    {
        get
        {
            return _mpCost;
        }
        private set
        {
            _mpCost = value;
        }
    }

    [Header("Element Information")]
    [SerializeField] List<ElementData> _elements = new List<ElementData>();
    public List<ElementData> elements
    {
        get
        {
            return _elements;
        }
        private set
        {
            _elements = value;
        }
    }

    [SerializeField] List<MatchType> _matchTypes = new List<MatchType>();
    public List<MatchType> matchTypes
    {
        get
        {
            return _matchTypes;
        }
        private set
        {
            _matchTypes = value;
        }
    }

    [Header("Animation Information")]
    [SerializeField] BattleAnimation _castAnimation;
    public BattleAnimation castAnimation
    { 
        get
        {
            return _castAnimation;
        } 
        private set
        {
            _castAnimation = value;
        }
    }

    [SerializeField] GameObject _executeAnimation;
    public GameObject executeAnimation
    {
        get
        {
            return _executeAnimation;
        }
        private set
        {
            _executeAnimation = value;
        }
    }
    
    [Header("Targeting Information")]
    [SerializeField, Range(0f, 100f)] float _hitChance = 100f;
    public float hitChance
    {
        get
        {
            return _hitChance + (5f * (matchTypes.Contains(MatchType.HIT_CHANCE) ? BattleManager.Instance.GetRiverMatches(this.elements) : 0));
        }
        private set
        {
            _hitChance = value;
        }
    }

    [SerializeField] TargetType _targetType;
    public TargetType targetType
    {
        get
        {
            return _targetType;
        }
        private set
        {
            _targetType = value;
        }
    }

    [SerializeField] TargetShape _targetShape;
    public TargetShape targetShape
    {
        get
        {
            return _targetShape;
        }
        private set
        {
            _targetShape = value;
        }
    }

    [SerializeField] RangeType _rangeType;
    public RangeType rangeType
    {
        get
        {
            return _rangeType;
        }
        private set
        {
            _rangeType = value;
        }
    }

    [SerializeField] int _range;
    public int range
    {
        get
        {
            return _range + (matchTypes.Contains(MatchType.RANGE) ? BattleManager.Instance.GetRiverMatches(this.elements) : 0);
        }
        private set
        {
            _range = value;
        }
    }

    [SerializeField] int _height;
    public int height
    {
        get
        {
            return _height;
        }
        private set
        {
            _height = height;
        }
    }

    [SerializeField] int _heightTolerance;
    public int heightTolerance
    {
        get
        {
            return _heightTolerance;
        }
        private set
        {
            _heightTolerance = value;
        }
    }

    [SerializeField] int _areaOfEffect;
    public int areaOfEffect
    {
        get
        {
            return _areaOfEffect + (matchTypes.Contains(MatchType.AREA) ? BattleManager.Instance.GetRiverMatches(this.elements) : 0);
        }
        private set
        {
            _areaOfEffect = value;
        }
    }

    [SerializeField] int _push = 0;
    public int push
    {
        get
        {
            return _push;
        }

        private set
        {
            _push = value;
        }
    }

    [SerializeField] bool _pushFromTarget;
    public bool pushFromTarget
    {
        get
        {
            return _pushFromTarget;
        }
        private set
        {
            _pushFromTarget = value;
        }
    }

    [SerializeField] bool _requireUnitTarget = true;
    public bool requireUnitTarget
    {
        get
        {
            return _requireUnitTarget;
        }
        private set
        {
            _requireUnitTarget = value;
        }
    }

    [Header("Skill Calculation Information")]
    [SerializeField] SkillCalculation _skillCalculation;
    public SkillCalculation skillCalculation
    {
        get
        {
            return _skillCalculation;
        }
        private set
        {
            _skillCalculation = value;
        }
    }

    [SerializeField] Stat _primaryAttribute;
    public Stat primaryAttribute
    {
        get
        {
            return _primaryAttribute;
        }
        private set
        {
            _primaryAttribute = value;
        }
    }

    [SerializeField] Stat _secondaryAttribute;
    public Stat secondaryAttribute
    {
        get
        {
            return _secondaryAttribute;
        }
        private set
        {
            _secondaryAttribute = value;
        }
    }

    [SerializeField] Stat _tertiaryAttribute;
    public Stat tertiaryAttribute
    {
        get
        {
            return _tertiaryAttribute;
        }
        private set
        {
            _tertiaryAttribute = value;
        }
    }

    [SerializeField] int _primaryValue;
    public int primaryValue
    {
        get
        {
            return _primaryValue + (matchTypes.Contains(MatchType.PRIMARY_BOOST) ? BattleManager.Instance.GetRiverMatches(this.elements) : 0);
        }
        private set
        {
            _primaryValue = value;
        }
    }

    [SerializeField] int _secondaryValue;
    public int secondaryValue
    {
        get
        {
            return _secondaryValue + (matchTypes.Contains(MatchType.SECONDARY_BOOST) ? BattleManager.Instance.GetRiverMatches(this.elements) : 0);;
        }
        private set
        {
            _secondaryValue = value;
        }
    }

    [SerializeField] int _tertiaryValue;
    public int tertiaryValue
    {
        get
        {
            return _tertiaryValue + (matchTypes.Contains(MatchType.TERTIARY_BOOST) ? BattleManager.Instance.GetRiverMatches(this.elements) : 0);
        }
        private set
        {
            _tertiaryValue = value;
        }
    }

    public List<UnitData> executedOn = new List<UnitData>();

    public int GetHitChance(UnitData source, UnitData target, bool ignoreEvasion = false)
    {
        return source.GetHitChance(this.hitChance, target, this.elements);
    }

    public bool IsHit(int hitChance)
    {
        return UnityEngine.Random.Range(0, 100) <= hitChance;
    }

    public void HandlePush(UnitData source, UnitData target)
    {
        if(push != 0)
        {
            GridCell targetCell = MapManager.Instance.GetForcedMovement(source.curPosition, target.curPosition, push);
            target.PushTo(targetCell);
        }
    }

    public int GetShapeRangeExtension()
    {
        switch(targetShape)
        {
            case TargetShape.CROSS:
                return 1 + areaOfEffect;
        }
        return 0;
    }

    public bool IsValidTargetType(UnitData unitData, UnitData target, bool cantBeIncapacitated = true)
    {
        if(cantBeIncapacitated && target.incapacitated)
        {
            return false;
        }
        if(targetType == TargetType.ALL)
        {
            return true;
        }
        if(targetType == TargetType.SELF && unitData != target)
        {
            return false;
        }
        if(targetType == TargetType.ENEMY && unitData.faction == target.faction)
        {
            return false;
        }
        if(unitData.faction != target.faction && 
            (targetType == TargetType.ALLIES
            || targetType == TargetType.SELF_AND_ALLIES))
        {
            return false;
        }
        return true;
    }

    public bool IsUsable(UnitData unitData)
    {
        if(unitData.hp > hpCost && unitData.mp >= mpCost)
        {
            if(actionType == ActionType.MOVE && unitData.canMove)
            {
                return true;
            }
            else if(actionType == ActionType.STANDARD && unitData.canAct)
            {
                return true;
            }
            else if(actionType == ActionType.BONUS && unitData.canUseBonus)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsSelfTargeting()
    {
        List<TargetType> selfTargeting = new List<TargetType>();
        selfTargeting.Add(TargetType.SELF);
        selfTargeting.Add(TargetType.SELF_AND_ALLIES);
        selfTargeting.Add(TargetType.ALL);
        return selfTargeting.Contains(targetType);
    }

    public virtual void Execute(UnitData unitData, List<GridCell> targets)
    {
        if(skillEffect != null)
        {
            skillEffect.Execute(this, unitData, targets);
        }
    }

    public virtual void ExecutePerTarget(UnitData unitData, GridCell gridCell)
    {
        if(skillEffect != null)
        {
            skillEffect.ExecutePerTarget(this, unitData, gridCell);
        }
    }

    public virtual int GetSkillScore(UnitData unitData, GridCell gridCell)
    {
        return 0;
    }

    public virtual int GetSkillScore(UnitData unitData, UnitData targetUnit)
    {
        return 0;
    }

    public virtual SkillPreview GetPreview(UnitData unitData, GridCell target)
    {
        if(skillEffect != null)
        {
            return skillEffect.GetPreview(this, unitData, target);
        }
        return new SkillPreview("No Effect", 0);
    }

    public virtual SkillStruct GetSkillStruct()
    {
        return new SkillStruct(range, areaOfEffect, heightTolerance, GetShapeRangeExtension(), rangeType, targetType, targetShape, actionType);
    }

    public virtual void ResolveSkill()
    {
        BattleManager.Instance.SkillClear();
    }

    public virtual SkillSaveData GetSaveData()
    {
        SkillSaveData saveData = new SkillSaveData();
        saveData.id = _id;
        saveData.loadType = loadType;
        saveData.name = _skillName;
        saveData.description = _description;
        saveData.spCost = _spCost;
        saveData.actionType = _actionType.ToString();
        if(_skillEffect != null)
        {
            saveData.skillEffectID = _skillEffect.id;
        }
        saveData.hpCost = _hpCost;
        saveData.mpCost = _mpCost;
        
        saveData.elementIDs = new string[_elements.Count];
        for(int i = 0; i < _elements.Count; i++)
        {
            saveData.elementIDs[i] = elements[i].id;
        }

        saveData.matchTypes = new string[_matchTypes.Count];
        for(int i = 0; i < _matchTypes.Count; i++)
        {
            saveData.matchTypes[i] = _matchTypes[i].ToString();
        }
        saveData.castAnimation = _castAnimation.ToString();
        
        if(_executeAnimation != null)
        {
            saveData.executeAnimation = _executeAnimation.GetComponent<SkillAnimation>().id;
        }

        saveData.hitChance = Mathf.RoundToInt(_hitChance);
        saveData.targetType = _targetType.ToString();
        saveData.targetShape = _targetShape.ToString();
        saveData.rangeType = _rangeType.ToString();
        saveData.range = _range;
        saveData.height = _height;
        saveData.heightTolerance = _heightTolerance;
        saveData.areaOfEffect = _areaOfEffect;
        saveData.push = _push;
        saveData.pushFromTarget = _pushFromTarget;
        saveData.requireUnitTarget = _requireUnitTarget;
        saveData.skillCalculation = skillCalculation.id;
        saveData.primaryAttribute = _primaryAttribute.ToString();
        saveData.secondaryAttribute = _secondaryAttribute.ToString();
        saveData.tertiaryAttribute = _tertiaryAttribute.ToString();
        saveData.primaryValue = _primaryValue;
        saveData.secondaryValue = _secondaryValue;
        saveData.tertiaryValue = _tertiaryValue;
        return saveData;
    }

    public virtual bool LoadFromSaveData(SkillSaveData saveData)
    {
        if(saveData.skillEffectID != "")
        {
            SkillEffect effect = SaveDataLoader.Instance.GetSkillEffect(saveData.skillEffectID);
            if(effect != null)
            {
                _skillEffect = effect;
            }
            else
            {
                return false;
            }
        }

        _id = saveData.ID;
        _skillName = saveData.name;
        _description = saveData.description;
        _spCost = saveData.spCost;
        _actionType = (ActionType)System.Enum.Parse(typeof(ActionType), saveData.actionType);
        _hpCost = saveData.hpCost;
        _mpCost = saveData.mpCost;

        _elements.Clear();
        for(int i = 0; i < saveData.elementIDs.Length; i++)
        {
            ElementData element = SaveDataLoader.Instance.GetElementData(saveData.elementIDs[i]);
            if(element != null)
            {
                _elements.Add(element);
            }
        }

        _matchTypes.Clear();
        for(int i = 0; i < saveData.matchTypes.Length; i++)
        {
            _matchTypes.Add((MatchType)System.Enum.Parse(typeof(MatchType), saveData.matchTypes[i]));
        }

        _castAnimation = (BattleAnimation)System.Enum.Parse(typeof(BattleAnimation), saveData.castAnimation);

        if(saveData.executeAnimation != "")
        {
            _executeAnimation = SaveDataLoader.Instance.GetExecuteAnimation(saveData.executeAnimation);
        }

        _hitChance = saveData.hitChance;
        _targetType = (TargetType)System.Enum.Parse(typeof(TargetType), saveData.targetType);
        _targetShape = (TargetShape)System.Enum.Parse(typeof(TargetShape), saveData.targetShape);
        _rangeType = (RangeType)System.Enum.Parse(typeof(RangeType), saveData.rangeType);
        _range = saveData.range;
        _height = saveData.height;
        _heightTolerance = saveData.heightTolerance;
        _areaOfEffect = saveData.areaOfEffect;
        _push = saveData.push;
        _pushFromTarget = saveData.pushFromTarget;
        _requireUnitTarget = saveData.requireUnitTarget;
        _skillCalculation = SaveDataLoader.Instance.GetSkillCalculation(saveData.skillCalculation);
        _primaryAttribute = (Stat)System.Enum.Parse(typeof(Stat), saveData.primaryAttribute);
        _secondaryAttribute = (Stat)System.Enum.Parse(typeof(Stat), saveData.secondaryAttribute);
        _tertiaryAttribute = (Stat)System.Enum.Parse(typeof(Stat), saveData.tertiaryAttribute);
        _primaryValue = saveData.primaryValue;
        _secondaryValue = saveData.secondaryValue;
        _tertiaryValue = saveData.tertiaryValue;

        return true;
    }
}