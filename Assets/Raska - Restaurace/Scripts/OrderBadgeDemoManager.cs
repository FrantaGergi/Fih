using UnityEngine;

public class OrderBadgeDemoManager : MonoBehaviour
{
    public OrderSlotUI orderSlot;          // assign in Inspector
    public float orderTimeLimit = 60f;

    private int _orderCounter = 0;

    [Header("Random fish pool")]
    public FishType[] fishPool = new[] { FishType.Salmon, FishType.Tuna, FishType.Carp, FishType.Trout, FishType.Cod };

    void Start()
    {
        SpawnOrder();
    }

    void SpawnOrder()
    {
        _orderCounter++;
        var fish = fishPool[Random.Range(0, fishPool.Length)];
        var order = new Order(_orderCounter, fish, orderTimeLimit);

        orderSlot.onTimerExpired = HandleTimedOut;
        orderSlot.Bind(order);
    }

    void HandleTimedOut(Order o)
    {
        // Here: mark bad rating, show angry icon/sound, etc.
        // For demo, spawn next order after 1s:
        Invoke(nameof(SpawnOrder), 1f);
    }
}
