using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Loading : MonoBehaviour
{
    [SerializeField] private Image loadingImage;
    private SceneChanger _sceneChanger;

    [Inject]
    private void Inject(SceneChanger sceneChanger)
    {
        _sceneChanger = sceneChanger;
        if (loadingImage == null)
        {
            StartCoroutine(ChangeScene());
        }
        else
        {
            loadingImage
                .DOFillAmount(1, 2)
                .OnStart(() => loadingImage.fillAmount = 0)
                .SetUpdate(true)
                .OnComplete(() => _sceneChanger.ChangeScene(SceneNames.MainMenu));
        }
    }

    private IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(2f);
        _sceneChanger.ChangeScene(SceneNames.MainMenu);
    }
}
