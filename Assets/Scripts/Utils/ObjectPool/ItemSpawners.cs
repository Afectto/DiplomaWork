using System;
using System.Collections;
using Tasks =  System.Threading.Tasks.Task;
using UnityEngine;

public abstract class ItemSpawners<T> : MonoBehaviour where T : class, IPooledObject 
{
    [SerializeField] protected InterfaceReference<IPooledObject> itemPrefab;
    [SerializeField, Min(0)] protected float DelayToSpawn;
    [SerializeField, Min(0)] protected int maxActiveObject = Int32.MaxValue;
    private int _maxActiveObject;
    
    protected abstract ObjectPool<T> ItemPool { get; }

    public event Action<T> OnCreateItem;
    public event Action<T> OnReturnItem;
    
    protected abstract void Inject(ObjectPool<T> pool);

    private void Start()
    {
        _maxActiveObject = maxActiveObject;
    }
    
    public void SetMaxActiveObject(int value)
    {
        maxActiveObject = value;
    }
    
    public void StopSpawn()
    {
        maxActiveObject = 0;
    }
    
    public void ResumeSpawn()
    {
        maxActiveObject = _maxActiveObject;
    }
    
    public T CreateItem()
    {
        var item = ItemPool.GetObject();
        InitializeItem(item);
        item.OnNeedReturnToPool += OnNeedReturnToPool;
        OnCreateItem?.Invoke(item);
        return item;
    }

    protected abstract void InitializeItem(IPooledObject item);

    protected void OnNeedReturnToPool(IPooledObject item)
    {
        item.OnNeedReturnToPool -= OnNeedReturnToPool;
        ItemPool.ReturnObject(item as T);
        OnReturnItem?.Invoke(item as T);
    }
    
    protected IEnumerator SpawnObjects(int delay = 0)
    {
        yield return new WaitForSeconds(delay);
        while (true)
        {
            yield return new WaitUntil(() => ItemPool.GetCurrentActiveItemCount() < maxActiveObject);

            CreateItem();
            yield return new WaitForSeconds(DelayToSpawn);
        }
    }
    
    public async void CreateItemAndResetDelay()
    {
        await  Tasks.Delay(1000);
        
        if (ItemPool.GetCurrentActiveItemCount() - 1 > 0) return;
        
        StopAllCoroutines();
        StartCoroutine(SpawnObjects(0));
    }
    
    public int GetActiveObjectCount()
    {
        return ItemPool.GetCurrentActiveItemCount();
    }
}