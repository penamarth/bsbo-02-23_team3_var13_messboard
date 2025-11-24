using MessageBoardApp.Domain.Entities;
using MessageBoardApp.Infrastructure.Repositories;
using System.Collections.Generic;

namespace MessageBoardApp.Infrastructure.Services
{
    public class SearchEngine
    {
        private readonly Database _database;

        public SearchEngine(Database database)
        {
            _database = database;
        }

        public List<Advertisement> Search(string query)
        {
            var results =  new List<Advertisement>();

            foreach (var ads in _database.FindAllAdvertisements())
            {
                if (ads.Name.Split(' ').Any(t => t == query))
                    results.Add(ads);
            }
            
            return results;
        }
    }
}