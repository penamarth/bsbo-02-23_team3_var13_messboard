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
        private List<Advertisement> _advertisments = new List<Advertisement>();

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

            _advertisments = _advertisementsRepository.FindAll();
        }

        public List<Advertisement> GetUserAdvertisments(uint userId) 
        { 
            Console.WriteLine($"  MessageBoardFacade: GetUserAdvertisments(userId: {userId})");

            var user = _usersRepository.FindById(userId);
            if (user == null)
            {
                Console.WriteLine($"    Пользователь не найден");
                return new List<Advertisement>();
            }

            Console.WriteLine($"    Получение объявлений пользователя");
            var advertisementIds = user.AdvertisementIds;

            Console.WriteLine($"    Получено {advertisementIds.Count} объявлений");
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
            Console.WriteLine($"  MessageBoardFacade: CreateAdvertisment(userId: {userId}, values: {values})");

            var user = _usersRepository.FindById(userId);
            if (user == null)
            {
                Console.WriteLine($"    Пользователь не найден");
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

            Console.WriteLine($"    Вызов репозитория для создание объявления");
            var createdAdvertisement = _advertisementsRepository.Create(newAdvertisement);

            if (createdAdvertisement != null)
            {
                Console.WriteLine($"    Добавление ID объявления пользователю");
                user.AdvertisementIds.Add(createdAdvertisement.Id);
                _usersRepository.Update(user);

                _advertisments.Add(createdAdvertisement);

                Console.WriteLine($"    Успех: Объявление '{createdAdvertisement.Name}' создано с ID: {createdAdvertisement.Id}");
            }

            return createdAdvertisement;
        }

        public Advertisement UpdateAdvertisment(uint userId, uint advertismentId, List<string> values)
        {
            Console.WriteLine($"  MessageBoardFacade: UpdateAdvertisment(userId: {userId}, advertismentId: {advertismentId}, values: {values})");

            var advertisement = _advertisementsRepository.FindById(advertismentId);
            if (advertisement == null || advertisement.UserId != userId)
            {
                Console.WriteLine($"    Объявление не найдено или пользователь не является владельцем");
                return null;
            }

            advertisement.Name = values[0];
            advertisement.Description = values[1];
            advertisement.Cost = float.Parse(values[2]);

            var updatedAdvertisement = _advertisementsRepository.Update(advertisement);

            var index = _advertisments.FindIndex(a => a.Id == advertismentId);
            if (index != -1)
            {
                _advertisments[index] = updatedAdvertisement;
            }

            return updatedAdvertisement;
        }

        public void DeleteAdvertisment(uint userId, uint advertismentId)
        {
            Console.WriteLine($"  MessageBoardFacade: DeleteAdvertisment(userId: {userId}, advertismentId: {advertismentId})");

            var advertisement = _advertisementsRepository.FindById(advertismentId);
            if (advertisement == null || advertisement.UserId != userId)
            {
                Console.WriteLine($"    Объявление не найдено или пользователь не является владельцем");
                return;
            }

            _advertisementsRepository.Delete(advertisement);

            _advertisments.RemoveAll(a => a.Id == advertismentId);

            var user = _usersRepository.FindById(userId);
            if (user != null)
            {
                user.AdvertisementIds.Remove(advertismentId);
                _usersRepository.Update(user);
            }
        }

        public List<Advertisement> GetAllAdvertisments()
        {
            Console.WriteLine($"  MessageBoardFacade: GetAllAdvertisments()");
            return _advertisments;
        }

        public List<Advertisement> Search(string query) => _searchEngine.Search(query);

        public void AddToFavorites(uint userId, uint advertisementId)
        {
            Console.WriteLine($"  MessageBoardFacade: AddToFavorites(userId: {userId}, advertisementId: {advertisementId})");

            var user = _usersRepository.FindById(userId);
            if (user == null)
            {
                Console.WriteLine($"    Пользователь не найден");
                return;
            }

            Console.WriteLine($"    Добавление advertisementId к user.FavouriteIds");
            user.FavouriteIds.Add(advertisementId);
            _usersRepository.Update(user);

            var advertisement = _advertisementsRepository.FindById(advertisementId);
            if (advertisement != null)
            {
                Console.WriteLine($"    Добавление пользователя в наблюдатели за объявлением");
                advertisement.Attach(user);
            }

            Console.WriteLine($"    Успех: объявление добавлено в избранные");
        }

        public void RemoveFromFavorites(uint userId, uint advertisementId)
        {
            Console.WriteLine($"  MessageBoardFacade: RemoveFromFavorites(userId: {userId}, advertisementId: {advertisementId})");

            var user = _usersRepository.FindById(userId);
            if (user == null)
            {
                Console.WriteLine($"    Пользователь не найден");
                return;
            }

            Console.WriteLine($"    Удаление advertisementId из user.FavouriteIds");
            user.FavouriteIds.Remove(advertisementId);
            _usersRepository.Update(user);

            var advertisement = _advertisementsRepository.FindById(advertisementId);
            if (advertisement != null)
            {
                Console.WriteLine($"    Отписка пользователя от объявления");
                advertisement.Detach(user);
            }

            Console.WriteLine($"    Успех: объявление удалено из избранных");
        }

        public List<Advertisement> GetFavorites(uint userId)
        {
            Console.WriteLine($"  MessageBoardFacade: GetFavorites(userId: {userId})");

            var user = _usersRepository.FindById(userId);
            if (user == null)
            {
                Console.WriteLine($"    Пользователь не найден");
                return new List<Advertisement>();
            }

            Console.WriteLine($"    Получение favouriteIds из пользователя");
            var favouriteIds = user.FavouriteIds;

            Console.WriteLine($"    Получено {favouriteIds.Count} избранных объявлений");
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

            if (advertisement == null || user == null)
            {
                Console.WriteLine($"    Объявление или пользователь не найдены");
                return null;
            }

            var order = new Order(user, advertisement);

            user.Orders.Add(order);
            _usersRepository.Update(user);

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

            var user = _usersRepository.FindById(userId);
            if (user == null)
            {
                Console.WriteLine($"    Пользователь не найден");
                return null;
            }

            var order = user.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                Console.WriteLine($"    Заказ не найден");
                return null;
            }

            var items = new List<string> { order.ItemIds.Count > 0 ? $"Товар {order.ItemIds[0]}" : "Товар" };
            var receipt = new Receipt(items, 100.0f, order.TotalSum + 100.0f, order.TotalItems);
            return receipt;
        }

        public User CreateUser(List<string> values)
        {
            Console.WriteLine($"  MessageBoardFacade: CreateUser(values: {values})");

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
            var createdUser = _usersRepository.Create(newUser);

            if (createdUser != null)
            {
                Console.WriteLine($"    Успех: Пользователь {createdUser.Email} успешно создан (ID: {createdUser.UserId})");
            }

            return createdUser;
        }

        public User LoadProfile(uint userId)
        {
            Console.WriteLine($"  MessageBoardFacade: LoadProfile(userId: {userId})");
            return _usersRepository.FindById(userId);
        }

        public User UpdateProfile(uint userId, List<string> values)
        {
            Console.WriteLine($"  MessageBoardFacade: UpdateProfile(userId: {userId}, values: {values})");

            var user = _usersRepository.FindById(userId);
            if (user == null)
            {
                Console.WriteLine($"    Пользователь не найден");
                return null;
            }

            user.Name = values[0];
            user.Email = values[1];
            user.Location = values[2];

            return _usersRepository.Update(user);
        }
    }
}
