public interface IPlayerStat
{
  public int Damage { get; }
  public float AttackSpeed { get; }
  public int Health { get; }
  public int Resists { get; }

  public float MovementSpeed { get ; }
  public float MovementSmoothing { get ; }
  public float TurnSpeed { get ; }
  public float TurnSmoothing { get ; }
  public float Gravity { get; }
  public float ComboCooldown { get;  }
}