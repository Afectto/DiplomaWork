using UnityEngine;

public class SetValidTutorialUnmask : MonoBehaviour
{
    // [SerializeField] private List<Unmask> unmaskItems;
    //
    // private List<Cell> _allCells;
    //
    // [Inject]
    // private void Inject(MenuController menuController)
    // {
    //     _allCells = menuController.GetCells();
    // }
    //
    // private void Start()
    // {
    //     var allOccupyCells = _allCells.FindAll(cell => cell.IsOccupied);
    //     if (unmaskItems.Count == allOccupyCells.Count)
    //     {
    //         for (var index = 0; index < unmaskItems.Count; index++)
    //         {
    //             var unmask = unmaskItems[index];
    //             unmask.transform.position = allOccupyCells[index].transform.position;
    //         }
    //     }
    //     else
    //     {
    //         transform.parent.gameObject.SetActive(false);
    //         gameObject.SetActive(false);
    //         _ = AwaitOccupyCellsAsync();
    //     }
    // }
    //
    // private async Task AwaitOccupyCellsAsync()
    // {
    //     var allOccupyCells = _allCells
    //         .FindAll(cell => cell.IsOccupied && !cell.name.Contains("Left") && !cell.name.Contains("Right"));
    //
    //     while (unmaskItems.Count != allOccupyCells.Count)
    //     {
    //         await Task.Yield();
    //         allOccupyCells = _allCells.FindAll(cell => cell.IsOccupied);
    //     }
    //     
    //     for (var index = 0; index < unmaskItems.Count; index++)
    //     {
    //         var unmask = unmaskItems[index];
    //         unmask.transform.position = allOccupyCells[index].transform.position;
    //     }
    //     transform.parent.gameObject.SetActive(true);
    //     gameObject.SetActive(true);
    // }
    
}
