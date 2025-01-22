﻿using Bulky.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class OrderVM
    {
        public IEnumerable<OrderHeader> OrderHeader { get; set; }  
        
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
}
