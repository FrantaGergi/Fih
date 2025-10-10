using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class FishController : MonoBehaviour
{
    [Header("Markers")]
    public Transform leftPos;
    public Transform middlePos;
    public Transform rightPos;

    [Header("Prefab & UI")]
    public GameObject fishPrefab;
    public Button startButton;

    [Header("Movement")]
    public float moveInDuration = 1.1f;
    public float moveOutDuration = 1.0f;
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Cutting")]
    public int requiredCuts = 5;
    public float tapCooldown = 0.10f;

    GameObject fish;
    bool cutting;
    int cuts;
    float nextTap;

    void Start()
    {
        if (startButton) startButton.onClick.AddListener(StartFlow);
    }

    public void StartFlow()
    {
        StopAllCoroutines();
        cutting = false;
        cuts = 0;

        if (fish) Destroy(fish);
        fish = Instantiate(fishPrefab, leftPos.position, Quaternion.identity);

        StartCoroutine(MoveCo(fish, middlePos.position, moveInDuration, () =>
        {
            cutting = true;
        }));
    }

    void Update()
    {
        if (!cutting) return;

        if (TapPressedThisFrame())
        {
            nextTap = Time.time + tapCooldown;
            cuts++;

            if (cuts >= requiredCuts)
            {
                cutting = false;
                StartCoroutine(MoveCo(fish, rightPos.position, moveOutDuration, () =>
                {
                    Destroy(fish);
                }));
            }
        }
    }

    bool TapPressedThisFrame()
    {
        if (Time.time < nextTap) return false;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) return true;

        var ts = Touchscreen.current;
        if (ts != null && ts.primaryTouch.press.wasPressedThisFrame) return true;

        return false;
    }

    IEnumerator MoveCo(GameObject obj, Vector3 to, float duration, System.Action onDone)
    {
        Vector3 from = obj.transform.position;
        float t = 0f;
        duration = Mathf.Max(0.0001f, duration);

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float e = ease.Evaluate(Mathf.Clamp01(t));
            obj.transform.position = Vector3.LerpUnclamped(from, to, e);
            yield return null;
        }
        onDone?.Invoke();
    }
}
