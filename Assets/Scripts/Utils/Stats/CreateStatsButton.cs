using UnityEngine;
using UnityEngine.UI;

public class CreateStatsButton : MonoBehaviour
{
    [SerializeField] private StatisticData.ResultStatus statusResult;
    private Button _button;
    private GameController _gameController;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(CreateStats);
        _gameController = FindObjectOfType<GameController>();
    }

    private void CreateStats()
    {
        // _gameController.AddStatisticData(statusResult);
    }
}
