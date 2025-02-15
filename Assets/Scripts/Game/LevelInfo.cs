using System.Collections.Generic;

public class LevelInfo : ISaveData
{
    public int LevelNumber;
    public int LevelCount;
    public List<bool> LevelStatus;
    public List<int> LevelPrice;
    
    public LevelInfo(string _)
    {
        LevelNumber = 1;
        LevelCount = 6;
        LevelStatus = new List<bool>{true, false, false, false, false, false};
        LevelPrice = new List<int>{0, 500, 625, 750, 875, 1000};
    }
    
    public void SetLevel(int level)
    {
        LevelNumber = level;
        SaveSystem.Save(this);
    }
    
    public void SetLevelStatus(int level, bool status)
    {
        LevelStatus[level - 1] = status;
        SaveSystem.Save(this);
    }
}