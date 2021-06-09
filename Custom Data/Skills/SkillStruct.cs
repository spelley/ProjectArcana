public struct SkillStruct
{
    public int range;
    public int areaOfEffect;
    public int heightTolerance;
    public int rangeBoost;
    public RangeType rangeType;
    public TargetType targetType;
    public TargetShape targetShape;

    public SkillStruct(int range, int areaOfEffect, int heightTolerance, int rangeBoost, RangeType rangeType, TargetType targetType, TargetShape targetShape)
    {
        this.range = range;
        this.areaOfEffect = areaOfEffect;
        this.heightTolerance = heightTolerance;
        this.rangeBoost = rangeBoost;
        this.rangeType = rangeType;
        this.targetType = targetType;
        this.targetShape = targetShape;
    }
}