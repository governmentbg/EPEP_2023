namespace Epep.Core.Contracts
{
    public interface IDocumentCleanerService
    {
        Task Clean();
        Task Notify();
    }
}
