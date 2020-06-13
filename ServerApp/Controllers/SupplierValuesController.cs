using ServerApp.Models;
using ServerApp.Models.BindingTargets;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerApp.Controllers
{
    public class SupplierValuesController : ApiController
    {
        private DataContext context;

        public SupplierValuesController(DataContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Supplier> GetSuppliers()
        {
            return context.Suppliers;
        }

        [HttpPost]
        public IHttpActionResult CreateSupplier([FromBody]SupplierData sdata)
        {
            if (ModelState.IsValid)
            {
                Supplier s = sdata.Supplier;

                context.Suppliers.Add(s);
                context.SaveChanges();

                return Ok(s.SupplierId);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut]
        public IHttpActionResult ReplaceSupplier(long id, [FromBody] SupplierData sdata)
        {
            if (ModelState.IsValid)
            {
                Supplier s = sdata.Supplier;
                Supplier dbEntry = context.Suppliers.Find(id);

                if (dbEntry != null)
                {
                    dbEntry.City = s.City;
                    dbEntry.Name = s.Name;
                    dbEntry.State = s.State;
                }

                context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
