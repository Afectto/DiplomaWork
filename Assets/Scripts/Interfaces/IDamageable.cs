using System;

interface IDamageable
{
    public event Action OnDeath;
    public event Action<float> OnChangeHealth;
    public void SetDamage(float value, Character owner);
}