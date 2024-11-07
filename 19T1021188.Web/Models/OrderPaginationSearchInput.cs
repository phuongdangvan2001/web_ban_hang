using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _19T1021188.Web.Models
{
    public class OrderPaginationSearchInput : PaginationSearchInput
    {
        public int status { get; set; } = 0;

    }
}