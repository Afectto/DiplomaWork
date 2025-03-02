using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour, ITargetable, IDamageable
{
    [SerializeField] protected float health;
    [SerializeField] private Image fill;
    protected float _healthMax;
    
    public Transform Transform => transform;

    public event Action OnDeath;
    public event Action<float> OnChangeHealth;
    
    public virtual float Health
    {
        get => health;
        protected set
        {
            health = value;
            OnChangeHealth?.Invoke(health);
            if (health <= 0)
            {
                OnDeath?.Invoke();
                OnDeathHandler();
            }

            UpdateHealth();
        }
    }

    protected virtual void Awake()
    {
        _healthMax = health;
    }

    protected abstract void OnDeathHandler(Character owner = null);

    public void SetDamage(float value, Character owner)
    {
        if (value < 0) return;

        Health -= value;
    }
    
    protected void UpdateHealth()
    {
        if(fill == null) return;
        fill.fillAmount = Health / _healthMax;
    }

    protected void InvokeOnDeath()
    {
        OnDeath?.Invoke();
    }

    protected void InvokeOnChangeHealth(float value)
    {
        OnChangeHealth?.Invoke(value);
    }
    
}