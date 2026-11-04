namespace PassengerTicketService.Messages
{
    public class CartUpdatedMessage
    {
        public int ItemCount { get; }
        public CartUpdatedMessage(int itemCount) => ItemCount = itemCount;
    }
}