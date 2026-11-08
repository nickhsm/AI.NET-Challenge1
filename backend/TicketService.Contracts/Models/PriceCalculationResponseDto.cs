namespace TicketService.Contracts.Models;

public class PriceCalculationResponseDto
{
    public decimal OutboundPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string FormattedPrice { get; set; }

    public PriceCalculationResponseDto(decimal outboundPrice, decimal totalPrice)
    {
        OutboundPrice = outboundPrice;
        TotalPrice = totalPrice;
        // Formatteer naar bijv. "€ 5,70"
        FormattedPrice = $"€ {totalPrice:F2}".Replace(".", ",");
    }
}