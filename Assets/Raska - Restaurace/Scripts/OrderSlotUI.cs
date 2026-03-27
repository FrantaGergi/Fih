using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderSlotUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject previewPanel;
    public GameObject badgePanel;
    public GameObject detailPanel;

    [Header("Preview")]
    public TextMeshProUGUI previewText;

    [Header("Badge")]
    public Button badgeButton;
    public TextMeshProUGUI badgeNumberText;
    public Image badgeBackground;
    public Color badgeNormalColor = new Color(0.12f, 0.33f, 0.38f, 1f);
    public Color badgeUrgentColor = new Color(0.72f, 0.18f, 0.18f, 1f);

    [Header("Detail")]
    public TextMeshProUGUI detailFishText;
    public TextMeshProUGUI detailTimerText;
    public Button closeDetailButton;

    [Header("Timer Bar")]
    public Image timerBarFill;
    public Color timerBarFullColor = new Color(0.25f, 0.78f, 0.55f, 1f);
    public Color timerBarUrgentColor = new Color(0.85f, 0.28f, 0.20f, 1f);

    [Header("Settings")]
    public float previewSeconds = 2f;
    public float urgencyThreshold = 15f;

    public System.Action<Order> onTimerExpired;
    public System.Action<OrderSlotUI> onFinished;

    private Order _order;
    private float _timeLeft, _timeTotal;
    private bool _timerStarted;
    private Coroutine _timerCo, _previewCo;

    void Awake()
    {
        badgeButton?.onClick.AddListener(OpenDetail);
        closeDetailButton?.onClick.AddListener(CloseDetail);
        HideAll();
    }

    public void Bind(Order order)
    {
        _order = order;
        _timeLeft = _timeTotal = order.timeLimit;
        _timerStarted = false;

        StopAll();
        if (previewText) previewText.text = $"New order!\n{order.fishSO.fishname}";

        HideAll();
        if (previewPanel) previewPanel.SetActive(true);
        _previewCo = StartCoroutine(PreviewThenBadge());
    }

    public void CompleteOrder() { StopAll(); HideAll(); onFinished?.Invoke(this); }
    public void HideAll()
    {
        if (previewPanel) previewPanel.SetActive(false);
        if (badgePanel) badgePanel.SetActive(false);
        if (detailPanel) detailPanel.SetActive(false);
    }

    IEnumerator PreviewThenBadge()
    {
        yield return new WaitForSeconds(previewSeconds);
        if (badgeNumberText) badgeNumberText.text = _order.orderId.ToString();
        if (badgeBackground) badgeBackground.color = badgeNormalColor;
        HideAll();
        if (badgePanel) badgePanel.SetActive(true);
        OpenDetail();
    }

    void OpenDetail()
    {
        if (detailFishText) detailFishText.text = $"{_order.fishSO.fishname}  #{_order.orderId}";
        UpdateTimerLabel(_timeLeft);
        UpdateTimerBar(_timeLeft);
        if (badgePanel) badgePanel.SetActive(false);
        if (detailPanel) detailPanel.SetActive(true);
        if (!_timerStarted) { _timerStarted = true; _timerCo = StartCoroutine(RunTimer()); }
    }

    void CloseDetail()
    {
        if (detailPanel) detailPanel.SetActive(false);
        if (badgePanel) badgePanel.SetActive(true);
    }

    IEnumerator RunTimer()
    {
        while (_timeLeft > 0f)
        {
            _timeLeft = Mathf.Max(0f, _timeLeft - Time.deltaTime);
            if (detailPanel && detailPanel.activeSelf) { UpdateTimerLabel(_timeLeft); UpdateTimerBar(_timeLeft); }
            if (badgeBackground) badgeBackground.color = _timeLeft <= urgencyThreshold ? badgeUrgentColor : badgeNormalColor;
            yield return null;
        }
        onTimerExpired?.Invoke(_order);
        onFinished?.Invoke(this);
        HideAll();
    }

    void UpdateTimerLabel(float t)
    {
        if (!detailTimerText) return;
        int s = Mathf.CeilToInt(t);
        detailTimerText.text = $"{s / 60:00}:{s % 60:00}";
        detailTimerText.color = t <= urgencyThreshold ? timerBarUrgentColor : Color.white;
    }

    void UpdateTimerBar(float t)
    {
        if (!timerBarFill) return;
        float r = _timeTotal > 0 ? t / _timeTotal : 0f;
        timerBarFill.fillAmount = r;
        timerBarFill.color = Color.Lerp(timerBarUrgentColor, timerBarFullColor, r);
    }

    void StopAll()
    {
        if (_timerCo != null) { StopCoroutine(_timerCo); _timerCo = null; }
        if (_previewCo != null) { StopCoroutine(_previewCo); _previewCo = null; }
    }
}