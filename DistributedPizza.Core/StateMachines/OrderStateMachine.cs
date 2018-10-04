using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;
using Stateless;

namespace DistributedPizza.Core.StateMachines
{
    public class OrderStateMachine : StateMachine<Status, Trigger>
    {
        private readonly Order _order;
        public OrderStateMachine(Order order) : base(() => order.Status, s => order.Status = s)
        {
            this._order = order;
            Initialize();
        }
        private void Initialize()
        {
            Configure(Status.Started).PermitIf(Trigger.UpdateOrder,
                Status.ReadyForDelivery, () => _order.Pizza.Any(a => a.Status == PizzaStatus.PackagedForDelivery));
            Configure(Status.ReadyForDelivery).PermitIf(Trigger.UpdateOrder,
                Status.Delivering, () => _order.Pizza.All(a => a.Status == PizzaStatus.PackagedForDelivery));
            Configure(Status.Delivering).PermitIf(Trigger.UpdateOrder,
                Status.Delivered, () => true);  
        }
    }
}
