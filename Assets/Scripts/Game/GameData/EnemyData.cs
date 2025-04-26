using System;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public MyVector3 Position; // Положение врага на карте
    public float HP; // Здоровье врага

    public EnemyData(string _) { }
    public EnemyData() { }
    
    public EnemyData(Enemy enemy)
    {
        Position = enemy.transform.position;
        HP = enemy.Health;
    }
    
    public EnemyData(Vector3 position, int hp)
    {
        Position = position;
        HP = hp;
    }
}