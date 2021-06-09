using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatBlock
{
    public int level;
    [Header("Resources")]
    public int maxHP;
    public int hp;
    public int maxMP;
    public int mp;
    public int maxTP;
    public int tp;

    [Header("Core Stats")]
    public int body;
    public int mind;
    public int spirit;
    public int speed;

    [Header("Movement")]
    public int move;
    public int jump;

    public event Action<ModInt> OnCalculateMaxHP;
    public event Action<ModInt> OnCalculateMaxMP;
    public event Action<ModInt> OnCalculateMaxTP;
    public event Action<ModInt> OnCalculateBody;
    public event Action<ModInt> OnCalculateMind;
    public event Action<ModInt> OnCalculateSpirit;
    public event Action<ModInt> OnCalculateSpeed;
    public event Action<ModInt> OnCalculateMove;
    public event Action<ModInt> OnCalculateJump;

    public int CalculateMaxHP()
    {
        ModInt modifiedMaxHP = new ModInt(maxHP);
        OnCalculateMaxHP?.Invoke(modifiedMaxHP);
        return modifiedMaxHP.GetCalculated();
    }
    public int CalculateMaxMP()
    {
        ModInt modifiedMaxMP = new ModInt(maxMP);
        OnCalculateMaxMP?.Invoke(modifiedMaxMP);
        return modifiedMaxMP.GetCalculated();
    }
    public int CalculateMaxTP()
    {
        ModInt modifiedMaxTP = new ModInt(maxTP);
        OnCalculateMaxTP?.Invoke(modifiedMaxTP);
        return modifiedMaxTP.GetCalculated();
    }
    public int CalculateBody()
    {
        ModInt modifiedBody = new ModInt(body);
        OnCalculateBody?.Invoke(modifiedBody);
        return modifiedBody.GetCalculated();
    }
    public int CalculateMind()
    {
        ModInt modifiedMind = new ModInt(mind);
        OnCalculateMind?.Invoke(modifiedMind);
        return modifiedMind.GetCalculated();
    }
    public int CalculateSpirit()
    {
        ModInt modifiedSpirit = new ModInt(spirit);
        OnCalculateSpirit?.Invoke(modifiedSpirit);
        return modifiedSpirit.GetCalculated();
    }
    public int CalculateSpeed()
    {
        ModInt modifiedSpeed = new ModInt(speed);
        OnCalculateSpeed?.Invoke(modifiedSpeed);
        return modifiedSpeed.GetCalculated();
    }
    public int CalculateMove()
    {
        ModInt modifiedMove = new ModInt(move);
        OnCalculateMove?.Invoke(modifiedMove);
        return modifiedMove.GetCalculated();
    }
    public int CalculateJump()
    {
        ModInt modifiedJump = new ModInt(jump);
        OnCalculateJump?.Invoke(modifiedJump);
        return modifiedJump.GetCalculated();
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
            case Stat.MAX_TP:
                return CalculateMaxTP();
            case Stat.TP:
                return tp;
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
}
