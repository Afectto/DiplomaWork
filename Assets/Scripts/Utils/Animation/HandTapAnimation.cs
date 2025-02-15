using DG.Tweening;
using UnityEngine;

public class HandTapAnimation : MonoBehaviour
{
    [SerializeField] private float duration;
    
    void Start()
    {
        transform
            .DOLocalRotate(new Vector3(35, 0, 0), duration)
            .SetRelative()
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }
}
