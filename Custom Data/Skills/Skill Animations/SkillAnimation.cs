using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAnimation : MonoBehaviour
{
    public string id;
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

    public void SetSkill(SkillData skillData, UnitData unitData, List<GridCell> targets)
    {
        this.skillData = skillData;
        this.unitData = unitData;
        this.targets = targets;

        Animate();
    }

    public void Animate()
    {
        if(pauseCastAnimation)
        {
            unitData.unitGO.GetComponent<TacticsMotor>().anim.speed = 0f;
        }

        if(timeBetweenTargets > 0f)
        {
            StartCoroutine(AnimateOverTime());
        }
        else
        {
            skillData.Execute(unitData, targets);
            if(pauseCastAnimation)
            {
                unitData.unitGO.GetComponent<TacticsMotor>().anim.speed = 1f;
            }
            skillData.ResolveSkill();

            Cleanup();
        }
    }

    public IEnumerator AnimateOverTime()
    {
        for(int i = 0; i < targets.Count; i++)
        {
            GridCell gridCell = targets[i];
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
            skillData.ExecutePerTarget(unitData, gridCell);

            if(i < targets.Count - 1)
            {
                yield return new WaitForSeconds(timeBetweenTargets);
            }
            else
            {
                yield return new WaitForSeconds(timeBetweenTargets);
            }
        }

        Camera.main.GetComponent<CameraController>().SetFocus(unitData.unitGO);
        
        yield return null;

        if(pauseCastAnimation)
        {
            unitData.unitGO.GetComponent<TacticsMotor>().anim.speed = 1f;
        }
        skillData.ResolveSkill();

        Cleanup();
    }

    public void Cleanup()
    {
        GameObject.Destroy(this.gameObject);
    }
}