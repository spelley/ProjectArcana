using System;

[Serializable]
public struct SimpleVector3
{
    public float x;
    public float y;
    public float z;

    public SimpleVector3(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }
}