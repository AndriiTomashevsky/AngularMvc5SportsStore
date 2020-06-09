using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductValuesController : ApiController
    {
        private DataContext context;

        public ProductValuesController(DataContext ctx)

        {
            context = ctx;
        }

        [HttpGet]
        [Route("{id}")]
        public Product GetProduct(long id)
        {
            return context.Products.Find(id);
        }
    }
}
