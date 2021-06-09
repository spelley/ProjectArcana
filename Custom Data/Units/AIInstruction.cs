public struct AIInstruction
{
    public GridCell walkCell;
    public SkillData skill;
    public GridCell targetCell;

    public AIInstruction(GridCell walkCell, SkillData skill, GridCell targetCell)
    {
        this.walkCell = walkCell;
        this.skill = skill;
        this.targetCell = targetCell;
    }
}