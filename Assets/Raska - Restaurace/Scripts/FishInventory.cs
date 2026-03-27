using System.Collections.Generic;
using UnityEngine;

public class FishInventory : MonoBehaviour
{
    public static FishInventory Instance { get; private set; }

    private readonly List<fish> _inventory = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Call this from your fishing scene when a fish is caught
    public void OnFishCaught(fish fishSO)
    {
        _inventory.Add(fishSO);
        Debug.Log($"[FishInventory] Caught {fishSO.fishname}. Total: {_inventory.Count}");
        TrySpawnOrder();
    }

    // Called by OrderManager when an order times out or is completed
    public void OnOrderRemoved(int orderId)
    {
        OrderManager.Instance.RemoveOrder(orderId);
    }

    private void TrySpawnOrder()
    {
        if (OrderManager.Instance == null || _inventory.Count == 0) return;

        int index = Random.Range(0, _inventory.Count);
        fish fishSO = _inventory[index];

        if (OrderManager.Instance.SpawnOrder(fishSO))
            _inventory.RemoveAt(index);
    }
}