using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Formation Data", menuName = "Custom Data/Formation", order = 1)]
public class FormationData : ScriptableObject
{
    [SerializeField]
    string _formationName;
    public string formationName
    {
        get
        {
            return _formationName;
        }
        private set
        {
            _formationName = value;
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
    private Sprite _image;
    public Sprite image
    {
        get
        {
            return _image;
        }
        private set
        {
            _image = value;
        }
    }
    
    [SerializeField]
    Vector3Int[] _offsets;
    public Vector3Int[] offsets
    {
        get
        {
            return _offsets;
        }
        private set
        {
            _offsets = value;
        }
    }
}