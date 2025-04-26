using System.Collections.Generic;

public class LevelData : ISaveData
{
    public bool IsNeedSave; // Флаг, указывающий, нужно ли сохранять данные
    public int Level; // Уровень
    public List<EnemyData> Enemies; // Данные о врагах
    public MapData MapConfiguration; // Конфигурация карты
    public List<TaskData> Tasks; // Данные о задачах
    public PlayerData PlayerData; // Данные о игроке

    public LevelData(string _)
    {
        Enemies = new List<EnemyData>();
        Tasks = new List<TaskData>();
        IsNeedSave = false;
    }
    
    public void SetData(List<EnemyData> enemies, MapData mapConfiguration, List<TaskData> tasks, PlayerData playerData)
    {
        Enemies = enemies;
        MapConfiguration = mapConfiguration;
        Tasks = tasks;
        PlayerData = playerData;
        SaveSystem.Save(this);
    }
    
    public void SetIsNeedSave(bool isNeedSave)
    {
        IsNeedSave = isNeedSave;
        SaveSystem.Save(this);
    }
    
    public void SetLevel(int level)
    {
        Level = level;
        SaveSystem.Save(this);
    }
}