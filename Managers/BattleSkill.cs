using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSkill
{
    public string instanceID { get; private set; }
    public SkillData skill { get; private set; }
    public SkillData skillData
    {
        get
        {
            return skill;
        }
    }
    public UnitData source { get; private set; }
    public UnitData unitData
    {
        get
        {
            return source;
        }
    }
    public List<GridCell> targetableArea { get; private set; }
    public List<GridCell> targetedArea { get; private set; }
    public List<GridCell> targets
    {
        get
        {
            return targetedArea;
        }
    }
    public GridCell skillOrigin { get; private set; }
    public GridCell targetOrigin { get; private set; }
    public Action completionCallback { get; private set; }
    public bool payCosts { get; private set; }
    public bool isTargeting { get; private set; }
    public bool isConfirmed { get; private set; }
    public bool isExecuting { get; private set; }
    public bool isCompleted { get; private set; }

    MapManager mapManager;

    public BattleSkill(SkillData initialSkill, UnitData unit, GridCell origin, bool payForSkill = true)
    {
        mapManager = MapManager.Instance;
        instanceID = Guid.NewGuid().ToString();
        InitializeTargets();
        SetSource(unit);
        SetSkill(initialSkill);
        SetCosts(payForSkill);
        SetSkillOrigin(origin);
        UpdateTargetableArea();

        isTargeting = false;
        isConfirmed = false;
        isExecuting = false;
        isCompleted = false;
    }

    public BattleSkill(SkillData initialSkill, UnitData unit, List<GridCell> initialTargets)
    {
        mapManager = MapManager.Instance;
        instanceID = Guid.NewGuid().ToString();
        InitializeTargets();
        SetSource(unit);
        SetSkill(initialSkill);
        SetCosts(payCosts);
        SetSkillOrigin(mapManager.GetCell(unit.curPosition));
        UpdateTargetableArea();
        SetTargetedArea(initialTargets);

        isTargeting = false;
        isConfirmed = false;
        isExecuting = false;
        isCompleted = false;
    }

    void InitializeTargets()
    {
        targetableArea = new List<GridCell>();
        targetedArea = new List<GridCell>();
    }

    public void SetSource(UnitData unit)
    {
        source = unit;
    }

    public void SetSkill(SkillData newSkill)
    {
        skill = newSkill;
    }

    public void SetCosts(bool payForSkill)
    {
        payCosts = payForSkill;
    }

    public void SetSkillOrigin(GridCell origin)
    {
        skillOrigin = origin;
    }

    public void StartTargeting()
    {
        isTargeting = true;
        UpdateTargetableArea();
    }

    public void EndTargeting()
    {
        isTargeting = false;
        UpdateTargetableArea();
    }

    public void UpdateTargetableArea()
    {
        List<GridCell> cells = mapManager.GetTargetableCells(skill, skillOrigin);
        SetTargetableArea(cells);
    }

    void SetTargetableArea(List<GridCell> newTargetableArea)
    {
        targetableArea.Clear();
        targetableArea.AddRange(newTargetableArea);
    }

    public void SetTarget(GridCell target = null)
    {
        targetOrigin = target;
        if(target != null)
        {
            SetTargetedArea(mapManager.GetTargetedArea(skill, skillOrigin, targetOrigin, targetableArea));
        }
        else
        {
            SetTargetedArea(new List<GridCell>());
        }
        isTargeting = false;
    }

    void SetTargetedArea(List<GridCell> newTargetedArea)
    {
        targetedArea.Clear();
        //targetedArea.AddRange(newTargetedArea);
        Vector3Int pos = (skill.targetShape == TargetShape.ALL) ? skillOrigin.position : targetOrigin.position;
        if(skill.push > 0)
        {
            targetedArea = newTargetedArea.OrderByDescending(gc => ManhattanDistance(pos, gc.position)).ToList();
        }
        else
        {
            targetedArea = newTargetedArea.OrderBy(gc => ManhattanDistance(pos, gc.position)).ToList();
        }
    }

    public void Confirm()
    {
        // need at least the skill, the source of the skill and the targeted area
        if(skill == null || source == null || targetedArea.Count == 0)
        {
            return;
        }

        isTargeting = false;
        isConfirmed = true;

        skill.executedOn.Clear();
        if(skill.executeAnimation != null)
        {
            GameObject skillAnimGO = GameObject.Instantiate(skill.executeAnimation);
            skillAnimGO.GetComponent<BattleSkillAnimation>().SetSkill(this);
        }
    }

    public void PayCosts()
    {
        if(!payCosts)
        {
            return;
        }

        source.hp -= skill.hpCost;
        source.mp -= skill.mpCost;

        switch(skill.actionType)
        {
            case ActionType.MOVE:
                source.moved = true;
            break;
            case ActionType.STANDARD:
                source.acted = true;
            break;
            case ActionType.BONUS:
                source.usedBonus = true;
            break;
        }
    }

    public void ExecutePerTarget(GridCell target, Action callback)
    {
        skill.ExecutePerTarget(source, target, callback);
    }

    public void SetCompletionCallback(Action callback)
    {
        completionCallback = callback;
    }

    public void Complete()
    {
        Debug.Log(skill.skillName + "Complete");
        skill.executedOn.Clear();
        isExecuting = false;
        isCompleted = true;

        if(completionCallback != null)
        {
            completionCallback.Invoke();
        }
    }

    public int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}