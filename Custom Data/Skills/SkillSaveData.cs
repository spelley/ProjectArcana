using System;

public class SkillSaveData : ISaveData
{
    public string id;
    public string ID { get { return id; } }
    public string loadType;
    public string LoadType { get { return loadType; } }
    public string name;
    public string description;
    public int spCost;
    public string actionType;
    public string skillEffectID;
    public int hpCost;
    public int mpCost;
    public string[] elementIDs;
    public string[] matchTypes;
    public string castAnimation;
    public string executeAnimation;
    public int hitChance;
    public string targetType;
    public string targetShape;
    public string rangeType;
    public int range;
    public int height;
    public int heightTolerance;
    public int areaOfEffect;
    public int push;
    public bool pushFromTarget;
    public bool requireUnitTarget;
    public bool requireEmptyTile;
    public string skillCalculation;
    public string primaryAttribute;
    public string secondaryAttribute;
    public string tertiaryAttribute;
    public int primaryValue;
    public int secondaryValue;
    public int tertiaryValue;
    public string associatedSkillID;
}