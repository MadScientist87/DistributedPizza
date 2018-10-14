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
using DistributedPizza.Core.Queues;
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
        public async Task<IHttpActionResult> Post(OrderDTO orderDTO)
        {
            Order order = _mapper.Map<OrderDTO, Order>(orderDTO);
            order.CreateDate = DateTime.Now;
            BetterRandom random = new BetterRandom();
            var orderManager = new OrderManager(_distributedPizzaDbContext, random);
            orderManager.GenerateNextOrderNumber(order);
            _distributedPizzaDbContext.Orders.Add(order);
            _distributedPizzaDbContext.SaveChanges();

            IStreamProcessingQueue queue = new KafkaStreamProcessing();
            await queue.QueueOrder(order);
            return Ok(new OrderResponseDTO { OrderReferenceId = order.OrderReferenceId });
        }

        [HttpPost]
        [Route("update")]
        public IHttpActionResult Update(OrderDTO orderDTO)
        {
           var order= _distributedPizzaDbContext.Orders.SingleOrDefault(a=>a.Id == orderDTO.Id);

            if (order != null)
            {
                order.Status = orderDTO.Status;

                foreach (var pizzaDTO in orderDTO.Pizza)
                {
                    var pizza=order.Pizza.SingleOrDefault(a => a.Id == pizzaDTO.Id);
                    if (pizza != null) pizza.Status = pizzaDTO.PizzaStatus;
                }
            }

            _distributedPizzaDbContext.SaveChanges();

            return Ok();
        }
    }

}
