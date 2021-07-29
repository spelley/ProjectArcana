using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatBlock
{
    public int level;
    public int experience;
    [Header("Resources")]
    public int maxHP;
    public int hp;
    public int maxMP;
    public int mp;
    public int maxSP;
    public int sp;

    [Header("Core Stats")]
    [Range(1, 100)]
    public int body;
    [Range(1, 100)]
    public int mind;
    [Range(1, 100)]
    public int spirit;
    [Range(1, 50)]
    public int speed;

    [Header("Movement")]
    [Range(0, 20)]
    public int move;
    [Range(0, 20)]
    public int jump;
    [Range(-100, 100)]
    public int evasion;

    public event Action<ModInt, Stat> OnCalculateMaxHP;
    public event Action<ModInt, Stat> OnCalculateMaxMP;
    public event Action<ModInt, Stat> OnCalculateBody;
    public event Action<ModInt, Stat> OnCalculateMind;
    public event Action<ModInt, Stat> OnCalculateSpirit;
    public event Action<ModInt, Stat> OnCalculateSpeed;
    public event Action<ModInt, Stat> OnCalculateMove;
    public event Action<ModInt, Stat> OnCalculateJump;
    public event Action<ModInt, Stat> OnCalculateEvasion;

    public bool AddExperience(int amount, UnitData unitData)
    {
        if(level <= 100)
        {
            if((experience + amount) > 100)
            {
                experience = 0;
                level += 1;
                unitData.GainedExperience(amount);
                unitData.GainedLevel(amount);
                return true;
            }
            
            experience += amount;
            unitData.GainedExperience(amount);
        }
        return false;
    }

    public int CalculateMaxHP()
    {
        ModInt modifiedMaxHP = new ModInt(maxHP);
        OnCalculateMaxHP?.Invoke(modifiedMaxHP, Stat.MAX_HP);
        return modifiedMaxHP.GetCalculated();
    }
    public int CalculateMaxMP()
    {
        ModInt modifiedMaxMP = new ModInt(maxMP);
        OnCalculateMaxMP?.Invoke(modifiedMaxMP, Stat.MAX_MP);
        return modifiedMaxMP.GetCalculated();
    }
    public int CalculateBody()
    {
        ModInt modifiedBody = new ModInt(body);
        OnCalculateBody?.Invoke(modifiedBody, Stat.BODY);
        return modifiedBody.GetCalculated();
    }
    public int CalculateMind()
    {
        ModInt modifiedMind = new ModInt(mind);
        OnCalculateMind?.Invoke(modifiedMind, Stat.MIND);
        return modifiedMind.GetCalculated();
    }
    public int CalculateSpirit()
    {
        ModInt modifiedSpirit = new ModInt(spirit);
        OnCalculateSpirit?.Invoke(modifiedSpirit, Stat.SPIRIT);
        return modifiedSpirit.GetCalculated();
    }
    public int CalculateSpeed()
    {
        ModInt modifiedSpeed = new ModInt(speed);
        OnCalculateSpeed?.Invoke(modifiedSpeed, Stat.SPEED);
        return modifiedSpeed.GetCalculated();
    }
    public int CalculateMove()
    {
        ModInt modifiedMove = new ModInt(move);
        OnCalculateMove?.Invoke(modifiedMove, Stat.MOVE);
        return modifiedMove.GetCalculated();
    }
    public int CalculateJump()
    {
        ModInt modifiedJump = new ModInt(jump);
        OnCalculateJump?.Invoke(modifiedJump, Stat.JUMP);
        return modifiedJump.GetCalculated();
    }
    public int CalculateEvasion()
    {
        ModInt modifiedEvasion = new ModInt(evasion);
        OnCalculateEvasion?.Invoke(modifiedEvasion, Stat.EVASION);
        return modifiedEvasion.GetCalculated();
    }

    public int GetByStat(Stat stat)
    {
        switch(stat)
        {
            case Stat.MAX_HP:
                return CalculateMaxHP();
            case Stat.HP:
                return hp;
            case Stat.MAX_MP:
                return CalculateMaxMP();
            case Stat.MP:
                return mp;
            case Stat.MAX_SP:
                return maxSP;
            case Stat.SP:
                return sp;
            case Stat.BODY:
                return CalculateBody();
            case Stat.MIND:
                return CalculateMind();
            case Stat.SPIRIT:
                return CalculateSpirit();
            case Stat.SPEED:
                return CalculateSpeed();
            case Stat.MOVE:
                return CalculateMove();
            case Stat.JUMP:
                return CalculateJump();
            case Stat.EVASION:
                return CalculateEvasion();
            case Stat.LEVEL:
                return level;
            case Stat.WEAPON_ATTACK:
                return 0; // TODO: Update when equipment is implemented
            case Stat.WEAPON_OFFHAND_ATTACK:
                return 0; // TODO: Update when equipment is implemented
            default:
                return 0;
        }
    }

    public void BindEventToStat(Stat stat, Action<ModInt, Stat> callback)
    {
        switch(stat)
        {
            case Stat.MAX_HP:
                OnCalculateMaxHP += callback;
            break;
            case Stat.MAX_MP:
                OnCalculateMaxMP += callback;
            break;
            case Stat.BODY:
                OnCalculateBody += callback;
            break;
            case Stat.MIND:
                OnCalculateMind += callback;
            break;
            case Stat.SPIRIT:
                OnCalculateSpirit += callback;
            break;
            case Stat.SPEED:
                OnCalculateSpeed += callback;
            break;
            case Stat.MOVE:
                OnCalculateMove += callback;
            break;
            case Stat.JUMP:
                OnCalculateJump += callback;
            break;
            case Stat.EVASION:
                OnCalculateEvasion += callback;
            break;
        }
    }

    public void UnbindEventToStat(Stat stat, Action<ModInt, Stat> callback)
    {
        switch(stat)
        {
            case Stat.MAX_HP:
                OnCalculateMaxHP -= callback;
            break;
            case Stat.MAX_MP:
                OnCalculateMaxMP -= callback;
            break;
            case Stat.BODY:
                OnCalculateBody -= callback;
            break;
            case Stat.MIND:
                OnCalculateMind -= callback;
            break;
            case Stat.SPIRIT:
                OnCalculateSpirit -= callback;
            break;
            case Stat.SPEED:
                OnCalculateSpeed -= callback;
            break;
            case Stat.MOVE:
                OnCalculateMove -= callback;
            break;
            case Stat.JUMP:
                OnCalculateJump -= callback;
            break;
            case Stat.EVASION:
                OnCalculateEvasion -= callback;
            break;
        }
    }

    public StatBlockSaveData GetSaveData()
    {
        StatBlockSaveData saveData = new StatBlockSaveData();
        saveData.level = level;
        saveData.experience = experience;
        saveData.maxHP = maxHP;
        saveData.hp = hp;
        saveData.maxMP = maxMP;
        saveData.mp = mp;
        saveData.maxSP = maxSP;
        saveData.sp = sp;
        saveData.body = body;
        saveData.mind = mind;
        saveData.spirit = spirit;
        saveData.speed = speed;
        saveData.move = move;
        saveData.jump = jump;
        saveData.evasion = evasion;

        return saveData;
    }

    public bool LoadFromSaveData(StatBlockSaveData saveData)
    {
        level = saveData.level;
        experience = saveData.experience;
        maxHP = saveData.maxHP;
        hp = saveData.hp;
        maxMP = saveData.maxMP;
        mp = saveData.mp;
        maxSP = saveData.maxSP;
        sp = saveData.sp;
        body = saveData.body;
        mind = saveData.mind;
        spirit = saveData.spirit;
        speed = saveData.speed;
        move = saveData.move;
        jump = saveData.jump;
        evasion = saveData.evasion;

        return true;
    }
}
