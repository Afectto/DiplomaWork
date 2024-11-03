using UnityEngine;

public class SlotGridObject
{
    private readonly Grid<SlotGridObject> _grid;
    private readonly int _x;
    private readonly int _y;

    private Slot _slotItem;

    public SlotGridObject(Grid<SlotGridObject> grid, int x, int y)
    {
        _grid = grid;
        _x = x;
        _y = y;
    }

    public void SetSlotItem(Slot slot)
    {
        if (_slotItem)
        {
            GameObject.Destroy(_slotItem);
        }

        _slotItem = slot;
        _grid.TriggerGridObjectChange(_x, _y);
    }

    public bool GetSlotIdentifier()
    {
        return _slotItem.Identifier;
    }

    public Slot GetSlot()
    {
        return _slotItem;
    }

    public void Destroy()
    {
        if (_slotItem)
        {
            GameObject.Destroy(_slotItem.gameObject);
        }
    }
}
