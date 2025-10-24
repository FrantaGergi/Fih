using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform Menubackground;
    [SerializeField] private List<GameObject> interactives = new List<GameObject>();

    [SerializeField] private string lakeSceneName; 
    [SerializeField] private string seaSceneName;
    [SerializeField] private string barSceneName;
    [SerializeField] private string riverSceneName;


    public void LoadScene(string sceneToLoad)
    {
        if (sceneToLoad != null)
        {
           
            // naèíst scénu
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public enum MenuState
    {
        Lake,
        Sea,
        Bar,
        River
    }

    void Start()
    {
        ShowMenu(true);
    }

    public void ShowMenu(bool show)
    {
        StopAllCoroutines(); // zastaví pøedchozí efekty, aby se nepletly

        if (show)
        {
            foreach (GameObject interact in interactives)
            {
                interact.SetActive(false);
            }

            Menubackground.gameObject.SetActive(true);
            StartCoroutine(ScaleIn(Menubackground, 3f));   
            Debug.Log("Show Menu");
        }
        else
        {
            StartCoroutine(ScaleOut(Menubackground, 3f));  
            Invoke(nameof(TurnOnInteract), 3.5f);
        }
    }

    public void StartGame()
    {
        ShowMenu(false);
        Invoke(nameof(HideBackground), 3f);
        Invoke(nameof(TurnOnInteract), 4f);
    }

    private void HideBackground()
    {
        Menubackground.gameObject.SetActive(false);
    }

    public void TurnOnInteract()
    {
        foreach (GameObject interact in interactives)
        {
            interact.SetActive(true);
        }
    }

    public void QuitGame()
    {
        ShowMenu(false);
        Application.Quit();
    }

    public void OpenSetting()
    {
        Debug.Log("Open Setting Menu");
    }

    public IEnumerator ScaleIn(Transform target, float duration = 1f)
    {
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero; // neviditelné
        Vector3 endScale = Vector3.one;    // plná velikost

        target.localScale = startScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.localScale = Vector3.LerpUnclamped(startScale, endScale, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        target.localScale = endScale;
    }

    public IEnumerator ScaleOut(Transform target, float duration = 1f)
    {
        float elapsed = 0f;
        Vector3 startScale = target.localScale;
        Vector3 endScale = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.localScale = Vector3.LerpUnclamped(startScale, endScale, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        target.localScale = endScale;
    }

    public void OnInteractiveClicked(MenuState name)
    {
        Debug.Log("Klikl jsi na: " + name);

        switch (name)
        {
            case MenuState.Lake:
                Debug.Log("Spouštím lake!");
                LoadScene(lakeSceneName);
                // tady zavoláš tøeba StartGame();
                break;

            case MenuState.River:
                Debug.Log("Otevírám reku!");
                LoadScene(riverSceneName);
                // tady tøeba OpenShop();
                break;
            
            case MenuState.Sea:
                Debug.Log("Otevírám more!");
                LoadScene(seaSceneName);
                // tady tøeba OpenShop();
                break;
            case MenuState.Bar:
                Debug.Log("Otevírám obchod!");
                LoadScene(barSceneName);
                // tady tøeba OpenShop();
                break;
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {

        if (!context.performed)
        {
            return;
        }

        Debug.Log("Kliknuto");

        Vector2 screenPos = Vector2.zero;

        // kontrola typu ovládání
        if (context.control.device is Mouse)
            screenPos = Mouse.current.position.ReadValue();
        else if (context.control.device is Touchscreen)
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();

        // nebo, pokud chceš univerzálnì
        // screenPos = Pointer.current.position.ReadValue();

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));


        Ray ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);  
            IInteractive interactionManager = hit.collider.gameObject.GetComponent<IInteractive>();
                interactionManager?.OnInteract(this);
            }
        
    }
}
