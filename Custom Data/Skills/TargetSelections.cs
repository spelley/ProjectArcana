using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct TargetSelections
{
    public Vector3Int origin;
    public List<Vector3Int> offsets;
    public int skillHeight;

    public int shapeHeight;

    public TargetSelections(Vector3Int origin, List<Vector3Int> offsets, int skillHeight, int shapeHeight)
    {
        this.origin = origin;
        this.offsets = offsets;
        this.skillHeight = skillHeight;
        this.shapeHeight = shapeHeight;
    }
}