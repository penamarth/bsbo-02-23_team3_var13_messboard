using MessageBoardApp.Infrastructure.DTO;

namespace MessageBoardApp.Infrastructure.ExternalAPIs
{
    public class OrderServiceAPI
    {
        public DeliveryQuote GetDeliveryQuote(uint orderId) 
        { 
            Console.WriteLine($"      OrderServiceAPI: GetDeliveryQuote(orderId: {orderId})");
            return new DeliveryQuote(); 
        }

        public DeliveryQuote ProcessPayment(uint orderId) 
        { 
            Console.WriteLine($"      OrderServiceAPI: ProcessPayment(orderId: {orderId})");
            return new DeliveryQuote(); 
        }
    }
}