using MessageBoardApp.Domain.Entities;

namespace MessageBoardApp.Application.Interfaces
{
    public interface IUsersRepository
    {
        User FindById(uint id);
        User Create(User user);
        User Update(User user);
        void Delete(User user);
    }
}