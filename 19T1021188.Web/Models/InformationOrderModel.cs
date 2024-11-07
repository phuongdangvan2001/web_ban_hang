using _19T1021188.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _19T1021188.Web.Models
{
    public class InformationOrderModel : Order
    {
        public string OrderName { get; set; } = "";

        public string StatusDescription { get; set; } = "";
        public List<OrderDetail> orderDetails { get; set; }
    }
}