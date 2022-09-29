using Inventory.Grpc.Client;

namespace Basket.API.GrpcStockServices
{
    public class StockItemGrpcService
    {
        private readonly StockProtoService.StockProtoServiceClient _stockProtoServiceClient;
        public StockItemGrpcService(StockProtoService.StockProtoServiceClient stockProtoServiceClient)
        {
            _stockProtoServiceClient = stockProtoServiceClient ?? throw new ArgumentNullException(nameof(StockItemGrpcService));
        }

        public async Task<StockModel> GetStock(string itemNo)
        {
            try
            {
                var stock = await _stockProtoServiceClient.GetStockAsync(new GetStockRequest { ItemNo = itemNo });
                return stock;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(nameof(ex), nameof(itemNo));
            }
        }
    }
}
