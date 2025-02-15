using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTargetByPoints : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private List<Transform> pointsPatrol;
    [SerializeField] private float speed = 2f;
    private int _currentPointIndex;
    private const float ReachDistance = 0.1f;

    private void Awake()
    {
        StartPatrol();
        target.transform.position = pointsPatrol[0].position;
    }

    private void StartPatrol()
    {
        StartCoroutine(PatrolRoutine());
    }
    
    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            Transform targetPoint = pointsPatrol[_currentPointIndex];

            while (Vector3.Distance(target.transform.position, targetPoint.position) > ReachDistance)
            {
                MoveToPoint(targetPoint);
                yield return null;
            }

            _currentPointIndex = (_currentPointIndex + 1) % pointsPatrol.Count;
        }
    }

    private void MoveToPoint(Transform targetPoint)
    {
        Vector3 direction = (targetPoint.position - target.transform.position).normalized;
        target.transform.position += direction * speed * Time.deltaTime;
    }
}
