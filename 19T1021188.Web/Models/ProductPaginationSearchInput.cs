using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _19T1021188.Web.Models
{
    public class ProductPaginationSearchInput : PaginationSearchInput
    {

        public int CategoryID { get; set; } = 0;

        public int SupplierID { get; set; } = 0;
    }
}