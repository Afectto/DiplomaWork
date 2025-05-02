using System.Collections.Generic;

public class PlayerSkillPointsData : ISaveData
{
    public int SkillPoints;
    public int SkillPointsUsage;
    public Dictionary<string, int> PlayerStatsAfterUseSkillPoints;

    public PlayerSkillPointsData(string _)
    {
        SkillPoints = 0;
        SkillPointsUsage = 0;
        PlayerStatsAfterUseSkillPoints = new Dictionary<string, int>();
    }
    
    public string GetStatsCount(string type)
    {
        if (PlayerStatsAfterUseSkillPoints.ContainsKey(type))
        {
            return PlayerStatsAfterUseSkillPoints[type].ToString();
        }
        return "0";
    }
    
    public void AddSkillPoints(int amount)
    {
        SkillPoints += amount;
        SaveSystem.Save(this);
    }
    
    public void UseSkillPoints(int amount, string type)
    {
        SkillPoints -= amount;
        SkillPointsUsage += amount;
        if (PlayerStatsAfterUseSkillPoints.ContainsKey(type))
        {
            PlayerStatsAfterUseSkillPoints[type] += amount;
        }
        else
        {
            PlayerStatsAfterUseSkillPoints[type] = amount;
        }
        SaveSystem.Save(this);
    }
    
    public void ClearSkillPoints()
    {
        SkillPoints = 0;
        SkillPointsUsage = 0;
        PlayerStatsAfterUseSkillPoints.Clear();
        SaveSystem.Save(this);
    }
}