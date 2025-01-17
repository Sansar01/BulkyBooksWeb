using Bulky.DataAccess.Data;
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
    public class OrderHeaderRepository : Repository<OrderHeader>,IOrderHeaderRepository
    {
      private ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        //public void save()
        //{
          
        //}

        public void Update(OrderHeader obj)
        {
            _context.OrderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromdb = _context.OrderHeaders.FirstOrDefault(u => u.Id == id);

            if (orderFromdb != null) 
            { 
              orderFromdb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromdb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderFromdb = _context.OrderHeaders.FirstOrDefault(u => u.Id == id);

            if (!string.IsNullOrEmpty(sessionId)) {
                  orderFromdb.SessionId = sessionId;
            }

            if (!string.IsNullOrEmpty(paymentIntentId)) { 
               orderFromdb.PaymentIntentId = paymentIntentId;
                orderFromdb.PaymentDate = DateTime.Now;
            }
        }
    }
}
