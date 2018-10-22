using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMapper;
using DistributedPizza.Core;
using DistributedPizza.Core.Data;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Core.Data.Models;
using DistributedPizza.Core.Queues;
using Microsoft.AspNet.SignalR;
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
            order.Status = Status.Started;
            order.CreateDate = DateTime.Now;
            BetterRandom random = new BetterRandom();
            var toppings = _distributedPizzaDbContext.Toppings.ToList();
            var orderManager = new OrderManager(toppings, random);
            order.OrderReferenceId = GetNextSeqAPI();
            //order.OrderReferenceId  = Guid.NewGuid().ToString();
            foreach (var pizza in order.Pizza)
            {
                int toppingsTogenerate = random.Next(1, 3);

                for (var x = 0; x < toppingsTogenerate; x++)
                {
                    int index = random.Next(toppings.Count);
                    var topping = toppings[index];
                    pizza.Toppings.Add(topping);
                }
            }

            _distributedPizzaDbContext.Orders.Add(order);
            _distributedPizzaDbContext.SaveChanges();

            IStreamProcessingQueue queue;
            switch (order.QueueType)
            {
                case QueueType.Kafka:
                    queue = new KafkaStreamProcessing();
                    break;
                case QueueType.AmazonSQS:
                    queue = new AmazonSQSProcessingQueue();
                    break;
                default:
                    queue = new KafkaStreamProcessing();
                    break;
            }

            queue.QueueOrder(order);



            return Ok(new OrderResponseDTO { OrderReferenceId = order.OrderReferenceId });
        }

        [HttpPost]
        [Route("update")]
        public IHttpActionResult Update(OrderDTO orderDTO)
        {
            var order = _distributedPizzaDbContext.Orders.SingleOrDefault(a => a.Id == orderDTO.Id);

            if (order != null)
            {
                order.Status = orderDTO.Status;

                foreach (var pizzaDTO in orderDTO.Pizza)
                {
                    var pizza = order.Pizza.SingleOrDefault(a => a.Id == pizzaDTO.Id);
                    if (pizza != null) pizza.Status = pizzaDTO.PizzaStatus;
                }
            }

            _distributedPizzaDbContext.SaveChanges();

            return Ok();
        }

        private string GetNextSeqAPI()
        {
            DateTime today = DateTime.Now;
            var prefix = GetNextSeq("DBP" + today.ToString("yy") + today.DayOfYear.ToString("D3"));
            return  prefix.Prefix + prefix.Seq.ToString("00");

        }

        private PrefixSeq GetNextSeq(string prefix)
        {
            var db = this._distributedPizzaDbContext;
            PrefixSeq rec = db.PrefixSeq.SingleOrDefault(a => a.Prefix == prefix);
            if (rec == null)
                db.PrefixSeq.Add(rec = new PrefixSeq { Prefix = prefix, Seq = 1 });
            else
                rec.Seq += 1;

            try
            {
                db.SaveChanges();
                return rec;
            }
            catch (DbUpdateConcurrencyException)
            {
                db.Refresh(RefreshMode.StoreWins, rec);
                return GetNextSeq(prefix);
            }
            catch (DbUpdateException exc)
            {
                if (!exc.Message.Contains("with unique index 'IX_Prefix_IdType'"))
                    throw;

                return GetNextSeq(prefix);
            }
        }

    }

   
}
