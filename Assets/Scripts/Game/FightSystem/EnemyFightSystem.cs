using System;
using UnityEngine;

public class EnemyFightSystem : FightSystem
{
    private Player _player;

    [Obsolete("Obsolete")]
    private void Start()
    {
        _player = FindObjectOfType<Player>();
    }

    protected override void Update()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) <= attackRange)
        {
            StartCoroutine(TryHit());
        }
    }
}