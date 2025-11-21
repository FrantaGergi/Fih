using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    // Singleton instance
    public static MenuManager Instance { get; private set; }



    [SerializeField] private string lakeSceneName;
    [SerializeField] private string seaSceneName;
    [SerializeField] private string barSceneName;
    [SerializeField] private string riverSceneName;
    [SerializeField] private string menuSceneName;


    private List<GameObject> interactives = new List<GameObject>();
    private Transform menubackground;

    private MainMenuController mainMenuController;
    private Button showMenuBttn;
    private Button registeredShowMenuButton; // track registered button to remove listener
    // Ensures the menu is shown only once on initial game start
    private bool hasShownMenuOnStartup = false;

    private void Awake()
    {
        Debug.Log("MenuManager Awake");
        // Enforce singleton - keep only one instance and persist it across scenes
        if (Instance != null && Instance != this)
        {
            Debug.Log("MenuManager duplicate destroyed");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);


    }

    private void OnDestroy()
    {
        // cleanup listener if manager destroyed
        if (registeredShowMenuButton != null)
        {
            registeredShowMenuButton.onClick.RemoveListener(ShowMenuManual);
            registeredShowMenuButton = null;
        }
    }


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
        River,
        Menu
    }

    void Start()
    {
        Debug.Log("MenuManager Start");

        mainMenuController = FindFirstObjectByType<MainMenuController>();
        if (mainMenuController != null)
        {
            interactives = mainMenuController.Interactives;
            menubackground = mainMenuController.MenuBackground;
            showMenuBttn = mainMenuController.ShowMenuBttn;
            if (menubackground != null)
                menubackground.gameObject.SetActive(false);

            RegisterShowMenuButton(showMenuBttn);
        }


        if (!hasShownMenuOnStartup)
        {
            ShowMenu(true);
            hasShownMenuOnStartup = true;
        }

    }

    private void RegisterShowMenuButton(Button btn)
    {
        if (registeredShowMenuButton == btn) return;

        if (registeredShowMenuButton != null)
        {
            registeredShowMenuButton.onClick.RemoveListener(ShowMenuManual);
        }

        registeredShowMenuButton = btn;

        if (registeredShowMenuButton != null)
        {
            registeredShowMenuButton.onClick.AddListener(ShowMenuManual);
        }
    }

    // Fallback input handling if Input System callback isn't wired
    private void Update()
    {
        // touch input (new Input System) - mobile
        if (Touchscreen.current != null)
        {
            // primaryTouch is null on some devices until touched
            var primary = Touchscreen.current.primaryTouch;
            if (primary != null && primary.press.wasPressedThisFrame)
            {
                Vector2 screenPos = primary.position.ReadValue();
                HandlePointerClick(screenPos);
                return;
            }

            // also check all touches (multi-touch)
            foreach (var t in Touchscreen.current.touches)
            {
                if (t.press != null && t.press.wasPressedThisFrame)
                {
                    Vector2 screenPos = t.position.ReadValue();
                    HandlePointerClick(screenPos);
                    return;
                }
            }
        }

        // use new Input System if available (mouse)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            HandlePointerClick(screenPos);
            return;
        }

        // also support legacy Input as an extra fallback
        // this call can throw InvalidOperationException if project is set to use Input System only
        try
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Vector2 screenPos = UnityEngine.Input.mousePosition;
                HandlePointerClick(screenPos);
            }
        }
        catch (System.InvalidOperationException)
        {
            // Input System is active only; ignore legacy Input calls
        }
    }

    // Public method you can wire to a button later to show the menu manually
    public void ShowMenuManual()
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
                if (interact == null) continue;
                interact.SetActive(false);
            }

            if (menubackground != null)
            {
                menubackground.gameObject.SetActive(true);
                StartCoroutine(ScaleIn(menubackground, 3f));
            }
            Debug.Log("Show Menu");
        }
        else
        {
            if (menubackground != null)
            {
                StartCoroutine(ScaleOut(menubackground, 3f));
            }
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
        if (menubackground != null)
            menubackground.gameObject.SetActive(false);
    }

    public void TurnOnInteract()
    {
        if (interactives == null) return;

        // Remove destroyed/null entries that can remain after scene changes
        interactives.RemoveAll(item => item == null);

        // If list is empty (e.g. new scene), try to find MainMenuController in the current scene and refresh
        if (interactives.Count == 0)
        {
            mainMenuController = FindFirstObjectByType<MainMenuController>();
            if (mainMenuController != null && mainMenuController.Interactives != null)
            {
                interactives = new List<GameObject>(mainMenuController.Interactives);
                showMenuBttn = mainMenuController.ShowMenuBttn;
                RegisterShowMenuButton(showMenuBttn);
            }
        }

        foreach (GameObject interact in interactives)
        {
            if (interact == null) continue;
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
            case MenuState.Menu:
                Debug.Log("Jsi už v menu!");
                LoadScene(menuSceneName);
                StartCoroutine(FindMainMenuNextFrame());
                break;
        }
    }


    private IEnumerator FindMainMenuNextFrame()
    {
        yield return null; // poèkej jeden frame, až se objekty vytvoøí
        mainMenuController = FindFirstObjectByType<MainMenuController>();
        if (mainMenuController != null)
        {
            interactives = new List<GameObject>(mainMenuController.Interactives);
            menubackground = mainMenuController.MenuBackground;
            showMenuBttn = mainMenuController.ShowMenuBttn;
            RegisterShowMenuButton(showMenuBttn);
            if (menubackground != null)
                menubackground.gameObject.SetActive(false);

            // Ensure interactives are enabled so they can be clicked after returning to menu
            interactives.RemoveAll(item => item == null);
            foreach (var go in interactives)
                if (go != null) go.SetActive(true);

            Debug.Log("MainMenuController found next frame and interactives refreshed");
        }
        else
        {
            Debug.LogWarning("MainMenuController not found in FindMainMenuNextFrame");
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != menuSceneName) return;

        mainMenuController = FindFirstObjectByType<MainMenuController>();
        if (mainMenuController != null)
        {
            interactives = new List<GameObject>(mainMenuController.Interactives);
            menubackground = mainMenuController.MenuBackground;
            if (menubackground != null)
                menubackground.gameObject.SetActive(false);

            // ensure interactives active
            interactives.RemoveAll(item => item == null);
            foreach (var go in interactives)
                if (go != null) go.SetActive(true);

            Debug.Log("MainMenuController nalezen po naètení scény a interactives aktivovány");
        }
        else
        {
            Debug.LogWarning("MainMenuController not found in OnSceneLoaded");
        }
    }

    // Centralized pointer click handling
    private void HandlePointerClick(Vector2 screenPos)
    {
        Debug.Log($"HandlePointerClick at {screenPos}");

        if (Camera.main == null)
        {
            Debug.LogWarning("Camera.main is null - cannot raycast");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // Try RaycastAll and check parent components as well, log all hits for debugging
        var hits = Physics.RaycastAll(ray, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        if (hits.Length == 0)
        {
            Debug.Log("No physics hits from RaycastAll");
        }
        else
        {
            Debug.Log($"RaycastAll hit count: {hits.Length}");
        }

        foreach (var h in hits)
        {
            Debug.Log("Hit: " + h.collider.gameObject.name + " (collider)");
            // try get interactive on the exact object
            var interactive = h.collider.gameObject.GetComponent<IInteractive>();
            if (interactive == null)
            {
                // fallback to parent
                interactive = h.collider.gameObject.GetComponentInParent<IInteractive>();
                if (interactive != null)
                    Debug.Log("Found IInteractive in parent: " + interactive);
            }
            if (interactive == null)
            {
                // fallback to children
                interactive = h.collider.gameObject.GetComponentInChildren<IInteractive>();
                if (interactive != null)
                    Debug.Log("Found IInteractive in children: " + interactive);
            }

            if (interactive != null)
            {
                Debug.Log("Invoking OnInteract on: " + h.collider.gameObject.name);
                interactive.OnInteract(this);
                return; // handle first interactive
            }
        }
    }
}
