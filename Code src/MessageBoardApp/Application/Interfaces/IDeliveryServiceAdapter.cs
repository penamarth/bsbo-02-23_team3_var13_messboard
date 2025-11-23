using MessageBoardApp.Infrastructure.DTO;

namespace MessageBoardApp.Application.Interfaces
{
    public interface IDeliveryServiceAdapter
    {
        DeliveryQuote GetDeliveryQuote(uint orderId, uint serviceId);
        DeliveryQuote ProcessPayment(uint orderId);
    }
}