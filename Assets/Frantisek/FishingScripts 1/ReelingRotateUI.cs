using UnityEngine;
using UnityEngine.EventSystems;

public class ReelingRotateUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform rect;

    // Konfigurace plynulosti
    [SerializeField, Tooltip("Vyšší = rychlejší následování cílové rotace")] private float smoothSpeed = 15f;
    [SerializeField, Tooltip("Maximální úhel (stupnì) který se akceptuje mezi dvìma snímky, zabraòuje skokùm")] private float maxDeltaPerDrag = 45f;

    // Stav navíjení (pokud false, ikona se nebude otáèet)
    [SerializeField, Tooltip("Zapnout pokud aktuálnì navíjíte (povolí otáèení ikony)")] private bool isReeling = false;

    // Škálování ikony pøi navíjení
    [SerializeField, Tooltip("Škála ikony pøi navíjení")] private float reelingScale = 1.2f;
    [SerializeField, Tooltip("Škála ikony když se nenavíjí")] private float idleScale = 1f;
    [SerializeField, Tooltip("Rychlost plynulého pøepínání škály")] private float scaleSmoothSpeed = 12f;

    // Minimální úhel v stupních, pod kterým se považuje zmìna za šum (nebude považována za otáèení)
    [SerializeField, Tooltip("Minimální úhel (stupnì) pro rozpoznání smìru otáèení")] private float minRotationThreshold = 0.5f;

    private bool isDragging = false;
    private int activePointerId = -1;

    private Vector2 prevLocalPos;
    private float targetAngle; // cílový absolutní úhel
    private float currentAngle; // aktuálnì aplikovaný úhel (pro interpolaci)

    // pro výpoèet úhlové rychlosti
    private float prevAppliedAngle = 0f;

    // škálování internì
    private float targetScale;
    private float currentScale;

    // Veøejné read-only vlastnosti pro další skripty
    public bool IsReeling
    {
        get => isReeling;
        set
        {
            isReeling = value;
            if (!isReeling)
            {
                // pokud pøepneme mimo navíjení, zrušíme aktivní drag/rotaci
                isDragging = false;
                activePointerId = -1;
                IsRotatingClockwise = false;
            }

            // aktualizovat cílovou škálu podle stavu
            targetScale = isReeling ? reelingScale : idleScale;
        }
    }

    // true pouze když se momentálnì detekuje otáèení po smìru hodinových ruèièek
    public bool IsRotatingClockwise { get; private set; } = false;

    // Veøejné telemetry (ètení z jiných skripù)
    public float CurrentAngle => currentAngle;
    public float CurrentAngularSpeed { get; private set; } = 0f; // deg/s

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        currentAngle = rect.localEulerAngles.z;
        targetAngle = currentAngle;
        prevAppliedAngle = currentAngle;

        // inicializovat škálu
        currentScale = rect != null ? rect.localScale.x : 1f;
        targetScale = isReeling ? reelingScale : idleScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Pokud se právì nenavíjí, nedovolíme zaèít otáèet
        if (!isReeling) return;

        // pouze první dotek, který zaèal zde
        isDragging = true;
        activePointerId = eventData.pointerId;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            eventData.position,
            eventData.pressEventCamera,
            out prevLocalPos
        );

        // synchronizovat cílový úhel s aktuálním (zabrání okamžitému skoku)
        targetAngle = currentAngle = rect.localEulerAngles.z;

        // reset smìru pøi novém doteku
        IsRotatingClockwise = false;
        prevAppliedAngle = currentAngle;
        CurrentAngularSpeed = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId) return;

        isDragging = false;
        activePointerId = -1;
        IsRotatingClockwise = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Blokovat otáèení pokud se nenavíjí
        if (!isReeling) return;
        if (!isDragging) return;
        if (eventData.pointerId != activePointerId) return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 currentLocalPos))
        {
            return;
        }

        // Vypoèítat zmìnu úhlu mezi pøedchozím a aktuálním lokálním bodem
        float angleDelta = Vector2.SignedAngle(prevLocalPos, currentLocalPos);

        // Omezit extrémní skoky zpùsobené šumem nebo rychlým gestem
        angleDelta = Mathf.Clamp(angleDelta, -maxDeltaPerDrag, maxDeltaPerDrag);

        // Detekce smìru: v Unity kladné hodnoty SignedAngle znamenají CCW, záporné jsou CW.
        if (Mathf.Abs(angleDelta) >= minRotationThreshold)
        {
            IsRotatingClockwise = angleDelta < 0f;
        }
        else
        {
            // pøíliš malá zmìna = považovat za netoèení
            IsRotatingClockwise = false;
        }

        // Akumulovat do cílového úhlu (absolutní úhel v lokálním prostoru)
        targetAngle += angleDelta;

        prevLocalPos = currentLocalPos;
    }

    void Update()
    {
        // Plynulé pøiblížení k cílovému úhlu
        if (!Mathf.Approximately(currentAngle, targetAngle))
        {
            // Exponenciální smoothing (frame-rate nezávislý)
            float t = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, t);

            rect.localEulerAngles = new Vector3(0f, 0f, currentAngle);
        }
        else
        {
            // Pokud se nic nemìní, žádné otáèení
            IsRotatingClockwise = false;
        }

        // spoèítat úhlovou rychlost (deg/s) z rozdílu aplikovaného úhlu mezi snímky
        float delta = Mathf.DeltaAngle(prevAppliedAngle, currentAngle);
        CurrentAngularSpeed = Mathf.Abs(delta) / Mathf.Max(Time.deltaTime, 1e-6f);
        prevAppliedAngle = currentAngle;

        // Plynulé škálování ikony podle stavu navíjení
        if (!Mathf.Approximately(currentScale, targetScale))
        {
            float tScale = 1f - Mathf.Exp(-scaleSmoothSpeed * Time.deltaTime);
            currentScale = Mathf.Lerp(currentScale, targetScale, tScale);
            rect.localScale = Vector3.one * currentScale;
        }
    }
}
