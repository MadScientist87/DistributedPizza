using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;
using Stateless;

namespace DistributedPizza.Core.StateMachines
{
    public class PizzaStateMachine : StateMachine<PizzaStatus, Trigger>
    {
        private readonly Pizza _pizza;
        public PizzaStateMachine(Pizza pizza) : base(() => pizza.Status, s => pizza.Status = s)
        {
            this._pizza = pizza;
            Initialize();
        }
        private void Initialize()
        {
            Configure(PizzaStatus.Prep).PermitIf(Trigger.UpdatePizza,
                PizzaStatus.Bake, () => true);
            Configure(PizzaStatus.Bake).PermitIf(Trigger.UpdatePizza,
                PizzaStatus.PackagedForDelivery, () =>true);
        }
    }
}
