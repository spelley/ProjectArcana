using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArcanaData", menuName = "Custom Data/Arcana Data", order = 1)]
public class ArcanaData : ScriptableObject
{
    public string arcanaName;
    public Sprite icon;
    public Sprite cardImage;
    public List<ElementData> elements = new List<ElementData>();
}
