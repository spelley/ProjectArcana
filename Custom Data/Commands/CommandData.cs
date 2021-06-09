using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommandData: ScriptableObject
{
    [SerializeField]
    string _commandName;
    public string commandName
    { 
        get
        {
            return _commandName;
        } 
        private set
        {
            _commandName = value;
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
    
    public abstract void Execute(UnitData unitData);
}