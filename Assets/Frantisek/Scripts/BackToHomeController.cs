using UnityEngine;
using UnityEngine.UI;

public class BackToHomeController : MonoBehaviour
{
    [SerializeField] private Button backButton;

    private void OnEnable()
    {

        if (backButton == null)
            backButton = GetComponent<Button>();

        if (backButton != null)
            backButton.onClick.AddListener(BackToHome);
        else
            Debug.LogError("Back button is not assigned in BackToHomeController.");
    }

    private void OnDisable()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(BackToHome);
    }

    public void BackToHome()
    {
        MenuManager.Instance.OnInteractiveClicked(MenuManager.MenuState.Menu);
    }
}
