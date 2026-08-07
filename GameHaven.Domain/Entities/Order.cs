namespace GameHaven.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime PurchaseDate { get; private set; }

    private readonly List<OrderLine> _lines = new();
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    private Order() { }

    public Order(Guid userId, decimal totalAmount)
    {
        UserId = userId;
        TotalAmount = totalAmount;
        PurchaseDate = DateTime.UtcNow;
    }

    public void AddLine(Guid gameId, decimal price)
    {
        _lines.Add(new OrderLine(Id, gameId, price));
    }
}

public class OrderLine
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid GameId { get; private set; }
    public decimal PurchasePrice { get; private set; }

    private OrderLine() { }

    internal OrderLine(Guid orderId, Guid gameId, decimal purchasePrice)
    {
        OrderId = orderId;
        GameId = gameId;
        PurchasePrice = purchasePrice;
    }
}
