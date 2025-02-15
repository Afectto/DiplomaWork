using System;

public interface IInteractiveItemGame
{
    public event Action OnItemDestroy;
    public event Action OnItemHit;
}