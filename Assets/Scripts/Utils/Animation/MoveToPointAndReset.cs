using DG.Tweening;
using UnityEngine;

public class MoveToPointAndReset : MonoBehaviour
{
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private float duration;
    
    void Start()
    {
        transform
            .DOLocalMove(endPosition, duration)
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .OnStepComplete(() => transform.localPosition = Vector3.zero)
            .SetUpdate(true);
    }

}
