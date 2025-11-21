using UnityEngine;
using static MenuManager;

public class MenuIneractController : MonoBehaviour, IInteractive
{
    [SerializeField] MenuState menuState;


    public void OnInteract(MenuManager menuManager)
    {
        // Use the passed-in MenuManager (the one that detected the click)
        menuManager.OnInteractiveClicked(menuState);
    }
}
