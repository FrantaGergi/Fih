[System.Serializable]
public class Order
{
    public int orderId;
    public FishType fishType;
    public float timeLimit;

    public Order(int orderId, FishType fishType, float timeLimit = 60f)
    {
        this.orderId = orderId;
        this.fishType = fishType;
        this.timeLimit = timeLimit;
    }
}
