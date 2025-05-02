using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsItem : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI textCount;
    [SerializeField] private TextMeshProUGUI text;
    
    public event Action OnClick;
    
    public void Awake()
    {
        button.onClick.AddListener(() =>
        {
            transform
                .DOShakeScale(0.25f, 0.1f)
                .SetUpdate(true)
                .OnComplete(() => OnClick?.Invoke());
        });
    }
    
    public void SetText(string value, string nameStats)
    {
        text.text = nameStats;
        textCount.text = value;
    }
}