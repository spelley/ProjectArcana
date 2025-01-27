using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationExecuteSkill : MonoBehaviour
{
    public void Execute(AnimationEvent myEvent)
    {
        if(BattleManager.Instance != null)
        {
            BattleManager.Instance.PreparedSkillConfirm();
        }
    }

    public void Clear()
    {
        if(BattleManager.Instance != null)
        {
            BattleManager.Instance.PreparedSkillComplete();
        }
    }
}
