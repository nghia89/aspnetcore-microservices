using System;
namespace Shared.DTOs.InVentory
{
    public class CreatedSalesOrderSuccessDto
    {
        public string DocumentNo { get; }

        public CreatedSalesOrderSuccessDto(string documentNo)
        {
            DocumentNo = documentNo;
        }
    }
}

