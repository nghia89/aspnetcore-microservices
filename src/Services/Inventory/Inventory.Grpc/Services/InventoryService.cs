using Grpc.Core;
using Inventory.Grpc.Protos;
using Inventory.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Inventory.Grpc.Services
{
    public class InventoryService : StockProtoService.StockProtoServiceBase
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger _logger;
        public InventoryService(IInventoryRepository inventoryRepository, ILogger logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public override async Task<StockModel> GetStock(GetStockRequest request, ServerCallContext context)
        {
            _logger.Information($"BEGIN Get Stock of ItemNo: {request.ItemNo}");
            var stockQuantity = await _inventoryRepository.GetStockQuantity(request.ItemNo);
            var result = new StockModel()
            {
                Quantity = stockQuantity
            };
            _logger.Information($"END Get Stock of ItemNo: {request.ItemNo} - Quantity: {result.Quantity}");

            return result;
        }
    }
}
