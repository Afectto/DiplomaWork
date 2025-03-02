public class StatModifier
{
    public StatModifierType ModifierType { get; }
    public float Value { get; }

    public StatModifier(float value, StatModifierType modifierType = StatModifierType.Additive )
    {
        ModifierType = modifierType;
        Value = value;
    }
}