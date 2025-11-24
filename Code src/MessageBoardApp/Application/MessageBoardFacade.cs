using MessageBoardApp.Domain.Entities;
using MessageBoardApp.Infrastructure.DTO;
using MessageBoardApp.Application.Interfaces;
using MessageBoardApp.Infrastructure.Services;
using MessageBoardApp.Infrastructure.ExternalAPIs;
using System.Collections.Generic;
using System.Linq;

namespace MessageBoardApp.Application
{
    public class MessageBoardFacade
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IAdvertisementsRepository _advertisementsRepository;
        private readonly IDeliveryServiceAdapter _deliveryServiceAdapter;
        private readonly SearchEngine _searchEngine;
        private Dictionary<uint, Order> _orders = new Dictionary<uint, Order>();

        public MessageBoardFacade(
            IUsersRepository usersRepository, 
            IAdvertisementsRepository advertisementsRepository, 
            IDeliveryServiceAdapter deliveryServiceAdapter,
            SearchEngine searchEngine)
        {
            _usersRepository = usersRepository;
            _advertisementsRepository = advertisementsRepository;
            _deliveryServiceAdapter = deliveryServiceAdapter;
            _searchEngine = searchEngine;
        }

        public List<Advertisement> GetUserAdvertisments(uint userId) 
        { 
            Console.WriteLine($"  MessageBoardFacade: GetUserAdvertisments(userId: {userId})");

            var user = _usersRepository.FindById(userId);
            if (user == null)
            {
                Console.WriteLine($"    User not found");
                return new List<Advertisement>();
            }

            Console.WriteLine($"    Getting advertisementIds from user");
            var advertisementIds = user.AdvertisementIds;

            Console.WriteLine($"    Retrieving {advertisementIds.Count} advertisements");
            var advertisements = new List<Advertisement>();
            foreach (var id in advertisementIds)
            {
                var ad = _advertisementsRepository.FindById(id);
                if (ad != null)
                {
                    advertisements.Add(ad);
                }
            }

            return advertisements;
        }

        public Advertisement CreateAdvertisment(uint userId, List<string> values) 
        { 
            Console.WriteLine($"  MessageBoardFacade: CreateAdvertisment(userId: {userId}, values: List<string>)");

            var user = _usersRepository.FindById(userId);
            if (user == null)
            {
                Console.WriteLine($"    User not found");
                return null;
            }

            string name = values[0];
            string description = values[1];
            float cost = float.Parse(values[2]);

            var newAdvertisement = new Advertisement
            {
                Id = (uint)new Random().Next(100, 9999),
                UserId = userId,
                Name = name,
                Description = description,
                Cost = cost,
                Media = new List<byte[]>()
            };

            Console.WriteLine($"    Calling repository to create advertisement");
            var createdAdvertisement = _advertisementsRepository.Create(newAdvertisement);

            if (createdAdvertisement != null)
            {
                Console.WriteLine($"    Appending advertisement ID to user");
                user.AdvertisementIds.Add(createdAdvertisement.Id);
                _usersRepository.Update(user);
                Console.WriteLine($"    Success: Advertisement '{createdAdvertisement.Name}' created with ID: {createdAdvertisement.Id}");
            }

            return createdAdvertisement;
        }

        public List<Advertisement> Search(string query) => _searchEngine.Search(query); 

        public void AddToFavorites(uint userId, uint advertisementId)
        {
            Console.WriteLine($"  MessageBoardFacade: AddToFavorites(userId: {userId}, advertisementId: {advertisementId})");

            var user = _usersRepository.FindById(userId);
            if (user == null)
            {
                Console.WriteLine($"    User not found");
                return;
            }

            Console.WriteLine($"    Adding advertisementId to user.FavouriteIds");
            user.FavouriteIds.Add(advertisementId);
            _usersRepository.Update(user);

            var advertisement = _advertisementsRepository.FindById(advertisementId);
            if (advertisement != null)
            {
                Console.WriteLine($"    Attaching user as observer to advertisement");
                advertisement.Attach(user);
            }

            Console.WriteLine($"    Success: Advertisement added to favorites");
        }

