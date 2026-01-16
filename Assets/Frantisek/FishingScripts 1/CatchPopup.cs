using System.Collections;
using UnityEngine;
using TMPro;

public class CatchPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject popupRoot; // root popup objekt (obsahuje TextMeshProUGUI)
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Timing / animation")]
    [SerializeField] private float showDuration = 0.35f;
    [SerializeField] private float visibleTime = 5f;
    [SerializeField] private float hideDuration = 0.3f;

    [Header("Scale")]
    [SerializeField] private Vector3 showScale = Vector3.one;
    [SerializeField] private Vector3 startScale = Vector3.one * 0.8f;

    [Header("Easing")]
    [SerializeField] private AnimationCurve ease = default;

    private CanvasGroup canvasGroup;
    private Coroutine running;

    void Awake()
    {
        if (popupRoot == null) popupRoot = gameObject;
        if (messageText == null) messageText = GetComponentInChildren<TextMeshProUGUI>(true);

        canvasGroup = popupRoot.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = popupRoot.AddComponent<CanvasGroup>();

        if (ease == null || ease.length == 0)
        {
            // default ease (smooth)
            ease = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        }

        popupRoot.SetActive(false);
    }

    // Volatelná metoda: zobrazí popup s textem, po visibleTime se sám skryje
    public void ShowCatch(string message)
    {
        if (messageText != null) messageText.text = message;
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(ShowRoutine());
    }

    // Pøetížená pomocná metoda s výchozí zprávou
    public void ShowCatch()
    {
        ShowCatch("Chytil nìjakou rybu!");
    }

    private IEnumerator ShowRoutine()
    {
        popupRoot.SetActive(true);
        popupRoot.transform.localScale = startScale;
        canvasGroup.alpha = 0f;

        // show
        float t = 0f;
        while (t < showDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / showDuration);
            float e = ease.Evaluate(p);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, e);
            popupRoot.transform.localScale = Vector3.Lerp(startScale, showScale, e);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        popupRoot.transform.localScale = showScale;

        // visible
        yield return new WaitForSeconds(visibleTime);

        // hide
        t = 0f;
        while (t < hideDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / hideDuration);
            float e = ease.Evaluate(p);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, e);
            popupRoot.transform.localScale = Vector3.Lerp(showScale, startScale, e);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        popupRoot.SetActive(false);
        running = null;
    }
}
