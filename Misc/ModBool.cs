using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModBool
{
    public bool baseValue;

    public ModBool(bool baseValue)
    {
        this.baseValue = baseValue;
    }

    public bool GetCalculated()
    {
        return baseValue;
    }
}
