using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DistributedPizza.Core;
using DistributedPizza.Core.Data;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Core.Data.Models;

namespace DistributedPizza.Web.Api.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {

        private readonly IDistributedPizzaDbContext _distributedPizzaDbContext;
        public OrdersController(IDistributedPizzaDbContext distributedPizzaDbContext)
        {
            _distributedPizzaDbContext = distributedPizzaDbContext;
        }

        [HttpPost]
        [Route("create")]
        public IHttpActionResult Post()
        {
            BetterRandom random = new BetterRandom();
            var orderManager = new OrderManager(_distributedPizzaDbContext, random);
            var order = orderManager.GenerateRandomOrder();
            _distributedPizzaDbContext.Orders.Add(order);
            _distributedPizzaDbContext.SaveChanges();
            return Ok(new OrderResponseDTO {OrderReferenceId = order.OrderReferenceId});
        }
    }
}
