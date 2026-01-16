using UnityEngine;

public class FloatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform targetRect; // obrazek splávku
    [SerializeField] private RectTransform startT;     // start transform (obsahuje scale a Y)
    [SerializeField] private RectTransform endT;       // end transform (obsahuje scale a Y)
    [SerializeField] private ReelingRotateUI reelingSource; // zdroj stavu/režimu navíjení
    [SerializeField] private CatchPopup catchPopup; // volitelný popup pro zobrazení chycení

    [Header("Progress / speed")]
    [SerializeField, Tooltip("Max úhlová rychlost (deg/s) která odpovídá plnému posunu (progress = 1)")]
    private float maxAngularSpeed = 360f;
    [SerializeField, Tooltip("Násobitel rychlosti akumulace progressu")]
    private float progressMultiplier = 1.5f;
    [SerializeField, Tooltip("Rychlost návratu progressu na 0 když se nenavíjí nebo otáèí špatným smìrem")]
    private float progressDecaySpeed = 0.8f;
    [SerializeField, Tooltip("Rychlost vyhlazení aplikovaného progressu (vìtší = rychlejší)")]
    private float progressSmooth = 8f;
    [SerializeField, Tooltip("Prahová hodnota appliedProgress pro vyvolání OnCatchUp")] private float catchThreshold = 0.995f;

    [Header("Position X oscillation")]
    [SerializeField, Tooltip("Max hodnoty oscilace na ose X (pixels)")] private float xRange = 250f;
    [SerializeField, Tooltip("Rychlost oscilace osy X (Hz)")] private float xSpeed = 0.3f;
    [SerializeField, Tooltip("Rychlost vyhlazení pozice (vìtší = rychlejší)")] private float positionSmooth = 8f;

    [Header("Jolt (cuknutí) pøi startu navíjení")]
    [SerializeField, Tooltip("Svislé posunutí splávku pøi cuknutí (pixely, kladné = dolù)")] private float joltDistance = 30f;
    [SerializeField, Tooltip("Délka cuknutí v sekundách")] private float joltDuration = 0.5f;

    // Interní
    private float progress = 0f; // 0..1 mezi start a end
    private float appliedProgress = 0f; // vyhlazený progress použitý pro Lerp
    private bool hasCaught = false; // zabrání opakovanému volání OnCatchUp

    // jolt interní
    private bool isJolting = false;
    private float joltTimer = 0f;
    private bool prevIsReeling = false;

    // Veøejné pro ètení z jiných skriptù
    public float Progress => progress;
    public float AppliedProgress => appliedProgress;

    void Awake()
    {
        if (targetRect == null)
            Debug.LogWarning("FloatController: targetRect není nastavený.", this);

        if (startT == null || endT == null)
            Debug.LogWarning("FloatController: startT nebo endT nejsou nastavené.", this);

        if (reelingSource == null)
            Debug.LogWarning("FloatController: reelingSource není nastavený (nebudu èíst IsReeling/IsRotatingClockwise).", this);

        // aplikovat start hodnoty okamžitì
        ResetToStartImmediate();
        prevIsReeling = reelingSource != null && reelingSource.IsReeling;
    }

    private void StartCatching()
    {
        if (reelingSource != null)
            reelingSource.IsReeling = true;
    }
    private void Start()
    {
        Invoke(nameof(StartCatching), Random.Range(3f, 15f));
    }

    void Update()
    {
        float dt = Time.deltaTime;
        float angularSpeed = 0f;

        // Získat úhlovou rychlost pøímo z reelingSource (pokud je dostupná)
        if (reelingSource != null)
        {
            angularSpeed = reelingSource.CurrentAngularSpeed;
        }

        // DETEKCE PØEPNUTÍ isReeling -> pokud nastane z false na true, spustit jolt
        bool currentIsReeling = reelingSource != null && reelingSource.IsReeling;
        if (!prevIsReeling && currentIsReeling)
        {
            StartJolt();
        }
        prevIsReeling = currentIsReeling;

        // Pokud je reeling aktivní a otáèí správným smìrem, navyšujeme progress podle normalizované rychlosti
        bool canAdvance = reelingSource != null && reelingSource.IsReeling && reelingSource.IsRotatingClockwise;

        if (canAdvance && angularSpeed > 0f)
        {
            float normalized = Mathf.Clamp01(angularSpeed / Mathf.Max(1e-6f, maxAngularSpeed));
            float deltaProgress = normalized * progressMultiplier * dt;
            progress = Mathf.Clamp01(progress + deltaProgress);
        }
        else
        {
            // ubývání progressu pokud se nenavíjí nebo otáèí opaènì
            progress = Mathf.MoveTowards(progress, 0f, progressDecaySpeed * dt);
        }

        // vyhladit aplikovaný progress (pro plynulé Lerpování)
        appliedProgress = Mathf.Lerp(appliedProgress, progress, 1f - Mathf.Exp(-progressSmooth * dt));

        // Update jolt timer pokud probíhá
        if (isJolting)
        {
            joltTimer += dt;
            if (joltTimer >= joltDuration)
            {
                isJolting = false;
                joltTimer = 0f;
            }
        }

        // Lerpovat scale mezi start a end podle appliedProgress a aktualizovat pozici
        if (startT != null && endT != null && targetRect != null)
        {
            Vector3 desiredScale = Vector3.Lerp(startT.localScale, endT.localScale, appliedProgress);
            targetRect.localScale = Vector3.Lerp(targetRect.localScale, desiredScale, 1f - Mathf.Exp(-progressSmooth * dt));

            // Y pozice (anchoredPosition.y) mezi start a end
            float targetY = Mathf.Lerp(startT.anchoredPosition.y, endT.anchoredPosition.y, appliedProgress);

            // Aplikovat jolt offset (sinusový prùbìh: dolù a zpìt)
            float joltOffset = 0f;
            if (isJolting)
            {
                float phase = Mathf.Clamp01(joltTimer / joltDuration);
                // Sinusový tvar 0->1->0: sin(pi * t)
                joltOffset = -joltDistance * Mathf.Sin(phase * Mathf.PI);
            }

            // X oscilace v rozmezí -xRange..xRange
            float xOsc = Mathf.Sin(Time.time * xSpeed * 2f * Mathf.PI) * Mathf.Clamp(xRange, 0f, 250f);

            Vector2 desiredPos = new Vector2(xOsc, targetY + joltOffset);
            targetRect.anchoredPosition = Vector2.Lerp(targetRect.anchoredPosition, desiredPos, 1f - Mathf.Exp(-positionSmooth * dt));
        }

        // Pokud jsme dosáhli (nebo velmi blízko) koncového stavu, zavolat OnCatchUp jednou
        if (!hasCaught && appliedProgress >= catchThreshold)
        {
            hasCaught = true;
            OnCatchUp();
        }
    }


    private void StartJolt()
    {
        isJolting = true;
        joltTimer = 0f;
    }

    public void OnCatchUp()
    {
        // TODO: zde doplnit logiku co se má stát pøi úspìšném "catch" (pøidat rybu do inventáøe apod.)
        // Pokud žádná další logika, bude následovat reset stavu.
        OnReset();
        catchPopup?.ShowCatch("Chytil nìjakou rybu!");


    }

    private void OnReset()
    {
        // kompletní reset stavù tak, aby se vše vrátilo do výchozího (start) stavu
        progress = 0f;
        appliedProgress = 0f;
        hasCaught = false;
        isJolting = false;
        joltTimer = 0f;
        if (reelingSource != null) reelingSource.IsReeling = false;

        ResetToStartImmediate();
        Invoke(nameof(StartCatching), Random.Range(3f, 15f));
    }

    private void ResetToStartImmediate()
    {
        if (targetRect == null || startT == null) return;

        // škála na start
        targetRect.localScale = startT.localScale;

        // pozice Y na start, X podle start nebo v rámci rozsahu
        Vector2 anchor = targetRect.anchoredPosition;
        anchor.y = startT.anchoredPosition.y;
        anchor.x = Mathf.Clamp(startT.anchoredPosition.x, -Mathf.Abs(xRange), Mathf.Abs(xRange));
        targetRect.anchoredPosition = anchor;
    }
}