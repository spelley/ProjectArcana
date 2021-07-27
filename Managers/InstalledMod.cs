using System;

[Serializable]
public struct InstalledMod
{
    public string name;
    public int priority;

    public InstalledMod(string name, int priority)
    {
        this.name = name;
        this.priority = priority;
    }
}