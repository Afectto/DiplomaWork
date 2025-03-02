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
    private void Init()
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