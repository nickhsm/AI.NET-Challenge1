namespace TicketService.Contracts.Models;

public class PriceCalculationResponseDto
{
    public decimal TotalPrice { get; set; }
    public string FormattedPrice { get; set; }

    public PriceCalculationResponseDto(decimal totalPrice)
    {
        TotalPrice = totalPrice;
        // Formatteer naar bijv. "€ 5,70"
        FormattedPrice = $"€ {totalPrice:F2}".Replace(".", ",");
    }
}