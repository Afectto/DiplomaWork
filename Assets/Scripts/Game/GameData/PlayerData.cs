using UnityEngine;

public class PlayerData
{
    public MyVector3 Position; // Положение игрока на карте
    public float HP; // Здоровье игрока
    
    public PlayerData(string _) { }
    public PlayerData() { }
    
    public PlayerData(Player player)
    {
        Position = player.transform.position;
        HP = player.Health;
    }
    
    public PlayerData(Vector3 position, int hp)
    {
        Position = position;
        HP = hp;
    }
}