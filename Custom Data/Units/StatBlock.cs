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

    public event Action<ModInt, Stat> OnCalculateMaxHP;
    public event Action<ModInt, Stat> OnCalculateMaxMP;
    public event Action<ModInt, Stat> OnCalculateMaxTP;
    public event Action<ModInt, Stat> OnCalculateBody;
    public event Action<ModInt, Stat> OnCalculateMind;
    public event Action<ModInt, Stat> OnCalculateSpirit;
    public event Action<ModInt, Stat> OnCalculateSpeed;
    public event Action<ModInt, Stat> OnCalculateMove;
    public event Action<ModInt, Stat> OnCalculateJump;

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
    public int CalculateMaxTP()
    {
        ModInt modifiedMaxTP = new ModInt(maxTP);
        OnCalculateMaxTP?.Invoke(modifiedMaxTP, Stat.MAX_TP);
        return modifiedMaxTP.GetCalculated();
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
        }
    }
}
