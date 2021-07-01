using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DivinationData : ScriptableObject
{
    [Header("Basic Information")]
    [SerializeField] int _divinationID;
    public int divinationID {
        get { return _divinationID; }
    }

    [SerializeField] string _divinationName;
    public string divinationName {
        get { return _divinationName; }
        private set { _divinationName = value; }
    }

    [SerializeField] string _description;
    public string description {
        get { return _description; }
        private set { _description = value; }
    }

    [Header("Targeting Information")]
    [SerializeField] RiverSelectType _riverSelectType;
    public RiverSelectType riverSelectType {
        get { return _riverSelectType; }
        private set { _riverSelectType = value; }
    }

    [SerializeField, Range(0, 5)] int _minSelection;
    public int minSelection {
        get { return _minSelection; }
    }

    [SerializeField, Range(0, 5)] int _maxSelection;
    public int maxSelection {
        get { return _maxSelection; }
    }

    [SerializeField] bool _lockSelections;
    public bool lockSelections {
        get { return _lockSelections; }
    }

    [SerializeField] bool _unlockSelections;
    public bool unlockSelections {
        get { return _unlockSelections; }
    }

    [SerializeField] bool _deactivateSelections;
    public bool deactivateSelections {
        get { return _deactivateSelections; }
    }

    [SerializeField] bool _activateSelections;
    public bool activateSelections {
        get { return _activateSelections; }
    }

    [SerializeField] string _instructionsText;
    public string instructionsText {
        get { return _instructionsText; }
    }

    [Header("Animation Information")]
    [SerializeField]
    BattleAnimation _castAnimation;
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

    [SerializeField]
    GameObject _executeAnimation;
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