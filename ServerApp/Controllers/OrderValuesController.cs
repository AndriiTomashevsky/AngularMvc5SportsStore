using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Web.Http.Cors;

namespace ServerApp.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*", SupportsCredentials = true)]
    public class OrderValuesController : ApiController
    {
        private DataContext context;

        public OrderValuesController(DataContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Order> GetOrders()
        {
            return context.Orders.Include(o => o.Products).Include(o => o.Payment);
        }

        [HttpPost]
        public void MarkShipped(long id)
        {
            Order order = context.Orders.Find(id);

            if (order != null)
            {
                order.Shipped = true;
                context.SaveChanges();
            }
        }

        [HttpPost]
        public IHttpActionResult CreateOrder([FromBody] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderId = 0;
                order.Shipped = false;
                order.Payment.Total = GetPrice(order.Products);
                ProcessPayment(order.Payment);

                if (order.Payment.AuthCode != null)
                {
                    context.Orders.Add(order);
                    context.SaveChanges();

                    return Ok(new
                    {
                        orderId = order.OrderId,
                        authCode = order.Payment.AuthCode,
                        amount = order.Payment.Total
                    });
                }
                else
                {
                    return BadRequest("Payment rejected");
                }
            }
            return BadRequest(ModelState);
        }
        private decimal GetPrice(IEnumerable<CartLine> lines)
        {
            IEnumerable<long> ids = lines.Select(l => l.ProductId);

            IEnumerable<Product> prods = context.Products.Where(p => ids.Contains(p.ProductId));

            return prods.Select(p => lines.First(l => l.ProductId == p.ProductId).Quantity * p.Price).Sum();
        }
        private void ProcessPayment(Payment payment)
        {
            // integrate your payment system here
            payment.AuthCode = "12345";
        }
    }
}
