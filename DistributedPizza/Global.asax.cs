using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DistributedPizza.App_Start;
using DistributedPizza.Core.Data;
using Ninject.Modules;

namespace DistributedPizza
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            NinjectContainer.RegisterModules(new IDistributedPizzaDbContextModule());
        }
    }

    public class IDistributedPizzaDbContextModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDistributedPizzaDbContext>().To<DistributedPizzaDbContext>();
        }
    }
}
