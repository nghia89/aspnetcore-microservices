using System;
using Shared.Enums.InVentory;

namespace Shared.DTOs.InVentory
{
    public class SaleItemDto
    {
        public string ItemNo { get; set; }
        public int Quantity { get; set; }
        public EDocumentType DocumentType => EDocumentType.Sale;
    }
}

