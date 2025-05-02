using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerStats
{
    private IPlayerStat _playerStat;
    private StatDecorator _statDecorator;
    
    public StatDecorator Stats => _statDecorator;

    public event Action<string> OnChangeStats;

    [Inject]
    private void Inject()
    {
        _playerStat = Resources.Load<BasePlayerStats>("BasePlayerStats");
        _statDecorator = new StatDecorator(_playerStat);
        _statDecorator.OnChangeStats += (name) => OnChangeStats?.Invoke(name);
    }

    public void AddBuff(string statName, StatModifier modifier)
    {
        Stats.AddModifier(statName, modifier);
    }

    public void AddBuff(Dictionary<string, StatModifier> buff)
    {
        foreach (var stat in buff)
        {
            Stats.AddModifier(stat.Key, stat.Value);
        }
    }

    public void AddBuff(string statName)
    {
        var modifier = GetDefaultModifier(statName);
        if (modifier != null)
        {
            Stats.AddModifier(statName, modifier);
        }
        else
        {
            Debug.LogWarning($"Нет предустановленного баффа для {statName}");
        }
    }
    
    private StatModifier GetDefaultModifier(string statName)
    {
        switch (statName)
        {
            case StatsTypeName.Damage:
                return new StatModifier(1f); // +5 урона
            case StatsTypeName.Health:
                return new StatModifier(1f); // +20 здоровья
            case StatsTypeName.AttackSpeed:
                return new StatModifier(1.02f, StatModifierType.Multiplicative);
            case StatsTypeName.Resists:
                return new StatModifier(1f);
            case StatsTypeName.MovementSpeed:
                return new StatModifier(1.1f, StatModifierType.Multiplicative);
            default:
                return null;
        }
    }
    
    public void RemoveBuff(string statName, StatModifier modifier)
    {
        Stats.RemoveModifier(statName, modifier);
    }
    
    public void RemoveBuff(Dictionary<string, StatModifier> buff)
    {
        foreach (var stat in buff)
        {
            Stats.RemoveModifier(stat.Key, stat.Value);
        }
    }
}