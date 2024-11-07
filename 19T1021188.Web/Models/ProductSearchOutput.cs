using _19T1021188.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _19T1021188.Web.Models
{
    public class ProductSearchOutput : PaginationSearchOutput
    {
        public List<Product> Data { get; set; }
    }
}