using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data;

namespace DistributedPizza.Core
{
    public class NinjectBindings : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<IDistributedPizzaDbContext>().To<DistributedPizzaDbContext>();
        }
    }
}
