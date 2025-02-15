
using DG.Tweening;
using Tasks = System.Threading.Tasks.Task;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Zenject;

public class SoundSettingsUI : MonoBehaviour
{
    [SerializeField] private GameObject _enable, _disable;
    [SerializeField] private AudioMixerGroup _mixer;
    private Button _button;

    [Inject]
    private async void Inject()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() =>
        {
            _button.interactable = false;
            transform.DOShakeScale(0.5f, 0.1f, 10, 90).SetUpdate(true)
                .OnComplete(() => { _button.interactable = true; });
            SoundManager.Play(SoundType.Click);
            RefreshSaveData();
            RefreshGameValue();
        });

        await Tasks.Delay(10);
        RefreshGameValue();
    }

    private void RefreshGameValue()
    {
        var settings = SaveSystem.Load<SoundSettings>();
        _mixer.audioMixer.SetFloat("Sounds", settings.IsSoundActive ? 0 : -80);
        Show(settings.IsSoundActive);
    }

    private static void RefreshSaveData()
    {
        var settings = SaveSystem.Load<SoundSettings>();
        settings.IsSoundActive = !settings.IsSoundActive;
        SaveSystem.Save(settings);
    }

    public void Show(bool active)
    {
        _enable.SetActive(active);
        _disable.SetActive(!active);
    }
}
