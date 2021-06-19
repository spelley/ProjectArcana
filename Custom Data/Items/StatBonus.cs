using System;

[Serializable]
public struct StatBonus
{
    public Stat stat;
    public int amount;

    public StatBonus(Stat stat, int amount)
    {
        this.stat = stat;
        this.amount = amount;
    }
}