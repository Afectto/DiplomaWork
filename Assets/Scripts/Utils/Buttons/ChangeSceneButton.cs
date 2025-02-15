using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChangeSceneButton : MonoBehaviour
{
    [SerializeField] private SceneType typeScene;
    private SceneChanger _sceneChanger;

    [Inject]
    private void Inject(SceneChanger sceneChanger)
    {
        _sceneChanger = sceneChanger;
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClickButton);
    }

    protected virtual void OnClickButton()
    {
        SoundManager.Play(SoundType.Click);
        transform
            .DOShakeScale(0.25f, 0.1f, 10, 90)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                switch (typeScene)
                {
                    case SceneType.Menu:
                        _sceneChanger.ChangeScene(SceneNames.MainMenu);
                        break;
                    case SceneType.Game:
                        _sceneChanger.ChangeScene(SceneNames.GameScene);
                        break;
                    case SceneType.Loading:
                        _sceneChanger.ChangeScene(SceneNames.Loading);
                        break;
                }
            });
        
    }
}
