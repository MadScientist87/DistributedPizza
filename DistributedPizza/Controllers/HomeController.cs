using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
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
        [System.Web.Mvc.Route("home/createRandomOrder")]
        public ActionResult CreateRandomOrder(OrderInfoDTO orderInfoDTO)
        {
            IRestResponse<OrderResponseDTO> response = null;
            var toppings = _distributedPizzaDbContext.Toppings.ToList();
            BetterRandom random = new BetterRandom();

            for (int i = 0; i < orderInfoDTO.NumberOfRequests; i++)
            {
                var orderManager = new OrderManager(toppings, random);
                var order = orderManager.GenerateRandomOrder();
                order.QueueType = orderInfoDTO.QueueType;
                order.ReportBackToClient = orderInfoDTO.NumberOfRequests == 1;
                var client = new RestClient("http://localhost/DistributedPizza.Web.Api");
                var request = new RestRequest("api/orders/create", Method.POST);
                request.AddHeader("Accept", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddBody(order);
                response = client.Execute<OrderResponseDTO>(request);
            }

            if (orderInfoDTO.NumberOfRequests == 1 & response != null)
            {
                response.Data.NumberOfRequests = orderInfoDTO.NumberOfRequests;
                return Json(response.Data);
            }
            else
            {
                return Json(new OrderResponseDTO { NumberOfRequests = orderInfoDTO.NumberOfRequests });
            }
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