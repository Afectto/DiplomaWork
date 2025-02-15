using TMPro;
using UnityEngine;

public class StatObject : MonoBehaviour
{    
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinText;

    [SerializeField] private GameObject win;
    [SerializeField] private GameObject skip;
    [SerializeField] private GameObject lose;

    public void Initialize(StatisticData data)
    {
        levelText.text = data.Level.ToString();
        scoreText.text = data.Score.ToString();
        coinText.text = data.Coins.ToString();

        HideAll();
        
        var valueStars = data.Result;
        switch (valueStars)
        {
            case StatisticData.ResultStatus.Skip:
                skip.gameObject.SetActive(true);
                break;
            case StatisticData.ResultStatus.Win:
                win.gameObject.SetActive(true);
                break;
            case StatisticData.ResultStatus.Lose:
                lose.gameObject.SetActive(true);
                break;
        }
    }

    public void HideAll()
    {
        win.gameObject.SetActive(false);
        skip.gameObject.SetActive(false);
        lose.gameObject.SetActive(false);
    }
}