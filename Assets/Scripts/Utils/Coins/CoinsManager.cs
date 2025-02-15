using System;
using Zenject;

public class CoinsManager
{
    private int _currentCount;
    private CoinsInfo _coinsInfo;

    public int CoinsCount => _currentCount;
    
    public event Action OnCoinsChanged; 

    [Inject]
    private void Inject()
    {
        _coinsInfo = SaveSystem.Load<CoinsInfo>();
        _currentCount = _coinsInfo.coins;
    }
    
    public void AddCoins(int count)
    {
        _currentCount += count;
        _coinsInfo.AddCoins(count);
        OnCoinsChanged?.Invoke();
    }
    
    public bool CanRemoveCoins(int count)
    {
        return _currentCount >= count;
    }
    
    public void RemoveCoins(int count)
    {
        _currentCount -= count;
        _coinsInfo.AddCoins(-count);
        OnCoinsChanged?.Invoke();
    }
    
    private class CoinsInfo : ISaveData
    {
        public int coins;
        
        public CoinsInfo(string _)
        {
            coins = 0;
        }
        
        public void AddCoins(int count)
        {
            coins += count;
            SaveSystem.Save(this);
        }
    }
}