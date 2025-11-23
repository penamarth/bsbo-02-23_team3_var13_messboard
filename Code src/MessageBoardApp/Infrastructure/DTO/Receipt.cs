namespace MessageBoardApp.Infrastructure.DTO
{
    using System.Collections.Generic;

    public class Receipt
    {
        public List<string> Items { get; set; }
        public float DeliveryCost { get; set; }
        public float TotalSum { get; set; }
        public uint TotalItems { get; set; }

        public Receipt(List<string> items, float deliveryCost, float totalSum, uint totalItems)
        {
            Items = items;
            DeliveryCost = deliveryCost;
            TotalSum = totalSum;
            TotalItems = totalItems;
            Console.WriteLine("    Receipt: Чек успешно сгенерирован.");
        }
    }
}