        public void RemoveFromFavorites(uint userId, uint advertisementId)
        {
            Console.WriteLine($"  MessageBoardFacade: RemoveFromFavorites(userId: {userId}, advertisementId: {advertisementId})");

            var user = _usersRepository.FindById(userId);
            if (user == null)
            {
                Console.WriteLine($"    User not found");
                return;
            }

            Console.WriteLine($"    Removing advertisementId from user.FavouriteIds");
            user.FavouriteIds.Remove(advertisementId);
            _usersRepository.Update(user);

            var advertisement = _advertisementsRepository.FindById(advertisementId);
            if (advertisement != null)
            {
                Console.WriteLine($"    Detaching user from advertisement");
                advertisement.Detach(user);
            }

            Console.WriteLine($"    Success: Advertisement removed from favorites");
        }

        public List<Advertisement> GetFavorites(uint userId)
        {
            Console.WriteLine($"  MessageBoardFacade: GetFavorites(userId: {userId})");

            var user = _usersRepository.FindById(userId);
            if (user == null)
            {
                Console.WriteLine($"    User not found");
                return new List<Advertisement>();
            }

            Console.WriteLine($"    Getting favouriteIds from user");
            var favouriteIds = user.FavouriteIds;

            Console.WriteLine($"    Retrieving {favouriteIds.Count} favorite advertisements");
            var favouriteAdvertisements = new List<Advertisement>();
            foreach (var id in favouriteIds)
            {
                var ad = _advertisementsRepository.FindById(id);
                if (ad != null)
                {
                    favouriteAdvertisements.Add(ad);
                }
            }

            return favouriteAdvertisements;
        }

        public Order CreateOrder(uint userId, uint advertismentId)
        {
            Console.WriteLine($"  MessageBoardFacade: CreateOrder(userId: {userId}, advertismentId: {advertismentId})");

            var advertisement = _advertisementsRepository.FindById(advertismentId);
            var user = _usersRepository.FindById(userId);

            var order = new Order(user, advertisement);
            _orders[order.Id] = order;

            return order;
        }

        public void SelectDelivery(uint userId, uint orderId, uint serviceId) 
        { 
            Console.WriteLine($"  MessageBoardFacade: SelectDelivery(userId: {userId}, orderId: {orderId}, serviceId: {serviceId})");
        }

        public DeliveryQuote RequestDeliveryQuote(uint userId, uint orderId, uint serviceId) 
        { 
            Console.WriteLine($"  MessageBoardFacade: RequestDeliveryQuote(userId: {userId}, orderId: {orderId}, serviceId: {serviceId})");
            return _deliveryServiceAdapter.GetDeliveryQuote(orderId, serviceId); 
        }

        public DeliveryQuote ProcessPayment(uint userId, uint orderId) 
        { 
            Console.WriteLine($"  MessageBoardFacade: ProcessPayment(userId: {userId}, orderId: {orderId})");
            return _deliveryServiceAdapter.ProcessPayment(orderId); 
        }

        public Receipt GenerateReceipt(uint userId, uint orderId)
        {
            Console.WriteLine($"  MessageBoardFacade: GenerateReceipt(userId: {userId}, orderId: {orderId})");

            if (_orders.TryGetValue(orderId, out var order))
            {
                var items = new List<string> { "Product 1" };
                var receipt = new Receipt(items, 100.0f, order.TotalSum + 100.0f, order.TotalItems);
                return receipt;
            }

            return null;
        }

        public User CreateUser(List<string> values)
        {
            Console.WriteLine($"  MessageBoardFacade: CreateUser(values: List<string>)");

            string name = values[0];
            string email = values[1];
            string password = values[2];
            string location = values[3];

            var newUser = new User
            {
                UserId = (uint)new Random().Next(1000, 9999),
                Name = name,
                Email = email,
                Location = location,
                AdvertisementIds = new List<uint>(),
                FavouriteIds = new List<uint>(),
                Orders = new List<Order>()
            };

            Console.WriteLine($"    MessageBoardFacade: Create(User: {newUser.Email})");
            var createdUser = _usersRepository.Create(newUser);

            if (createdUser != null)
            {
                Console.WriteLine($"    Success: Пользователь {createdUser.Email} успешно создан (ID: {createdUser.UserId})");
            }

            return createdUser;
        }

    }
}
