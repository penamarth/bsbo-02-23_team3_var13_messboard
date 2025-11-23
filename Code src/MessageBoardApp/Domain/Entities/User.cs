using MessageBoardApp.Domain.Interfaces;
using System.Collections.Generic;

namespace MessageBoardApp.Domain.Entities
{
    public class User : IObserver
    {
        public uint UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public byte[] ProfilePicture { get; set; }

        public List<uint> AdvertisementIds { get; set; }
        public List<uint> FavouriteIds { get; set; }
        public List<Order> Orders { get; set; }

        public User()
        {
            AdvertisementIds = new List<uint>();
            FavouriteIds = new List<uint>();
            Orders = new List<Order>();
        }

        public void Action(ISubject subject)
        {
            var ad = subject as Advertisement;
            if (ad != null)
            {
                Console.WriteLine($"  User(ID: {UserId}): Получено уведомление! Статус объявления '{ad.Name}' изменился.");
            }
        }

    }
}