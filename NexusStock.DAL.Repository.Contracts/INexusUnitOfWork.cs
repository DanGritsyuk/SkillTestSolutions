using NexusStock.Common.Entities;

namespace NexusStock.DAL.Repository.Contracts
{
    public interface INexusUnitOfWork
    {
        IResourceRepository Resources { get; }
        IRepository<Unit> Units { get; }
        IRepository<Client> Clients { get; }
        IStockBalanceRepository StockBalances { get; }
        IReceiptDocumentRepository ReceiptDocuments { get; }
        IRepository<ReceiptItem> ReceiptItems { get; }
        IShipmentDocumentRepository ShipmentDocuments { get; }
        IRepository<ShipmentItem> ShipmentItems { get; }

        Task<int> CompleteAsync();
    }
}
