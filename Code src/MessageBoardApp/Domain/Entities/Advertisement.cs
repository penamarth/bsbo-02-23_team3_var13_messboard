using MessageBoardApp.Domain.Interfaces;
using System.Collections.Generic;

namespace MessageBoardApp.Domain.Entities
{
    public class Advertisement : ISubject
    {
        public uint Id { get; set; }
        public uint UserId { get; set; }
        public string Name { get; set; }
        public float Cost { get; set; }
        public string Description { get; set; }
        public List<byte[]> Media { get; set; }

        private readonly List<IObserver> _observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            var user = observer as User;
            Console.WriteLine($"      Advertisement(ID:{Id}): Пользователь(ID:{user?.UserId}) подписался на обновления.");
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify()
        {
            Console.WriteLine($"      Advertisement(ID: {Id}): Уведомляю {_observers.Count} подписчиков об изменениях.");
            foreach (var observer in _observers)
            {
                observer.Action(this);
            }
        }
    }
}