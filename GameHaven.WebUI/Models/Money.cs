using System.Globalization;

namespace GameHaven.WebUI.Models;

/// <summary>
/// Currency formatting for the storefront.
///
/// The format is pinned rather than taken from the browser culture: prices are
/// authored in euros and every visitor must read the same figure, regardless of
/// their locale or of whether ICU data shipped with the WebAssembly payload.
/// </summary>
public static class Money
{
    private static readonly NumberFormatInfo EuroFormat = new()
    {
        CurrencySymbol = "€",
        CurrencyDecimalSeparator = ",",
        CurrencyGroupSeparator = ".",
        CurrencyDecimalDigits = 2,
        CurrencyPositivePattern = 3, // n $
        CurrencyNegativePattern = 8  // -n $
    };

    public static string Format(decimal amount) => amount.ToString("C", EuroFormat);
}
