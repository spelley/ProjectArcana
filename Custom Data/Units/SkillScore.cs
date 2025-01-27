using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillScore
{
    public Vector3Int origin;
    public Vector3Int target;
    public int skillIndex;
    public int score;
    public int distanceScore;

    public SkillScore(Vector3Int origin, Vector3Int target, int skillIndex, int score, int distanceScore)
    {
        this.origin = origin;
        this.target = target;
        this.skillIndex = skillIndex;
        this.score = score;
        this.distanceScore = distanceScore;
    }
}