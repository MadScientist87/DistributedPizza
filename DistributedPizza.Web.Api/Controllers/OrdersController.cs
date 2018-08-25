using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using DistributedPizza.Core;
using DistributedPizza.Core.Data;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Core.Data.Models;
using Newtonsoft.Json;

namespace DistributedPizza.Web.Api.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {

        private readonly IDistributedPizzaDbContext _distributedPizzaDbContext;
        private readonly IMapper _mapper;
        public OrdersController(IDistributedPizzaDbContext distributedPizzaDbContext, IMapper mapper)
        {
            _distributedPizzaDbContext = distributedPizzaDbContext;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("create")]
        public IHttpActionResult Post(OrderDTO orderDTO)
        {
            Order order = _mapper.Map<OrderDTO, Order>(orderDTO);
            BetterRandom random = new BetterRandom();
            var orderManager = new OrderManager(_distributedPizzaDbContext, random);
            orderManager.GenerateNextOrderNumber(order);
            _distributedPizzaDbContext.Orders.Add(order);
            _distributedPizzaDbContext.SaveChanges();
            return Ok(new OrderResponseDTO { OrderReferenceId = order.OrderReferenceId });
        }
    }

}
