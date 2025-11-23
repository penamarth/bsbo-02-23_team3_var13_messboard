using MessageBoardApp.Domain.Entities;
using System.Collections.Generic;

namespace MessageBoardApp.Application.Interfaces
{
    public interface IAdvertisementsRepository
    {
        Advertisement FindById(uint id);
        List<Advertisement> FindAll();
        Advertisement Create(Advertisement advertisement);
        Advertisement Update(Advertisement advertisement);
        void Delete(Advertisement advertisement);
    }
}