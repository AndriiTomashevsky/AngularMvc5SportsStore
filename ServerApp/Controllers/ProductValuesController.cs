using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using ServerApp.Models.BindingTargets;
using Microsoft.AspNetCore.JsonPatch;

namespace ServerApp.Controllers
{
    public class ProductValuesController : ApiController
    {
        private DataContext context;

        public ProductValuesController(DataContext ctx)

        {
            context = ctx;
        }

        public Product GetProduct(long id)
        {
            Product result = context.Products
            .Include(p => p.Supplier)
            .Include(p => p.Ratings)
            .FirstOrDefault(p => p.ProductId == id);

            if (result != null)
            {
                if (result.Supplier != null)
                {
                    result.Supplier.Products = context.Products
                        .Where(p => p.Supplier.SupplierId == result.ProductId)
                        .ToList()
                        .Select(p => new Product
                        {
                            ProductId = p.ProductId,
                            Name = p.Name,
                            Category = p.Category,
                            Description = p.Description,
                            Price = p.Price,
                        });
                }
                if (result.Ratings != null)
                {
                    foreach (Rating r in result.Ratings)
                    {
                        r.Product = null;
                    }
                }
            }

            return result;
        }

        public IHttpActionResult GetProducts(string category = null, string search = null, bool related = false, bool metadata = false)
        {
            IQueryable<Product> query = context.Products;

            if (!string.IsNullOrWhiteSpace(category))
            {
                string catLower = category.ToLower();
                query = query.Where(p => p.Category.ToLower().Contains(catLower));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower)
                            || p.Description.ToLower().Contains(searchLower));
            }

            if (related)
            {
                query = query.Include(p => p.Supplier).Include(p => p.Ratings);

                List<Product> data = query.ToList();

                data.ForEach(p =>
                {
                    if (p.Supplier != null)
                    {
                        p.Supplier.Products = null;
                    }
                    if (p.Ratings != null)
                    {
                        p.Ratings.ForEach(r => r.Product = null);
                    }
                });

                return metadata ? CreateMetadata(data) : Ok(data);
            }
            else
            {
                return metadata ? CreateMetadata(query) : Ok(query);
            }
        }

        private IHttpActionResult CreateMetadata(IEnumerable<Product> products)
        {
            return Ok(new
            {
                data = products,
                categories = context.Products.Select(p => p.Category).Distinct().OrderBy(c => c)
            });
        }

        [HttpPost]
        public IHttpActionResult CreateProduct([FromBody] ProductData pdata)
        {
            if (ModelState.IsValid)
            {
                Product p = pdata.Product;

                if (p.Supplier != null && p.Supplier.SupplierId != 0)
                {
                    context.Suppliers.Attach(p.Supplier);

                }

                context.Products.Add(p);
                context.SaveChanges();

                return Ok(p.ProductId);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut]
        public IHttpActionResult ReplaceProduct(long id, [FromBody] ProductData pdata)
        {
            if (ModelState.IsValid)
            {
                Product p = pdata.Product;
                Product dbEntry = context.Products.Find(id);

                if (p.Supplier != null && p.Supplier.SupplierId != 0)
                {
                    Supplier dbEntryS = context.Suppliers.Find(pdata.Supplier);

                    if (dbEntryS != null)
                    {
                        context.Suppliers.Attach(dbEntryS);
                    }

                }

                if (dbEntry != null)
                {
                    dbEntry.Category = p.Category;
                    dbEntry.Description = p.Description;
                    dbEntry.Name = p.Name;
                    dbEntry.Price = p.Price;
                }

                context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPatch]
        public IHttpActionResult UpdateProduct(long id, [FromBody]JsonPatchDocument<ProductData> patch)
        {
            Product product = context.Products.Include(p => p.Supplier).First(p => p.ProductId == id);
            ProductData pdata = new ProductData { Product = product };

            patch.ApplyTo(pdata);

            if (product.Supplier != null && product.Supplier.SupplierId != 0)
            {
                Supplier dbEntryS = context.Suppliers.Find(product.Supplier);

                if (dbEntryS != null)
                {
                    context.Suppliers.Attach(dbEntryS);
                }

            }

            if (ModelState.IsValid)
            {
                context.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete]
        public void DeleteProduct(long id)
        {
            Product dbEntry = context.Products.Find(id);

            if (dbEntry != null)
            {
                context.Products.Remove(dbEntry);
            }

            context.SaveChanges();
        }
    }
}
