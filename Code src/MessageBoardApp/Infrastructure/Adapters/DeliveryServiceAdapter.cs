using MessageBoardApp.Application.Interfaces;
using MessageBoardApp.Infrastructure.DTO;
using MessageBoardApp.Infrastructure.ExternalAPIs;

namespace MessageBoardApp.Infrastructure.Adapters
{
    public class DeliveryServiceAdapter : IDeliveryServiceAdapter
    {
        private readonly OrderServiceAPI _orderServiceAPI;

        public DeliveryServiceAdapter(OrderServiceAPI orderServiceAPI)
        {
            _orderServiceAPI = orderServiceAPI;
        }

        public DeliveryQuote GetDeliveryQuote(uint orderId, uint serviceId)
        {
            Console.WriteLine($"    DeliveryServiceAdapter: GetDeliveryQuote(orderId: {orderId}, serviceId: {serviceId})");
            var quote = _orderServiceAPI.GetDeliveryQuote(orderId);
            quote.Name = $"Delivery Service {serviceId}";
            quote.Type = "Express";
            quote.Cost = 150.0f;
            quote.Status = "Доступно";
            return quote;
        }

        public DeliveryQuote ProcessPayment(uint orderId)
        {
            Console.WriteLine($"    DeliveryServiceAdapter: ProcessPayment(orderId: {orderId})");
            var quote = _orderServiceAPI.ProcessPayment(orderId);
            quote.Name = "Обработка платежа";
            quote.Type = "Paid";
            quote.Cost = 250.0f;
            quote.Status = "Одобрено";
            return quote;
        }
    }
}