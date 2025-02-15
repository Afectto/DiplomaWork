using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class AfterPauseDelay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counter;

    public void StartCounterAnimation(int count)
    {
        gameObject.SetActive(true);
        StartCoroutine(AnimateCounter(count));
    }

    private IEnumerator AnimateCounter(int count)
    {
        yield return new WaitForSeconds(0.1f);
        
        Time.timeScale = 0;
        counter.transform.localScale = Vector3.zero;
        
        for (int i = count; i >= 0; i--)
        {
            counter.text = i.ToString();
            yield return counter.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutQuad).SetUpdate(true).WaitForCompletion();
            yield return counter.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutQuad).SetUpdate(true).WaitForCompletion();
        }
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
