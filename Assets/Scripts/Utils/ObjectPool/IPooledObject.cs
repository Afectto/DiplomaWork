using System;

public interface IPooledObject
{
    event Action<IPooledObject> OnNeedReturnToPool;

    void GetInit();
    void CreateInit();
    void TriggerOnNeedReturnToPool();
}