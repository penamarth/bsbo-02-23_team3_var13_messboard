using MessageBoardApp.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MessageBoardApp.Infrastructure.Repositories
{
    public class Database
    {
        private List<User> _users = new List<User>();
        private List<Advertisement> _advertisements = new List<Advertisement>();
        private uint _userIdCounter = 1;
        private uint _advertisementIdCounter = 101;

        public User FindUserById(uint id) 
        { 
            return _users.FirstOrDefault(u => u.UserId == id);
        }

        public User InsertUser(User user) 
        { 
            user.UserId = _userIdCounter++;
            _users.Add(user);
            return user;
        }

        public Advertisement FindAdvertisementById(uint id) 
        { 
            return _advertisements.FirstOrDefault(a => a.Id == id);
        }

        public List<Advertisement> FindAllAdvertisements() 
        { 
            return new List<Advertisement>(_advertisements);
        }

        public Advertisement InsertAdvertisement(Advertisement advertisement) 
        { 
            advertisement.Id = _advertisementIdCounter++;
            _advertisements.Add(advertisement);
            return advertisement;
        }

        public List<Advertisement> FindAdvertisementsByIds(List<uint> ids) 
        { 
            return _advertisements.Where(a => ids.Contains(a.Id)).ToList();
        }

        public List<Advertisement> FindAdvertisementsByUserId(uint userId)
        {
            return _advertisements.Where(a => a.UserId == userId).ToList();
        }

        public List<User> GetAllUsers()
        {
            return new List<User>(_users);
        }

        public void PrintDatabase()
        {
            System.Console.WriteLine("\n=== СОДЕРЖИМОЕ БАЗЫ ДАННЫХ ===\n");
            System.Console.WriteLine("ПОЛЬЗОВАТЕЛИ:");
            foreach (var user in _users)
            {
                System.Console.WriteLine($"  ID: {user.UserId}, Имя: {user.Name}, Email: {user.Email}, Локация: {user.Location}");
            }

            System.Console.WriteLine("\nОБЪЯВЛЕНИЯ:");
            foreach (var ad in _advertisements)
            {
                System.Console.WriteLine($"  ID: {ad.Id}, Название: {ad.Name}, Цена: {ad.Cost}₽, Продавец ID: {ad.UserId}");
            }
        }
    }
}
