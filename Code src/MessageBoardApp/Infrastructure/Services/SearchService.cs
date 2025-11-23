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
            return new List<Advertisement>();
        }
    }
}