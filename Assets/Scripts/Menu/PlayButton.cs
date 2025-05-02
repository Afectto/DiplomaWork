using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private bool isNewGame;

    [Inject]
    private void Inject(SceneChanger sceneChanger)
    {
        var levelData = SaveSystem.Load<LevelData>();
        
        if(!isNewGame && (levelData.MapConfiguration == null || !levelData.IsNeedSave))
        {
            gameObject.SetActive(false);
        }
        
        GetComponent<Button>().onClick.AddListener(() =>
        {
            levelData.SetIsNeedSave(!isNewGame);
            sceneChanger.ChangeScene(SceneNames.GameScene);
        });
    }
}
