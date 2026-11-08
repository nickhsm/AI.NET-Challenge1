namespace TicketService.Domain;

public static class TicketPricing
{
    // Price of a single trip for one passenger.
    public const decimal BasePrice = 2.85m;

    public static decimal CalculateOutboundPrice(int passengerCount)
    {
        return BasePrice * passengerCount;
    }

    public static decimal CalculateTotalPrice(int passengerCount, bool isRoundTrip)
    {
        decimal outboundPrice = CalculateOutboundPrice(passengerCount);

        return isRoundTrip ? outboundPrice * 2 : outboundPrice;
    }
}
