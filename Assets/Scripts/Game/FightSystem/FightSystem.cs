using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FightSystem : MonoBehaviour
{
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] private float attackRatePerSecond = 1f;
    [SerializeField] private Hit hit;
    private bool _isDelay;
    private Vector3 _lastMoveDirection;
    
    public void SetDirection(Vector3 direction)
    {
        _lastMoveDirection = direction;
    }

    protected IEnumerator TryHit()
    {
        if (_isDelay) yield break;
        _isDelay = true;
        hit.transform.position = transform.position + _lastMoveDirection * attackRange;
        hit.gameObject.SetActive(true);
        StartCoroutine(HideHit());
        yield return new WaitForSeconds(1f / attackRatePerSecond);
        _isDelay = false;
    }

    private IEnumerator HideHit()
    {
        yield return new WaitForSeconds(0.2f);
        hit.gameObject.SetActive(false);
        hit.transform.position = Vector3.zero;
    }

    protected abstract void Update();
}