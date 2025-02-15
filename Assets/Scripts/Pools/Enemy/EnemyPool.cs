
using UnityEngine;

public class EnemyPool : ObjectPool<Enemy>
{
    public new void Initialize(Enemy objectPrefab, Transform parentPool = null)
    {
        base.Initialize(objectPrefab, parentPool);
    }
}
