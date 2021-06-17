using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Arcana 
{
    public static int CompareArcana(ArcanaData sourceArcana, ArcanaData targetArcana)
    {
        return Arcana.CompareElements(sourceArcana.elements, targetArcana.elements);
    }

    public static int CompareElements(ArcanaData sourceArcana, List<ElementData> targetElements)
    {
        return Arcana.CompareElements(sourceArcana.elements, targetElements);
    }

    public static int CompareElements(List<ElementData> sourceElements, ArcanaData targetArcana)
    {
        return Arcana.CompareElements(sourceElements, targetArcana.elements);
    }

    public static int CompareElements(List<ElementData> sourceElements, List<ElementData> targetElements)
    {
        int affinity = 0;

        foreach(ElementData sourceElement in sourceElements)
        {
            foreach(ElementData targetElement in targetElements)
            {
                if(targetElement.weaknesses.Contains(sourceElement))
                {
                    affinity += 1;
                }
                if(targetElement.strengths.Contains(sourceElement))
                {
                    affinity -= 1;
                }
            }
        }

        return Mathf.Clamp(affinity, -2, 2);
    }
}