using UnityEngine;

public class Hit : MonoBehaviour
{
    private IDamageable _thisOwner;
    
    private void Awake()
    {
        _thisOwner = GetComponentInParent<IDamageable>();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageable character) && character != _thisOwner)
        {
            character.SetDamage(1, GetComponent<Character>());
        }
    }

}