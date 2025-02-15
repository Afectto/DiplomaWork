using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public abstract class ObjectPool<T> where T : IPooledObject
{
    private GameObject _parent;
    private T _objectPrefab;
    private readonly List<T> _activeObjects = new List<T>();
    private readonly List<T> _inactiveObjects = new List<T>();
    private DiContainer _container;

    [Inject]
    protected virtual void Inject(DiContainer container)
    {
        _container = container;
    }

    protected virtual void Initialize(T objectPrefab, Transform parentPool = null)
    {
        _objectPrefab = objectPrefab;
        _parent = new GameObject($"{typeof(T).Name} Pool");
        _parent.transform.SetParent(parentPool);
        _parent.transform.localPosition = Vector3.zero;
        _parent.transform.localScale = Vector3.one;
    }

    private T GetInactiveObject()
    {
        var obj = _inactiveObjects.FirstOrDefault();
        if (!EqualityComparer<T>.Default.Equals(obj, default(T)))
        {
            _inactiveObjects.Remove(obj);
            _activeObjects.Add(obj);
        }
        return obj;
    }

    private T Create()
    {
        var newObject = _container.InstantiatePrefabForComponent<T>(_objectPrefab as Component, _parent.transform);
        newObject.CreateInit();
        
        _activeObjects.Add(newObject);
        return newObject;
    }
    
    public T GetObject()
    {
        T currentObject = GetInactiveObject() ?? Create();
        currentObject.GetInit();
        (currentObject as MonoBehaviour)?.gameObject.SetActive(true);
        return currentObject;
    }
    
    public void ReturnObject(T obj)
    {
        (obj as MonoBehaviour)?.gameObject.SetActive(false);
        _activeObjects.Remove(obj);
        _inactiveObjects.Add(obj);
    }

    public int GetCurrentActiveItemCount()
    {
        return _activeObjects.Count;
    }
    
    public List<T> GetCurrentActiveItem()
    {
        return _activeObjects;
    }
}
