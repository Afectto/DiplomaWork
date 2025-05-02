using UnityEngine;

[CreateAssetMenu(menuName = "PlayerStats")]
public class BasePlayerStats : ScriptableObject, IPlayerStat
{
    [SerializeField] private int _damage = 10;
    [SerializeField] private int _health = 100;
    [SerializeField] private int _resists = 0;

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed;

    [Header("Combat Settings")]
    [SerializeField] private float _attackSpeed = 1;

    public int Damage { get => _damage; }
    public float AttackSpeed { get => _attackSpeed; }
    public int Health { get => _health; }
    public int Resists { get => _resists; }
    public float MovementSpeed { get => _movementSpeed; }
}