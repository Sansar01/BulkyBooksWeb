﻿using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
      private ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        //public void save()
        //{
          
        //}

        public void Update(Product obj)
        {
            _context.Products.Update(obj);
        }
    }
}