using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LassoLine : MonoBehaviour
{
    [Header("Body")]
    [SerializeField] private RectTransform startPoint;
    [SerializeField] private RectTransform endPoint;

    [Header("Canvas (pokud používáte UI)")]
    [Tooltip("Nechte prázdné pro automatické nalezení")]
    [SerializeField] private Canvas canvas;

    [Header("LineRenderer settings")]
    [SerializeField] private Color lineColor = Color.white;
    [SerializeField, Tooltip("Šíøka èáry v metrech / jednotkách")] private float lineWidth = 0.02f;
    [SerializeField, Tooltip("Poèet segmentù (min 2) — víc segmentù pro køivku")] private int segments = 2;

    private LineRenderer lr;
    private RectTransform canvasRect;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = lr.endWidth = Mathf.Max(0.0001f, lineWidth);
        lr.positionCount = Mathf.Max(2, segments);
        lr.material = new Material(Shader.Find("Sprites/Default")) { hideFlags = HideFlags.DontSave };
        lr.startColor = lr.endColor = lineColor;

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
            canvasRect = canvas.transform as RectTransform;

        // pokud používáte UI canvas, parentujte LineRenderer pod Canvas pro lokální pozice
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // vytvoøíme GameObject parent (tento mùže být childem canvasu) — nastavíme transform parent na canvas tak, aby lokální pozice fungovaly
            transform.SetParent(canvas.transform, false);
            lr.useWorldSpace = false;
        }
        else
        {
            // pro Camera nebo World space použijeme world-space
            lr.useWorldSpace = true;
        }
    }

    void LateUpdate()
    {
        if (startPoint == null || endPoint == null) return;

        // Pokud je canvas ScreenSpace-Overlay nebo Camera, pøevést body na místní souøadnice canvasu a použít local positions
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Pozn.: pro ScreenSpaceOverlay je worldCamera null v RectTransformUtility volání
            Vector2 sScreen = RectTransformUtility.WorldToScreenPoint(null, startPoint.position);
            Vector2 eScreen = RectTransformUtility.WorldToScreenPoint(null, endPoint.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, sScreen, null, out Vector2 sLocal);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eScreen, null, out Vector2 eLocal);

            EnsureCount(2);
            lr.SetPosition(0, sLocal);
            lr.SetPosition(1, eLocal);
        }
        else if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Camera cam = canvas.worldCamera;
            Vector3 sWorld = cam.ScreenToWorldPoint(RectTransformUtility.WorldToScreenPoint(cam, startPoint.position));
            Vector3 eWorld = cam.ScreenToWorldPoint(RectTransformUtility.WorldToScreenPoint(cam, endPoint.position));

            sWorld.z = eWorld.z = 0f; // u UI chceme rovinu; upravte podle potøeby
            EnsureCount(2);
            lr.SetPosition(0, sWorld);
            lr.SetPosition(1, eWorld);
        }
        else
        {
            // World space - použijeme pøímo svìtové pozice
            EnsureCount(2);
            lr.SetPosition(0, startPoint.position);
            lr.SetPosition(1, endPoint.position);
        }
    }

    private void EnsureCount(int count)
    {
        if (lr.positionCount != count)
            lr.positionCount = count;
    }

    // Volitelné veøejné API
    public void SetPoints(RectTransform start, RectTransform end)
    {
        startPoint = start;
        endPoint = end;
    }

    public void SetColor(Color c)
    {
        lineColor = c;
        if (lr != null)
        {
            lr.startColor = lr.endColor = c;
        }
    }

    public void SetWidth(float w)
    {
        lineWidth = w;
        if (lr != null)
            lr.startWidth = lr.endWidth = Mathf.Max(0.0001f, w);
    }
}