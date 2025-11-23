namespace MessageBoardApp.Domain.Interfaces
{
    public interface IObserver
    {
        void Action(ISubject subject);
    }
}