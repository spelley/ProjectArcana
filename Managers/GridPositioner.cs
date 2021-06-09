using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPositioner : MonoBehaviour
{ 
    [Header("Height Controls")]
    public int height = 0;
    public bool preventOverride = false;

    [Header("Normals Override")]
    [Tooltip("Flat surfaces generally have normals of 0, 1, 0 (straight up) while slopes are malleable")]
    public Vector3 normal = new Vector3(0, 1f, 0);
    public bool preventNormalOverride = false;

    [Header("How many tiles below us we obstruct")]
    public int obstructionHeight = 0;
    public bool autoDetectObstructionHeight = true;

    [Header("Miscellaneous")]
    public bool isWalkable = true;
    public bool blockUnderneath = true;
}
