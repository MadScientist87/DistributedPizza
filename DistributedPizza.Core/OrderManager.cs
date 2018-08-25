using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data;
using DistributedPizza.Core.Data.Entities;
using Ninject;

namespace DistributedPizza.Core
{
    public class OrderManager
    {
        readonly IDistributedPizzaDbContext distributedPizzaDbContext;
        private readonly BetterRandom random;
        [Inject]
        public OrderManager(IDistributedPizzaDbContext distributedPizzaDbContext, BetterRandom random)
        {
            this.distributedPizzaDbContext = distributedPizzaDbContext;
            this.random = random;
        }

        public Order GenerateRandomOrder()
        {
            var customerManager = new CustomerManager(random);


            var pizzaManger = new PizzaManager(this.distributedPizzaDbContext, random);
            var customer = customerManager.GetRandomCustomer(random.Next(0, 6));
            var randomPizzas = pizzaManger.GetRandomPizzas();

            var order = new Order
            {
                CustomerName = customer.CustomerName,
                CustomerPhone = customer.CustomerPhoneNumber,
                Pizza = randomPizzas
            };

            return order;
        }

        public void GenerateNextOrderNumber(Order order)
        {
            DateTime today = DateTime.Now;
            PrefixSeq rec = GetNextSeq("DBP" + today.ToString("yy") + today.DayOfYear.ToString("D3"));
            order.OrderReferenceId = rec.Prefix + rec.Seq.ToString("00");
        }


        private PrefixSeq GetNextSeq(string prefix)
        {
            var db = this.distributedPizzaDbContext;
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
