using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private List<TutorialPart> partTutorial;
    [SerializeField] private Button skipTutorial;

    [SerializeField] private bool isNeedStopTime = true;

    private bool _isNeedShow;

    [Inject]
    private void Inject(SceneChanger sceneChanger)
    {
        _isNeedShow = SaveSystem.Load<ShowTutorial>().isNeedShow;

        sceneChanger.OnChangeScene += OnChangeScene;
        
        foreach (var part in partTutorial)
        {
            part.OnNextPart += TryShowNextPart;
        }
    }

    private void OnChangeScene(string nameScene)
    {
        if (!_isNeedShow)
            return;

        var showTutorial = SaveSystem.Load<ShowTutorial>();
        var namePart = nameScene == SceneNames.MainMenu && showTutorial.isNeedShowBuy 
            ? TutorialPart.TutorialPartName.Buy 
            : TutorialPart.TutorialPartName.BallsDestroy;

        if (nameScene == SceneNames.MainMenu && !showTutorial.isNeedShowBuy)
        {
            namePart = TutorialPart.TutorialPartName.SecondBuy;
        }

        ShowTutorialPartByName(namePart);
    }

    private void TryShowNextPart(TutorialPart part)
    {
        if (part.NextName != TutorialPart.TutorialPartName.None)
        {
            part.gameObject.SetActive(false);
            ShowTutorialPartByName(part.NextName);
            return;
        }

        if (part.CurrentName == partTutorial.Find(tutorialPart => tutorialPart.isActiveAndEnabled).CurrentName)
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }
        part.gameObject.SetActive(false);
        
        UpdateSaveTutorialState(part.CurrentName);
    }

    private void UpdateSaveTutorialState(TutorialPart.TutorialPartName nameShowedPart)
    {
        var showTutorial = SaveSystem.Load<ShowTutorial>();
        switch (nameShowedPart)
        {
            case TutorialPart.TutorialPartName.Trash:
                showTutorial.isNeedShow = false;
                break;
            case TutorialPart.TutorialPartName.Buy:
                showTutorial.isNeedShowBuy = false;
                break;
        }
        SaveSystem.Save(showTutorial);
    }

    public void ShowTutorialPartByName(TutorialPart.TutorialPartName namePart)
    {
        _isNeedShow = SaveSystem.Load<ShowTutorial>().isNeedShow;
        var part = partTutorial.FirstOrDefault(partTutor => partTutor.CurrentName == namePart);
        if (_isNeedShow && part != null)
        {
            gameObject.SetActive(true);
            part.gameObject.SetActive(true);
        }
        else if(this)
        {
            gameObject.SetActive(false);
        }

        UpdateSaveTutorialState(namePart);
    }
    
    private void Awake()
    {
        if (!_isNeedShow)
        {
            EndTutorial();
            return;
        }
        Time.timeScale = isNeedStopTime ? 0 : 1;
        skipTutorial.onClick.AddListener(EndTutorial);
    }

    private void EndTutorial()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
        CloseTutorial();
    }

    private void CloseTutorial()
    {
        var showTutorial = SaveSystem.Load<ShowTutorial>();
        showTutorial.isNeedShow = false;
        SaveSystem.Save(showTutorial);
    }

}