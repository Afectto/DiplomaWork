using DG.Tweening;
using UnityEngine;

public class InfinityRotate : MonoBehaviour
{
    [SerializeField] private float duration = 10;
    [SerializeField] private bool clockwise = true;
    
    void Start()
    {
        transform.DORotate(new Vector3(0, 0, clockwise ? 360 : -360), duration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(true);
    }
}
