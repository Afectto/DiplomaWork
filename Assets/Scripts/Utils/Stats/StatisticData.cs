using System;

[Serializable]
public struct StatisticData
{
    public int Level;
    public int Score;
    public int Coins;
    public ResultStatus Result;
    
    public enum ResultStatus
    {
        Skip,
        Win,
        Lose
    }
}