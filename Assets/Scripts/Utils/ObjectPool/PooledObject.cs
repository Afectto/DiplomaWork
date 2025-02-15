using System;
using UnityEngine;

[Serializable]
public abstract class PooledObject : MonoBehaviour, IPooledObject
{
    public event Action<IPooledObject> OnNeedReturnToPool;
    
    public abstract void GetInit();
    public abstract void CreateInit();

    public void TriggerOnNeedReturnToPool()
    {
        OnNeedReturnToPool?.Invoke(this);
    }
}