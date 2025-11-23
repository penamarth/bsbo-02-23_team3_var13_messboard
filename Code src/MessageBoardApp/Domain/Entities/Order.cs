using System.Collections.Generic;

namespace MessageBoardApp.Domain.Entities
{
    public class Order
    {
        public uint Id { get; set; }
        public User User { get; set; }
        public float TotalSum { get; set; }
        public uint TotalItems { get; set; }
        public List<uint> ItemIds { get; set; }
        public string Status { get; set; }

        public Order(User user, Advertisement advertisement)
        {
            Id = (uint)new Random().Next(1000, 9999);
            User = user;
            ItemIds = new List<uint>();
            ItemIds.Add(advertisement.Id);
            TotalSum = advertisement.Cost;
            TotalItems = 1;
            Status = "Created";
            Console.WriteLine($"    Order(ID: {Id}): Заказ создан для пользователя(ID: {user.UserId}) на товар(ID: {advertisement.Id}).");
        }
    }
}