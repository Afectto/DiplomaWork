using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScaleLoop : MonoBehaviour
{
    [SerializeField] private List<Transform> targets;
    
    private void OnEnable()
    {
        foreach (var target in targets)
        {
            target
                .DOScale(1.1f, 1.5f)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }
    }
    
    private void OnDisable()
    {
        foreach (var target in targets)
        {
            target.DOKill();
        }
    }
}
