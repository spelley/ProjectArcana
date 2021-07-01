using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public abstract class SkillData: ScriptableObject
{
    [SerializeField]
    string _skillName;
    public string skillName
    { 
        get
        {
            return _skillName;
        } 
        private set
        {
            _skillName = value;
        }
    }

    [SerializeField]
    string _description;
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

    [Header("Resource Costs")]
    [SerializeField]
    ActionType _actionType;
    public ActionType actionType
    {
        get { return _actionType; }
        private set { _actionType = value; }
    }
    [SerializeField]
    int _hpCost;
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
    int _mpCost;
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
    [SerializeField]
    List<ElementData> _elements = new List<ElementData>();
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

    [SerializeField]
    List<MatchType> _matchTypes = new List<MatchType>();
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
    [SerializeField]
    BattleAnimation _castAnimation;
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

    [SerializeField]
    GameObject _executeAnimation;
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
    [SerializeField, Range(0f, 100f)]
    float _hitChance = 100f;
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

    [SerializeField]
    TargetType _targetType;
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

    [SerializeField]
    TargetShape _targetShape;
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

    [SerializeField]
    RangeType _rangeType;
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

    [SerializeField]
    int _range;
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

    [SerializeField]
    int _height;
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

    [SerializeField]
    int _heightTolerance;
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

    [SerializeField]
    int _areaOfEffect;
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

    [SerializeField]
    int _push = 0;
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

    [SerializeField]
    bool _pushFromTarget;
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

    [SerializeField]
    bool _requireUnitTarget = true;
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
    [SerializeField]
    SkillCalculation _skillCalculation;
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

    [SerializeField]
    Stat _primaryAttribute;
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

    [SerializeField]
    Stat _secondaryAttribute;
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

    [SerializeField]
    Stat _tertiaryAttribute;
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

    [SerializeField]
    int _primaryValue;
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

    [SerializeField]
    int _secondaryValue;
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

    [SerializeField]
    int _tertiaryValue;
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

    void TryAddOffset(Vector3Int targetPosition, Vector3Int offset, List<Vector3Int> offsets)
    {
        if(MapManager.Instance.CheckIfTargetable(targetPosition.x + offset.x, targetPosition.y + offset.y, targetPosition.z + offset.z))
        {
            offsets.Add(offset);
        }
    }

    public List<Vector3Int> GetTargetOffsets(Vector3Int targetPosition)
    {
        List<Vector3Int> offsets = new List<Vector3Int>();
        switch(targetShape)
        {
            case TargetShape.SINGLE:
                TryAddOffset(targetPosition, new Vector3Int(0, 0, 0), offsets);
            break;
            case TargetShape.ALL:
                if(rangeType == RangeType.LINE)
                {
                    TryAddOffset(targetPosition, new Vector3Int(0, 0, 0), offsets);
                    for(int r = 1; r < range; r++)
                    {
                        for(int h = 0; h <= heightTolerance; h++)
                        {
                            TryAddOffset(targetPosition, new Vector3Int(r, 0, h), offsets);
                            TryAddOffset(targetPosition, new Vector3Int(-r, 0, h), offsets);
                            TryAddOffset(targetPosition, new Vector3Int(0, r, h), offsets);
                            TryAddOffset(targetPosition, new Vector3Int(0, -r, h), offsets);
                            if(h > 0)
                            {
                                TryAddOffset(targetPosition, new Vector3Int(r, 0, -h), offsets);
                                TryAddOffset(targetPosition, new Vector3Int(-r, 0, -h), offsets);
                                TryAddOffset(targetPosition, new Vector3Int(0, r, -h), offsets);
                                TryAddOffset(targetPosition, new Vector3Int(0, -r, -h), offsets);
                            }
                        }
                    }
                }
                else
                {
                    // we don't use offsets for "ALL" target shapes
                    TryAddOffset(targetPosition, new Vector3Int(0, 0, 0), offsets);
                }
            break;
            case TargetShape.CROSS:
                TryAddOffset(targetPosition, new Vector3Int(0, 0, 0), offsets);
                for(int a = 1; a <= areaOfEffect; a++)
                {
                    int targetXA = a;
                    int targetXB = -a;
                    for(int i = 0; i <= heightTolerance; i++)
                    {
                        TryAddOffset(targetPosition, new Vector3Int(targetXA, 0, i), offsets);
                        TryAddOffset(targetPosition, new Vector3Int(0, targetXA, i), offsets);
                        TryAddOffset(targetPosition, new Vector3Int(targetXB, 0, i), offsets);
                        TryAddOffset(targetPosition, new Vector3Int(0, targetXB, i), offsets);
                        if(i != 0)
                        {
                            TryAddOffset(targetPosition, new Vector3Int(targetXA, 0, -i), offsets);
                            TryAddOffset(targetPosition, new Vector3Int(0, targetXA, -i), offsets);
                            TryAddOffset(targetPosition, new Vector3Int(targetXB, 0, -i), offsets);
                            TryAddOffset(targetPosition, new Vector3Int(0, targetXB, -i), offsets);
                        }
                    }
                }
            break;
        }
        return offsets;
    }

    public List<Vector3Int> GetTargetShape()
    {
        List<Vector3Int> offsets = new List<Vector3Int>();
        switch(targetShape)
        {
            case TargetShape.SINGLE:
                offsets.Add(new Vector3Int(0, 0, 0));
            break;
            case TargetShape.ALL:
                if(rangeType == RangeType.LINE)
                {
                    offsets.Add(new Vector3Int(0, 0, 0));
                    for(int r = 1; r < range; r++)
                    {
                        for(int h = 0; h <= heightTolerance; h++)
                        {
                            offsets.Add(new Vector3Int(r, 0, h));
                            offsets.Add(new Vector3Int(-r, 0, h));
                            offsets.Add(new Vector3Int(0, r, h));
                            offsets.Add(new Vector3Int(0, -r, h));
                            if(h > 0)
                            {
                                offsets.Add(new Vector3Int(r, 0, -h));
                                offsets.Add(new Vector3Int(-r, 0, -h));
                                offsets.Add(new Vector3Int(0, r, -h));
                                offsets.Add(new Vector3Int(0, -r, -h));
                            }
                        }
                    }
                }
                else
                {
                    // we don't use offsets for "ALL" target shapes
                    offsets.Add(new Vector3Int(0, 0, 0));
                }
            break;
            case TargetShape.CROSS:
                offsets.Add(new Vector3Int(0, 0, 0));
                for(int a = 1; a <= this.areaOfEffect; a++)
                {
                    int targetXA = a;
                    int targetXB = -a;
                    for(int i = 0; i <= heightTolerance; i++)
                    {
                        offsets.Add(new Vector3Int(targetXA, 0, i));
                        offsets.Add(new Vector3Int(0, targetXA, i));
                        offsets.Add(new Vector3Int(targetXB, 0, i));
                        offsets.Add(new Vector3Int(0, targetXB, i));
                        if(i != 0)
                        {
                            offsets.Add(new Vector3Int(targetXA, 0, -i));
                            offsets.Add(new Vector3Int(0, targetXA, -i));
                            offsets.Add(new Vector3Int(targetXB, 0, -i));
                            offsets.Add(new Vector3Int(0, targetXB, -i));
                        }
                    }
                }
            break;
        }
        return offsets;
    }

    public List<Vector3Int> GetTargetShape(GridCell originCell, GridCell targetCell, List<GridCell> targetableArea)
    {
        List<Vector3Int> offsets = new List<Vector3Int>();
        switch(targetShape)
        {
            case TargetShape.SINGLE:
                offsets.Add(new Vector3Int(0, 0, 0));
            break;
            case TargetShape.ALL:
                if(rangeType == RangeType.LINE)
                {
                    if(IsSelfTargeting())
                    {
                        offsets.Add(originCell.position - targetCell.position);
                    }
                    foreach(GridCell gridCell in targetableArea)
                    {
                        if(Mathf.Abs(gridCell.position.z - originCell.position.z) > heightTolerance)
                        {
                            continue;
                        }

                        if(targetCell.position.x > originCell.position.x
                            && targetCell.position.y == originCell.position.y
                            && gridCell.position.y == originCell.position.y
                            && gridCell.position.x > originCell.position.x)
                        {
                            offsets.Add(new Vector3Int(gridCell.position.x - targetCell.position.x, 0, gridCell.position.z - targetCell.position.z));
                        }
                        else if(targetCell.position.x < originCell.position.x 
                            && targetCell.position.y == originCell.position.y
                            && gridCell.position.y == originCell.position.y 
                            && gridCell.position.x < originCell.position.x)
                        {
                            offsets.Add(new Vector3Int(gridCell.position.x - targetCell.position.x, 0, gridCell.position.z - targetCell.position.z));
                        }
                        else if(targetCell.position.y > originCell.position.y
                            && targetCell.position.x == originCell.position.x
                            && gridCell.position.x == originCell.position.x 
                            && gridCell.position.y > originCell.position.y)
                        {
                            offsets.Add(new Vector3Int(0, gridCell.position.y - targetCell.position.y, gridCell.position.z - targetCell.position.z));
                        }
                        else if(targetCell.position.y < originCell.position.y
                            && targetCell.position.x == originCell.position.x
                            && gridCell.position.x == originCell.position.x
                            && gridCell.position.y < originCell.position.y)
                        {
                            offsets.Add(new Vector3Int(0, gridCell.position.y - targetCell.position.y, gridCell.position.z - targetCell.position.z));
                        }
                    }
                }
                else
                {
                    foreach(GridCell gridCell in targetableArea)
                    {
                        offsets.Add(gridCell.position - targetCell.position);
                    }
                }
            break;
            case TargetShape.CROSS:
                offsets.Add(new Vector3Int(0, 0, 0));
                for(int a = 1; a <= this.areaOfEffect; a++)
                {
                    int targetXA = a;
                    int targetXB = -a;
                    for(int i = 0; i <= heightTolerance; i++)
                    {
                        offsets.Add(new Vector3Int(targetXA, 0, i));
                        offsets.Add(new Vector3Int(0, targetXA, i));
                        offsets.Add(new Vector3Int(targetXB, 0, i));
                        offsets.Add(new Vector3Int(0, targetXB, i));
                        if(i != 0)
                        {
                            offsets.Add(new Vector3Int(targetXA, 0, -i));
                            offsets.Add(new Vector3Int(0, targetXA, -i));
                            offsets.Add(new Vector3Int(targetXB, 0, -i));
                            offsets.Add(new Vector3Int(0, targetXB, -i));
                        }
                    }
                }
            break;
        }

        if(!IsSelfTargeting())
        {
            Vector3Int selfOffset = originCell.position - targetCell.position;
            int idx = offsets.IndexOf(selfOffset);
            if(idx > -1)
            {
                offsets.RemoveAt(idx);
            }
        }
        return offsets;
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

    public bool IsValidTargetType(UnitData unitData, UnitData target)
    {
        if(target.incapacitated)
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

    public void Initiate(UnitData unitData)
    {
        // TODO: implement default initiate
    }
    public void Cancel(UnitData unitData)
    {
        // TODO: implement default cancel
    }
    public abstract void Execute(UnitData unitData, List<GridCell> targets);

    public abstract void ExecutePerTarget(UnitData unitData, GridCell gridCell);

    public abstract int GetSkillScore(UnitData unitData, GridCell gridCell);

    public abstract int GetSkillScore(UnitData unitData, UnitData targetUnit);

    public abstract SkillPreview GetPreview(UnitData unitData, GridCell target);

    public SkillStruct GetSkillStruct()
    {
        return new SkillStruct(range, areaOfEffect, heightTolerance, GetShapeRangeExtension(), rangeType, targetType, targetShape, actionType);
    }

    public void ResolveSkill()
    {
        BattleManager.Instance.SkillClear();
    }
}