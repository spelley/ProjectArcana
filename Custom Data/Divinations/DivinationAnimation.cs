using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivinationAnimation : MonoBehaviour
{
    DivinationData divinationData;
    UnitData unitData;
    List<RiverCard> targets = new List<RiverCard>();

    [SerializeField]
    bool pauseCastAnimation = true;
    [SerializeField]
    float duration = 1f;
    [SerializeField]
    GameObject sfx;

    public void SetDivination(DivinationData divinationData, UnitData unitData, List<RiverCard> targets)
    {
        this.divinationData = divinationData;
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

        if(duration > 0f && sfx != null)
        {
            StartCoroutine(AnimateOverTime());
        }
        else
        {
            divinationData.Execute(unitData, targets);
            if(pauseCastAnimation)
            {
                unitData.unitGO.GetComponent<TacticsMotor>().anim.speed = 1f;
            }
            divinationData.ResolveDivination();

            Cleanup();
        }
    }

    public IEnumerator AnimateOverTime()
    {
        Camera.main.GetComponent<CameraController>().SetFocus(this.unitData.unitGO);
        if(sfx != null)
        {
            Instantiate(sfx, unitData.unitGO.transform.position, Quaternion.identity);
        }
        divinationData.Execute(unitData, targets);
        yield return new WaitForSeconds(duration);
        Camera.main.GetComponent<CameraController>().SetFocus(unitData.unitGO);
        
        yield return null;

        if(pauseCastAnimation)
        {
            unitData.unitGO.GetComponent<TacticsMotor>().anim.speed = 1f;
        }
        divinationData.ResolveDivination();

        Cleanup();
    }

    public void Cleanup()
    {
        GameObject.Destroy(this.gameObject);
    }
}