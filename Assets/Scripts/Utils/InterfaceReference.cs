using System;
using UnityEngine;

[Serializable]
public class InterfaceReference<T>
{
    [SerializeField] private MonoBehaviour target;

    public T Value => (T)(object)target;

    public bool IsValid => target is T;
}