using System.Collections.Generic;

class SaveStatistic : ISaveData
{    
    public List<StatisticData> SaveData;
    public SaveStatistic(string _)
    {
        SaveData = new List<StatisticData>();
    }

    public void AddStatisticData(StatisticData data)
    {
        if (SaveData.Count + 1 == 50)
        {
            SaveData.RemoveAt(0);
        }
        SaveData.Add(data);
        SaveSystem.Save(this);
    }
}