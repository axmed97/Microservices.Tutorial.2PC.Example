namespace Coordinator.Services.Abstract
{
    public interface ITransactionService
    {
        Task<Guid> CreateTransactionAsync();
        Task PrepeareServicesAsync(Guid transactionId);
        Task<bool> CheckReadyServicesAsync(Guid transactionId);
        Task CommitAsync(Guid transactionId);
        Task<bool> CheckTransactionStateServicesAsync(Guid transactionId);
        Task RollbackAsync(Guid transactionId);
    }
}
