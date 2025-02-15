using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShakeTarget : MonoBehaviour
{
    [SerializeField] private List<Transform> targets;

    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float strength = 0.1f;
    [SerializeField] private bool isLoops;
    
    private void OnEnable()
    {
        foreach (var target in targets)
        {
            target
                .DOShakeScale(duration, strength)
                .SetUpdate(true)
                .SetLoops(isLoops ? -1 : 0);
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
