using System.Collections.Generic;

public class SaveLevelData : ISaveData
{
    public Dictionary<int, LevelSettings> LevelSettingsDictionary { get; set; }

    public SaveLevelData(string _)
    {
        var offset = new MyVector3(0.25f, -4, 0);
        LevelSettingsDictionary = new Dictionary<int, LevelSettings>
        {
            {
                1, new LevelSettings
                {
                    SettingGrid = new GridSetting(10, 10, 2, offset),
                    MinTasksDifficulty = 1,
                    MaxTasksDifficulty = 1,
                    WallCount = 20,
                    DangerPointsCount = 3,
                    InterestPointsCount = 2
                }
            },
            {
                2, new LevelSettings
                {
                    SettingGrid = new GridSetting(10, 15, 2, offset),
                    MinTasksDifficulty = 1,
                    MaxTasksDifficulty = 2,
                    WallCount = 50,
                    DangerPointsCount = 3,
                    InterestPointsCount = 4
                }
            },
            {
                3, new LevelSettings
                {
                    SettingGrid = new GridSetting(15, 10, 2, offset),
                    MinTasksDifficulty = 1,
                    MaxTasksDifficulty = 3,
                    WallCount = 50,
                    DangerPointsCount = 4,
                    InterestPointsCount = 4
                }
            },
            {
                4, new LevelSettings
                {
                    SettingGrid = new GridSetting(15, 15, 2, offset),
                    MinTasksDifficulty = 2,
                    MaxTasksDifficulty = 4,
                    WallCount = 125,
                    DangerPointsCount = 5,
                    InterestPointsCount = 4
                }
            },
            {
                5, new LevelSettings
                {
                    SettingGrid = new GridSetting(20, 20, 2, offset),
                    MinTasksDifficulty = 2,
                    MaxTasksDifficulty = 5,
                    WallCount = 250,
                    DangerPointsCount = 7,
                    InterestPointsCount = 4
                }
            }
        };
    }
    
}