using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button back;

    [Inject]
    private void Inject(GameStateMachine stateMachine)
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {        
            transform
                .DOShakeScale(0.25f, 0.1f)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    stateMachine.ChangeState(GameStateData.Pause);
                    pauseMenu.SetActive(true);
                });
        });
        back.onClick.AddListener(() =>
        {
            transform
                .DOShakeScale(0.25f, 0.1f)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    stateMachine.ChangeState(GameStateData.Game);
                    pauseMenu.SetActive(false);
                });
        });
    }
}
