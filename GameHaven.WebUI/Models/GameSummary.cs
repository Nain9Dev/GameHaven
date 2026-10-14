namespace GameHaven.WebUI.Models;

/// <summary>Storefront projection of a catalogue entry, as returned by /api/games.</summary>
public sealed class GameSummary
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public int DiscountPercentage { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int RatingPercentage { get; set; }
    public DateTime ReleaseDate { get; set; }

    public bool HasDiscount => DiscountPercentage > 0;

    /// <summary>Steam-style wording for the community score.</summary>
    public string RatingLabel => RatingPercentage switch
    {
        >= 95 => "Extremadamente positivas",
        >= 85 => "Muy positivas",
        >= 75 => "Positivas",
        >= 60 => "Mayormente positivas",
        _ => "Variadas"
    };
}

/// <summary>A line in the demo cart. Price is captured when the item is added.</summary>
public sealed class CartLine
{
    public Guid GameId { get; set; }
    public decimal PriceAtAddedTime { get; set; }
}
