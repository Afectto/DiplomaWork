using System;
using System.Collections.Generic;

public class StatDecorator : IPlayerStat
{
    private readonly IPlayerStat _baseStats;
    
    public event Action<string> OnChangeStats;

    private Dictionary<string, List<StatModifier>> _statModifiers = new Dictionary<string, List<StatModifier>>
    {
        { StatsTypeName.Damage, new List<StatModifier>() },
        { StatsTypeName.AttackSpeed, new List<StatModifier>() },
        { StatsTypeName.Health, new List<StatModifier>() },
        { StatsTypeName.Resists, new List<StatModifier>() },
        { StatsTypeName.MovementSpeed, new List<StatModifier>() },
        { StatsTypeName.MovementSmoothing, new List<StatModifier>() },
        { StatsTypeName.TurnSpeed, new List<StatModifier>() },
        { StatsTypeName.TurnSmoothing, new List<StatModifier>() },
        { StatsTypeName.Gravity, new List<StatModifier>() },
        { StatsTypeName.ComboCooldown, new List<StatModifier>() }
    };

    public StatDecorator(IPlayerStat baseStats)
    {
        _baseStats = baseStats;
    }

    public int Damage => ApplyModifiers(_baseStats.Damage, _statModifiers[StatsTypeName.Damage]);
    public float AttackSpeed => ApplyModifiers(_baseStats.AttackSpeed, _statModifiers[StatsTypeName.AttackSpeed]);
    public int Health => ApplyModifiers(_baseStats.Health, _statModifiers[StatsTypeName.Health]);
    public int Resists => ApplyModifiers(_baseStats.Resists, _statModifiers[StatsTypeName.Resists]);

    public float MovementSpeed => ApplyModifiers(_baseStats.MovementSpeed, _statModifiers[StatsTypeName.MovementSpeed]);
    public float MovementSmoothing => ApplyModifiers(_baseStats.MovementSmoothing, _statModifiers[StatsTypeName.MovementSmoothing]);
    public float TurnSpeed => ApplyModifiers(_baseStats.TurnSpeed, _statModifiers[StatsTypeName.TurnSpeed]);
    public float TurnSmoothing => ApplyModifiers(_baseStats.TurnSmoothing, _statModifiers[StatsTypeName.TurnSmoothing]);
    public float Gravity => ApplyModifiers(_baseStats.Gravity, _statModifiers[StatsTypeName.Gravity]);
    public float ComboCooldown => ApplyModifiers(_baseStats.ComboCooldown, _statModifiers[StatsTypeName.ComboCooldown]);

    private T ApplyModifiers<T>(T baseValue, List<StatModifier> modifiers)
    {
        float result = Convert.ToSingle(baseValue);

        foreach (var modifier in modifiers)
        {
            switch (modifier.ModifierType)
            {
                case StatModifierType.Additive:
                    result += modifier.Value;
                    break;
                case StatModifierType.Multiplicative:
                    result *= modifier.Value;
                    break;
                case StatModifierType.PercentDecrease:
                    result *= (1 - modifier.Value / 100);
                    break;
            }
        }

        return (T)Convert.ChangeType(result, typeof(T));
    }

    public void AddModifier(string statName, StatModifier modifier)
    {
        if (_statModifiers.ContainsKey(statName))
        {
            _statModifiers[statName].Add(modifier);
            OnChangeStats?.Invoke(statName);
        }
    }

    public void RemoveModifier(string statName, StatModifier modifier)
    {
        if (_statModifiers.ContainsKey(statName))
        {
            _statModifiers[statName].Remove(modifier);
            OnChangeStats?.Invoke(statName);
        }
    }
}