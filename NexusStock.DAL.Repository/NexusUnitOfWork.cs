
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;

namespace NexusStock.DAL.Repository
{
    public class NexusUnitOfWork : INexusUnitOfWork
    {
        private readonly NexusStockDbContext _context;

        public NexusUnitOfWork(NexusStockDbContext context)
        {
            _context = context;

            Resources = new ResourceRepository(_context);
            Units = new Repository<Unit>(_context);
            Clients = new Repository<Client>(_context);
            StockBalances = new StockBalanceRepository(_context);
            ReceiptDocuments = new ReceiptDocumentRepository(_context);
            ReceiptItems = new Repository<ReceiptItem>(_context);
            ShipmentDocuments = new ShipmentDocumentRepository(_context);
            ShipmentItems = new Repository<ShipmentItem>(_context);
        }

        public IResourceRepository Resources { get; }
        public IRepository<Unit> Units { get; }
        public IRepository<Client> Clients { get; }
        public IStockBalanceRepository StockBalances { get; }
        public IReceiptDocumentRepository ReceiptDocuments { get; }
        public IRepository<ReceiptItem> ReceiptItems { get; }
        public IShipmentDocumentRepository ShipmentDocuments { get; }
        public IRepository<ShipmentItem> ShipmentItems { get; }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
