using UnityEngine;

public class StatsMenu : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private StatObject statObjectPrefab;

    private void Awake()
    {
        var statisticsList = SaveSystem.Load<SaveStatistic>().SaveData;
        for (int i = statisticsList.Count - 1; i >= 0; i--)
        {
            var statisticData = statisticsList[i];
            var statObject = Instantiate(statObjectPrefab, content.transform);
            statObject.Initialize(statisticData);
        }
    }
}