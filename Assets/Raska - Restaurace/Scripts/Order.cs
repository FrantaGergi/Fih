[System.Serializable]
public class Order
{
    public int orderId;
    public fish fishSO;
    public float timeLimit;

    public Order(int orderId, fish fishSO, float timeLimit = 60f)
    {
        this.orderId = orderId;
        this.fishSO = fishSO;
        this.timeLimit = timeLimit;
    }
}