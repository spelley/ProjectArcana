using Unity.Mathematics;

public struct CellStruct
{
    public int3 position;
    public int occupiedIdx;

    public CellStruct(int3 pos, int occupied)
    {
        position = pos;
        occupiedIdx = occupied;
    }
}