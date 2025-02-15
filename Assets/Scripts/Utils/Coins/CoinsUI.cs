using TMPro;
using UnityEngine;
using Zenject;

public class CoinsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    private CoinsManager _coinsManager;

    [Inject]
    private void Inject(CoinsManager coinsManager)
    {
        _coinsManager = coinsManager;
        _coinsManager.OnCoinsChanged += OnCoinsChanged;
        OnCoinsChanged();
    }

    private void OnCoinsChanged()
    {
        coinsText.text = _coinsManager.CoinsCount.ToString();
    }
}
