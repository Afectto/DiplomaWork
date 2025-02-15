using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private float fadeDuration = 1f;
    private string _currentName;
    
    public Action<string> OnChangeScene;
    public Action<string> OnPrepareChangeScene;

    [Inject]
    private void Inject()
    {
        loadingScreen.alpha = 0f;
        loadingScreen.gameObject.SetActive(false);
    }
    
    public void ChangeScene(string sceneName)
    {
        _currentName = sceneName;
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public string GetCurrentName() => _currentName;
    
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.gameObject.SetActive(true);
        
        OnPrepareChangeScene?.Invoke(sceneName);
        yield return loadingScreen.DOFade(1f, fadeDuration).SetUpdate(true).WaitForCompletion();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        OnChangeScene?.Invoke(sceneName);

        yield return loadingScreen.DOFade(0f, fadeDuration).SetUpdate(true).WaitForCompletion();

        loadingScreen.gameObject.SetActive(false);
    }
}