using UnityEngine;
using static MenuManager;

public class MenuIneractController : MonoBehaviour, IInteractive
{
    MenuManager MenuManager;
    [SerializeField] MenuState menuState;


    public void OnInteract(MenuManager menuManager)
    {
        MenuManager.OnInteractiveClicked(menuState);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MenuManager = FindFirstObjectByType<MenuManager>();
    }

}
