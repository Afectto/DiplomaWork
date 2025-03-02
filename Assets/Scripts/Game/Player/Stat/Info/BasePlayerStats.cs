using UnityEngine;

[CreateAssetMenu(menuName = "PlayerStats")]
public class BasePlayerStats : ScriptableObject, IPlayerStat
{
    [SerializeField] private int _damage = 10;
    [SerializeField] private int _health = 100;
    [SerializeField] private int _resists = 0;

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _movementSmoothing;
    [SerializeField] private float _turnSpeed;
    [SerializeField] private float _turnSmoothing;
    [SerializeField] private float _gravity;

    [Header("Combat Settings")]
    [SerializeField] private float _comboCooldown;
    [SerializeField] private float _attackSpeed = 1;

    public int Damage { get => _damage; }
    public float AttackSpeed { get => _attackSpeed; }
    public int Health { get => _health; }
    public int Resists { get => _resists; }
    public float MovementSpeed { get => _movementSpeed; }
    public float MovementSmoothing {  get => _movementSmoothing; }
    public float TurnSpeed {  get => _turnSpeed; }
    public float TurnSmoothing { get => _turnSmoothing; }
    public float Gravity { get => _gravity; }
    public float ComboCooldown { get => _comboCooldown; }
}