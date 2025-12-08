using MessageBoardApp.Application.Interfaces;
using MessageBoardApp.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MessageBoardApp.Infrastructure.Repositories
{
    public class AdvertisementsRepository : IAdvertisementsRepository
    {
        private readonly Database _database;

        public AdvertisementsRepository(Database database)
        {
            _database = database;
        }

        public Advertisement FindById(uint id) 
        { 
            Console.WriteLine($"      AdvertisementsRepository: FindById(id: {id})");
            return _database.FindAdvertisementById(id); 
        }

        public List<Advertisement> FindAll() 
        { 
            Console.WriteLine($"      AdvertisementsRepository: FindAll()");
            return _database.FindAllAdvertisements(); 
        }

        public Advertisement Create(Advertisement advertisement) 
        { 
            Console.WriteLine($"      AdvertisementsRepository: Create(advertisment(advertisementId: {advertisement.Id}, name: {advertisement.Name}, ...))");
            return _database.InsertAdvertisement(advertisement); 
        }

        public List<Advertisement> FindByIds(List<uint> ids)
        {
            Console.WriteLine($"      AdvertisementsRepository: FindByIds(count: {ids.Count})");
            return _database.FindAdvertisementsByIds(ids);
        }

        public Advertisement Update(Advertisement advertisement) { return advertisement; }
        public void Delete(Advertisement advertisement) { }
    }
}
