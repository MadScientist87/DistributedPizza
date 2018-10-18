using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using DistributedPizza.Core;
using DistributedPizza.Core.Data;
using DistributedPizza.Core.Data.Models;
using DistributedPizza.Web.Controllers;
using DistributedPizza.Web.Controllers;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Ninject;
using RestSharp;

namespace DistributedPizza.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDistributedPizzaDbContext _distributedPizzaDbContext;
        public HomeController(IDistributedPizzaDbContext distributedPizzaDbContext)
        {
            _distributedPizzaDbContext = distributedPizzaDbContext;
        }
        public ActionResult Index()
        {
            var toppings = _distributedPizzaDbContext.Toppings.ToList();
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Route("home/createOneRandomOrder")]
        public ActionResult CreateOneRandomOrder()
        {
            BetterRandom random = new BetterRandom();
            var orderManager = new OrderManager(_distributedPizzaDbContext, random);
            var order = orderManager.GenerateRandomOrder();
            var client = new RestClient("http://localhost/DistributedPizza.Web.Api");
            var request = new RestRequest("api/orders/create", Method.POST);
            request.AddHeader("Accept", "application/json");
            request.RequestFormat = DataFormat.Json;
            request.AddBody(order);
            var response = client.Execute<OrderResponseDTO>(request);
            return Json(response.Data);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Route("home/cleanUpDatabase")]
        public ActionResult CleanUpDatabase()
        {
            var deleteSql = @"Delete from PizzaToppings
                Delete from Pizza
                Delete from [Order]";
            _distributedPizzaDbContext.Database.ExecuteSqlCommand(deleteSql);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("home/BroadCastMessage")]
        public ActionResult BroadCastMessage(string message)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hub.Clients.All.broadcastMessage(message);
            return new HttpStatusCodeResult(200);
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