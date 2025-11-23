using MessageBoardApp.Application.Interfaces;
using MessageBoardApp.Domain.Entities;
using System.Collections.Generic;

namespace MessageBoardApp.Infrastructure.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly Database _database;

        public UsersRepository(Database database)
        {
            _database = database;
        }

        public User FindById(uint id) 
        { 
            Console.WriteLine($"      UsersRepository: FindById(id: {id})");
            return _database.FindUserById(id); 
        }

        public List<User> FindAll() { return new List<User>(); }

        public User Create(User user) 
        { 
            Console.WriteLine($"      UsersRepository: Create(userId: {user.UserId}, email: {user.Email})");
            return _database.InsertUser(user); 
        }

        public User Update(User user) { return user; }
        public void Delete(User user) { }
    }
}
