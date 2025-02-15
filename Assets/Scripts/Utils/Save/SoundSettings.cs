using System;

[Serializable]
public class SoundSettings : ISaveData
{
    public bool IsSoundActive;

    public SoundSettings(string _)
    {
        IsSoundActive = true;
    }
} 
