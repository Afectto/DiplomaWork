using UnityEngine;

public class PlayerFightSystem : FightSystem
{
    protected override void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(TryHit());
        }
    }
}