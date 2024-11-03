using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background;
    private bool _identifier;
    public bool Identifier => _identifier;
    
    private void ChangeBackgroundColor(bool isActive)
    {
        background.color = isActive? Color.gray : Color.white;
    }

    public void ChangIdentifier(bool identifier)
    {
        _identifier = identifier;
        ChangeBackgroundColor(identifier);
    }

}
