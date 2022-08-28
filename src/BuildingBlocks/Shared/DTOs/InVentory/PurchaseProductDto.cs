using Shared.Enums.InVentory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.InVentory
{
    public class PurchaseProductDto
    {
        public EDocumentType DocumentType => EDocumentType.Purchase;

        private string _itemNo { get; set; }

        public string GetItemNo() => _itemNo;

        public void SetItemNo(string itemNo)
        {
            _itemNo = itemNo;
        }

        public int Quantity { get; set; }
    }
}
