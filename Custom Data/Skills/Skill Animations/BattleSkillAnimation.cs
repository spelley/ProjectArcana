using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSkillAnimation : MonoBehaviour
{
    public string id;
    BattleSkill battleSkill;
    SkillData skillData;
    UnitData unitData;
    List<GridCell> targets = new List<GridCell>();

    [SerializeField]
    bool pauseCastAnimation = true;
    [SerializeField]
    float timeBetweenTargets = 1f;
    [SerializeField]
    float hitDelay = .2f;
    [SerializeField]
    GameObject hitSFX;

    public void SetSkill(BattleSkill _battleSkill)
    {
        battleSkill = _battleSkill;
        skillData = _battleSkill.skillData;
        unitData = _battleSkill.unitData;
        targets = _battleSkill.targets;
        Animate();
    }

    public void Animate()
    {
        if(pauseCastAnimation && unitData == BattleManager.Instance.curUnit)
        {
            unitData.unitGO.GetComponent<TacticsMotor>().anim.speed = 0f;
        }

        StartCoroutine(AnimateOverTime());
    }

    public IEnumerator AnimateOverTime()
    {
        int i = 0;
        bool continueExecuting = true;
        while(i < targets.Count)
        {
            if(!continueExecuting)
            {
                yield return null;
                continue;
            }
            else if(i > 0)
            {
                yield return new WaitForSeconds(timeBetweenTargets);
            }

            GridCell gridCell = targets[i];
            i++;
            if(skillData.requireUnitTarget && gridCell.occupiedBy == null)
            {
                continue;
            }

            if(skillData.requireUnitTarget && gridCell.occupiedBy != null)
            {
                Camera.main.GetComponent<CameraController>().SetFocus(gridCell.occupiedBy.unitGO);
            }

            if(hitSFX != null)
            {
                if(!skillData.requireUnitTarget || (gridCell.occupiedBy && !skillData.executedOn.Contains(gridCell.occupiedBy)))
                {
                    Instantiate(hitSFX, gridCell.realWorldPosition, Quaternion.identity);
                    if(hitDelay > 0f)
                    {
                        yield return new WaitForSeconds(hitDelay);
                    }
                }
            }

            continueExecuting = false;
            Action continueTargeting = () =>
            {
                continueExecuting = true;
            };
            
            battleSkill.ExecutePerTarget(gridCell, continueTargeting);

            yield return null;
        }
        yield return new WaitForSeconds(timeBetweenTargets);

        Camera.main.GetComponent<CameraController>().SetFocus(BattleManager.Instance.curUnit.unitGO);
        
        yield return null;

        if(pauseCastAnimation && unitData == BattleManager.Instance.curUnit)
        {
            unitData.unitGO.GetComponent<TacticsMotor>().anim.speed = 1f;
        }

        Cleanup();
    }

    public void Cleanup()
    {
        battleSkill.Complete();
        GameObject.Destroy(this.gameObject);
    }
}