using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModInt
{
    public int baseValue = 0;
    public float baseMultiplier = 1f;
    public float resultMultiplier = 1f;
    public int baseAdd = 0;
    public int resultAdd = 0;
    public List<Element> elements;

    public ModInt(int baseValue, int baseAdd = 0, int resultAdd = 0, float baseMultiplier = 1f, float resultMultiplier = 1f)
    {
        this.baseValue = baseValue;
        this.baseAdd = baseAdd;
        this.resultAdd = resultAdd;
        this.baseMultiplier = baseMultiplier;
        this.resultMultiplier = resultMultiplier;
    }

    public int GetCalculated()
    {
        return Mathf.RoundToInt((((baseValue + baseAdd) * baseMultiplier) * resultMultiplier) + resultAdd);
    }
}
