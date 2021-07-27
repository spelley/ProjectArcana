using System;

[Serializable]
public struct SimpleVector3Int
{
    public int x;
    public int y;
    public int z;

    public SimpleVector3Int(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }
}