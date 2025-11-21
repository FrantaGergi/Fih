using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderSlotUI : MonoBehaviour
{
    [Header("Root panels")]
    public GameObject previewPanel;   // fish name for a few seconds (no timer)
    public GameObject badgePanel;     // compact badge with order number
    public GameObject detailPanel;    // expanded card (fish + timer + Close)

    [Header("Preview refs")]
    public TextMeshProUGUI previewText;

    [Header("Badge refs")]
    public Button badgeButton;
    public TextMeshProUGUI badgeNumberText;

    [Header("Detail refs")]
    public TextMeshProUGUI detailFishText;
    public TextMeshProUGUI detailTimerText;
    public Button closeDetailButton;

    [Header("Settings")]
    public float previewSeconds = 2.0f;

    // State
    private Order _order;
    private bool _timerStarted;
    private float _timeLeft;
    private Coroutine _timerCo;

    // External callback
    public System.Action<Order> onTimerExpired;

    void Awake()
    {
        if (badgeButton) badgeButton.onClick.AddListener(OpenDetail);
        if (closeDetailButton) closeDetailButton.onClick.AddListener(CloseDetail);
        SetAllOff();
    }

    void SetAllOff()
    {
        if (previewPanel) previewPanel.SetActive(false);
        if (badgePanel) badgePanel.SetActive(false);
        if (detailPanel) detailPanel.SetActive(false);
    }

    public void Bind(Order order)
    {
        _order = order;
        _timerStarted = false;
        _timeLeft = order.timeLimit;
        if (_timerCo != null) { StopCoroutine(_timerCo); _timerCo = null; }

        // 1) PREVIEW
        if (previewText) previewText.text = $"Order: {order.fishType}";
        previewPanel.SetActive(true);
        badgePanel.SetActive(false);
        detailPanel.SetActive(false);

        StartCoroutine(PreviewThenBadge());
    }

    IEnumerator PreviewThenBadge()
    {
        yield return new WaitForSeconds(previewSeconds);

        // 2) BADGE
        if (badgeNumberText) badgeNumberText.text = _order.orderId.ToString();
        previewPanel.SetActive(false);
        badgePanel.SetActive(true);
        detailPanel.SetActive(false);
    }

    void OpenDetail()
    {
        // 3) DETAIL (start timer on first open)
        if (detailFishText) detailFishText.text = $"{_order.fishType} (#{_order.orderId})";
        UpdateTimerLabel(_timeLeft);
        badgePanel.SetActive(false);
        detailPanel.SetActive(true);

        if (!_timerStarted)
        {
            _timerStarted = true;
            _timerCo = StartCoroutine(RunTimer());
        }
    }

    void CloseDetail()
    {
        // Back to badge; timer keeps running
        detailPanel.SetActive(false);
        badgePanel.SetActive(true);
    }

    IEnumerator RunTimer()
    {
        while (_timeLeft > 0f)
        {
            _timeLeft -= Time.deltaTime;
            if (detailPanel.activeSelf) UpdateTimerLabel(_timeLeft);
            yield return null;
        }

        _timeLeft = 0f;
        if (detailPanel.activeSelf) UpdateTimerLabel(_timeLeft);
        onTimerExpired?.Invoke(_order);

        HideAll();
    }

    void UpdateTimerLabel(float secondsLeft)
    {
        if (!detailTimerText) return;
        secondsLeft = Mathf.Max(0, secondsLeft);
        int s = Mathf.CeilToInt(secondsLeft);
        detailTimerText.text = $"{s / 60:00}:{s % 60:00}";
    }

    public void HideAll() => SetAllOff();
}
