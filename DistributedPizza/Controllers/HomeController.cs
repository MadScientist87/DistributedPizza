using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using DistributedPizza.Core;
using DistributedPizza.Core.Data;
using DistributedPizza.Core.Data.Models;
using Ninject;
using RestSharp;

namespace DistributedPizza.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            IKernel _Kernal = new StandardKernel();
            _Kernal.Load(Assembly.GetExecutingAssembly());
            IDistributedPizzaDbContext dbContext = _Kernal.Get<DistributedPizzaDbContext>();
            var toppings = dbContext.Toppings.ToList();
            return View();
        }

        [HttpPost]
        [Route("home/createOneRandomOrder")]
        public ActionResult CreateOneRandomOrder()
        {
            var client = new RestClient("http://localhost/DistributedPizza.Web.Api");
            var request = new RestRequest("api/orders/create", Method.POST);
            request.RequestFormat = DataFormat.Json;
            var response = client.Execute<OrderResponseDTO>(request);
            return Json(response.Data);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}