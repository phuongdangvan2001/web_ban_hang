﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _19T1021188.DomainModels;

namespace _19T1021188.Web.Models
{
    public class ProductEditModel : Product
    {
        public List<ProductPhoto> LPhoto { get; set; }

        public List<ProductAttribute> LAttribute { get; set; }
    }
}