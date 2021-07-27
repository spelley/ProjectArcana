using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DivinationData : ScriptableObject
{
    [Header("Basic Information")]
    [SerializeField] protected string _id;
    public string id { get { return _id; } }

    string _loadType = "Divination";
    public virtual string loadType { get { return _loadType; } }

    [SerializeField] protected string _divinationName;
    public string divinationName {
        get { return _divinationName; }
        private set { _divinationName = value; }
    }

    [SerializeField] protected string _description;
    public string description {
        get { return _description; }
        private set { _description = value; }
    }

    [Header("Targeting Information")]
    [SerializeField] protected RiverSelectType _riverSelectType;
    public RiverSelectType riverSelectType {
        get { return _riverSelectType; }
        private set { _riverSelectType = value; }
    }

    [SerializeField, Range(0, 5)] protected int _minSelection;
    public int minSelection {
        get { return _minSelection; }
    }

    [SerializeField, Range(0, 5)] protected int _maxSelection;
    public int maxSelection {
        get { return _maxSelection; }
    }

    [SerializeField] protected bool _lockSelections;
    public bool lockSelections {
        get { return _lockSelections; }
    }

    [SerializeField] protected bool _unlockSelections;
    public bool unlockSelections {
        get { return _unlockSelections; }
    }

    [SerializeField] protected bool _deactivateSelections;
    public bool deactivateSelections {
        get { return _deactivateSelections; }
    }

    [SerializeField] protected bool _activateSelections;
    public bool activateSelections {
        get { return _activateSelections; }
    }

    [SerializeField] protected string _instructionsText;
    public string instructionsText {
        get { return _instructionsText; }
    }

    [Header("Animation Information")]
    [SerializeField] protected BattleAnimation _castAnimation;
    public BattleAnimation castAnimation
    { 
        get
        {
            return _castAnimation;
        } 
        private set
        {
            _castAnimation = value;
        }
    }

    [SerializeField] protected GameObject _executeAnimation;
    public GameObject executeAnimation
    {
        get
        {
            return _executeAnimation;
        }
        private set
        {
            _executeAnimation = value;
        }
    }

    public void HandleCardStates(List<RiverCard> riverCards)
    {
        if(!lockSelections && !unlockSelections && !activateSelections && !deactivateSelections)
        {
            return;
        }

        foreach(RiverCard riverCard in riverCards)
        {
            if(lockSelections)
            {
                riverCard.locked = true;
            }
            if(unlockSelections)
            {
                riverCard.locked = false;
            }
            if(deactivateSelections)
            {
                riverCard.inactive = true;
            }
            if(activateSelections)
            {
                riverCard.inactive = false;
            }
        }
    }

    public abstract void Execute(UnitData unitData, List<RiverCard> selectedRiverCards);

    public void ResolveDivination()
    {
        BattleManager.Instance.DivinationClear();
    }
}