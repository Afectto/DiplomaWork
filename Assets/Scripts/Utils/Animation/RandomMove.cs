using DG.Tweening;
using UnityEngine;

public class RandomMove : MonoBehaviour
{
    [SerializeField] private GameObject target;
    
    [SerializeField] private float moveRange = 5f;
    [SerializeField] private float moveDuration = 2f;
    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = target.transform.position;
        StartRandomMovement();
    }

    private void StartRandomMovement()
    {
        Vector3 randomTarget = _startPosition + new Vector3(
            Random.Range(-moveRange, moveRange),
            Random.Range(-moveRange, moveRange),
            0);

        target.transform.DOMove(randomTarget, moveDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(StartRandomMovement);
    }
}
