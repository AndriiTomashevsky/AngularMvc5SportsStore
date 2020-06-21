using Newtonsoft.Json;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace ServerApp.Controllers
{
    //[EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*", SupportsCredentials = true)]
    public class SessionValuesController : ApiController
    {
        public IHttpActionResult GetCart()
        {
            string jsonData = HttpContext.Current.Session["Cart"]?.ToString();
            object cart = null;

            if (jsonData != null)
            {
                JavaScriptSerializer j = new JavaScriptSerializer();
                cart = j.Deserialize(jsonData, typeof(object));
            }

            return Ok(cart);
        }

        [System.Web.Http.HttpPost]
        public void StoreCart([FromBody] CartProductSelection[] products)
        {
            string jsonData = JsonConvert.SerializeObject(products);
            HttpContext.Current.Session["Cart"] = jsonData;
        }
    }
}
