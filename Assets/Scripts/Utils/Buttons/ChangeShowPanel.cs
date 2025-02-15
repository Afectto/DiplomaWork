using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ChangeShowPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject panelFilling;
    [SerializeField] private Button closeButton;

    [SerializeField] private bool IsNeedHideThis;
    [SerializeField] private List<GameObject> hidePart;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(()=>ShowPanel(true));
        closeButton?.onClick.AddListener(()=>ShowPanel(false));
    }

    private void ShowPanel(bool value)
    {
        Time.timeScale = value ? 0 : 1;
        SoundManager.Play(SoundType.Click);
        
        closeButton?
            .transform
            .DOShakeScale(0.25f, 0.1f, 10, 90)
            .SetUpdate(true);
        
        transform
            .DOShakeScale(0.25f, 0.1f, 10, 90)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                if (IsNeedHideThis)
                {
                    ChangeAll(!value);
                }

                panel.SetActive(value);
                panelFilling.SetActive(value);
            });
    }

    private void ChangeAll(bool value)
    {
        foreach (var part in hidePart)
        {
            part.SetActive(value);
        }
    }
}
