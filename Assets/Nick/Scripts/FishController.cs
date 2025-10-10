using UnityEngine;
using UnityEngine.UI;

public class FishController : MonoBehaviour
{
    [Header("Positions")]
    public Transform leftPos;
    public Transform middlePos;

    [Header("Prefabs & Visuals")]
    public GameObject fishPrefab;
    GameObject activeFish;

    [Header("Movement Settings")]
    public float moveDuration = 1.2f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("UI Reference")]
    public Button triggerButton;

    float t;
    bool moving;

    void Start()
    {
        if (triggerButton != null)
            triggerButton.onClick.AddListener(MoveToMiddle);
    }

    void Update()
    {
        if (!moving || activeFish == null) return;

        t += Time.deltaTime / moveDuration;
        float eased = easeCurve.Evaluate(t);
        activeFish.transform.position = Vector3.Lerp(leftPos.position, middlePos.position, eased);

        if (t >= 1f)
            moving = false;
    }

    public void MoveToMiddle()
    {
        if (fishPrefab == null || leftPos == null || middlePos == null) return;

        if (activeFish != null)
            Destroy(activeFish);

        activeFish = Instantiate(fishPrefab, leftPos.position, Quaternion.identity);
        t = 0f;
        moving = true;
    }
}
