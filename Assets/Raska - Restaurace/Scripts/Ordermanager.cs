using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Setup")]
    public OrderSlotUI orderSlotPrefab;
    public Transform orderContainer;
    public int maxOrders = 4;
    public float orderTimeLimit = 60f;

    private int _idCounter = 0;
    private readonly Dictionary<int, OrderSlotUI> _activeSlots = new();
    private readonly Queue<OrderSlotUI> _pool = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Returns true if the order was accepted
    public bool SpawnOrder(fish fishSO)
    {
        if (_activeSlots.Count >= maxOrders) return false;

        _idCounter++;
        var order = new Order(_idCounter, fishSO, orderTimeLimit);
        var slot = GetSlot();

        slot.onTimerExpired = OnTimerExpired;
        slot.onFinished = ReturnToPool;
        slot.Bind(order);

        _activeSlots[order.orderId] = slot;
        return true;
    }

    // Call this when the player completes an order
    public void CompleteOrder(int orderId)
    {
        if (_activeSlots.TryGetValue(orderId, out var slot))
        {
            slot.CompleteOrder();
            RemoveOrder(orderId);
        }
    }

    public void RemoveOrder(int orderId)
    {
        _activeSlots.Remove(orderId);
    }

    private void OnTimerExpired(Order order)
    {
        Debug.Log($"[OrderManager] Order #{order.orderId} timed out.");
        RemoveOrder(order.orderId);
    }

    private void ReturnToPool(OrderSlotUI slot)
    {
        foreach (var kv in _activeSlots)
        {
            if (kv.Value == slot) { _activeSlots.Remove(kv.Key); break; }
        }
        slot.gameObject.SetActive(false);
        _pool.Enqueue(slot);
    }

    private OrderSlotUI GetSlot()
    {
        if (_pool.Count > 0)
        {
            var s = _pool.Dequeue();
            s.gameObject.SetActive(true);
            return s;
        }
        return Instantiate(orderSlotPrefab, orderContainer);
    }
}