using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class FishSplitController : MonoBehaviour
{
    [Header("Markers")]
    public Transform leftPos;
    public Transform middlePos;
    public Transform rightPos;

    [Header("Prefabs")]
    public GameObject topPrefab;
    public GameObject bottomPrefab;

    [Header("UI")]
    public Button startButton;

    [Header("Timing")]
    public float moveInDuration = 1.0f;
    public float moveOutDuration = 0.9f;
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float clearExtraRight = 1.5f;

    GameObject top;
    GameObject bottom;

    enum Phase { Idle, WaitFirstTap, TopSlidingOut, WaitClearTap, Clearing }
    Phase phase = Phase.Idle;

    void Start()
    {
        if (startButton) startButton.onClick.AddListener(StartFlow);
    }

    public void StartFlow()
    {
        StopAllCoroutines();
        DestroyIfExists();

        top = Instantiate(topPrefab, leftPos.position, Quaternion.identity);
        phase = Phase.Idle;

        StartCoroutine(MoveCo(top, middlePos.position, moveInDuration, () =>
        {
            phase = Phase.WaitFirstTap;
        }));
    }

    void Update()
    {
        if (!TapPressedThisFrame()) return;

        switch (phase)
        {
            case Phase.WaitFirstTap:
                bottom = Instantiate(bottomPrefab, middlePos.position, Quaternion.identity);

                phase = Phase.TopSlidingOut;
                StartCoroutine(MoveCo(top, rightPos.position, moveOutDuration, () =>
                {
                    phase = Phase.WaitClearTap;
                }));
                break;

            case Phase.WaitClearTap:
                phase = Phase.Clearing;

                Vector3 finalRight = rightPos.position + Vector3.right * clearExtraRight;

                int done = 0;
                System.Action onOneDone = () =>
                {
                    done++;
                    if (done >= (top ? 1 : 0) + (bottom ? 1 : 0))
                        DestroyIfExists();
                };

                if (top)
                    StartCoroutine(MoveCo(top, finalRight, moveOutDuration, onOneDone));
                if (bottom)
                    StartCoroutine(MoveCo(bottom, finalRight, moveOutDuration, onOneDone));
                break;
        }
    }

    bool TapPressedThisFrame()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) return true;
        var ts = Touchscreen.current;
        if (ts != null && ts.primaryTouch.press.wasPressedThisFrame) return true;
        return false;
    }

    IEnumerator MoveCo(GameObject obj, Vector3 to, float duration, System.Action onDone)
    {
        if (!obj) { onDone?.Invoke(); yield break; }

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
        obj.transform.position = to;
        onDone?.Invoke();
    }

    void DestroyIfExists()
    {
        if (top) Destroy(top);
        if (bottom) Destroy(bottom);
        top = null; bottom = null;
        phase = Phase.Idle;
    }
}
