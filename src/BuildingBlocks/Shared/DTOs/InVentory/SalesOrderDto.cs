using System;
namespace Shared.DTOs.InVentory
{
    public class SalesOrderDto
    {
        // Order's Document No
        public string OrderNo { get; set; }
        public List<SaleItemDto> SaleItems { get; set; }
    }
}

