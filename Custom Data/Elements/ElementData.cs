using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementData", menuName = "Custom Data/Element")]
public class ElementData : ScriptableObject
{
    [SerializeField]
    string _elementName;
    public string elementName
    {
        get
        {
            return _elementName;
        }
        private set
        {
            _elementName = value;
        }
    }

    [SerializeField]
    string _description;
    public string description
    {
        get
        {
            return _description;
        }
        private set
        {
            _description = value;
        }
    }

    [SerializeField]
    Sprite _icon;
    public Sprite icon
    {
        get
        {
            return _icon;
        }
        private set
        {
            _icon = value;
        }
    }

    [SerializeField]
    Color _color;
    public Color color
    {
        get
        {
            return _color;
        }
        private set
        {
            _color = value;
        }
    }
    
    [SerializeField]
    List<ElementData> _weaknesses = new List<ElementData>();
    public List<ElementData> weaknesses
    {
        get
        {
            return _weaknesses;
        }
        private set
        {
            _weaknesses = value;
        }
    }

    [SerializeField]
    List<ElementData> _strengths = new List<ElementData>();
    public List<ElementData> strengths
    {
        get
        {
            return _strengths;
        }
        private set
        {
            _strengths = value;
        }
    }
}
