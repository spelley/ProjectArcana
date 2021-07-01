using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModFloat
{
    public float baseValue = 0;
    public float baseMultiplier = 1f;
    public float resultMultiplier = 1f;
    public float baseAdd = 0;
    public float resultAdd = 0;
    public List<ElementData> elements;

    public ModFloat(float baseValue, float baseAdd = 0, float resultAdd = 0, float baseMultiplier = 1f, float resultMultiplier = 1f)
    {
        this.baseValue = baseValue;
        this.baseAdd = baseAdd;
        this.resultAdd = resultAdd;
        this.baseMultiplier = baseMultiplier;
        this.resultMultiplier = resultMultiplier;
    }

    public float GetCalculated()
    {
        return (((baseValue + baseAdd) * baseMultiplier) * resultMultiplier) + resultAdd;
    }
}
