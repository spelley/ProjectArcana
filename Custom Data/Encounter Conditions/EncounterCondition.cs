using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EncounterCondition : ScriptableObject
{
    [SerializeField]
    string _conditionLabel;
    public string conditionLabel
    {
        get
        {
            return _conditionLabel;
        }
        private set
        {
            _conditionLabel = value;
        }
    }

    [SerializeField]
    List<UnitData> _leaders = new List<UnitData>(); 
    public List<UnitData> leaders
    {
        get
        {
            return _leaders;
        }
        private set
        {
            _leaders = value;
        }
    }

    public abstract bool IsCompleted();
}
