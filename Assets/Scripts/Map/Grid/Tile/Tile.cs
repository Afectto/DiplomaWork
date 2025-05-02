

using UnityEngine;

public class Tile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.OnFindInterestTile(transform.position);
        }
    }    
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.OnOutFindInterestTile(transform.position);
        }
    }
}