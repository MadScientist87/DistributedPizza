using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DistributedPizza.Core.Data;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using Ninject.Web.Mvc;

namespace DistributedPizza.App_Start
{
    public class NinjectResolver : IDependencyResolver
    {
        /// <summary>
        /// Represents the Core Kernel of the IOC Container
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        /// Creates a new Instance of th Ninject Resolver with the modules Supplied
        /// </summary>
        /// <param name="modules">Modules to Load</param>
        public NinjectResolver(params NinjectModule[] modules)
        {
            Kernel = new StandardKernel(modules);
        }

        /// <summary>
        /// Creates a new Instance of th Ninject Resolver by loading Modules
        /// from the Assembly Supplied
        /// </summary>
        /// <param name="assembly">Assembly to Load Modules</param>
        public NinjectResolver(Assembly assembly)
        {
            Kernel = new StandardKernel();

            try
            {
                Kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                Kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                RegisterServices(Kernel);
            }
            catch
            {
                Kernel.Dispose();
                throw;
            }

            Kernel.Load(assembly);
        }

        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IDistributedPizzaDbContext>().To<DistributedPizzaDbContext>();
        }

        /// <summary>
        /// Creates an Instance of an Object based on the Type information Supplied
        /// </summary>
        /// <param name="serviceType">Type of Object to resolve</param>
        /// <returns>Instance of the Object</returns>
        public object GetService(Type serviceType)
        {
            return Kernel.TryGet(serviceType);
        }

        /// <summary>
        /// Returns all the instance based on the bindings of that type
        /// </summary>
        /// <param name="serviceType">Type to create Instances of</param>
        /// <returns>Instances of the Objects based on the Bindings</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Kernel.GetAll(serviceType);
        }
    }


    /// <summary>
    /// Its job is to Register Ninject Modules and Resolve Dependencies Manually (If need be)
    /// </summary>
    public class NinjectContainer
    {
        private static NinjectResolver _resolver;

        /// <summary>
        /// Sets up the IOC using the Ninject Modules provided
        /// </summary>
        /// <param name="modules">Modules</param>
        public static void RegisterModules(params NinjectModule[] modules)
        {
            _resolver = new NinjectResolver(modules);
            DependencyResolver.SetResolver(_resolver);
        }

        /// <summary>
        /// Sets up the IOC Container loading modules from the Currently Executing Assembly
        /// </summary>
        public static void RegisterAssembly()
        {
            _resolver = new NinjectResolver(Assembly.GetExecutingAssembly());

            //This is where the actual hookup to the MVC Pipeline is done.
            DependencyResolver.SetResolver(_resolver);
        }

        /// <summary>
        /// Manually Resolve Dependencies 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return _resolver.Kernel.Get<T>();
        }
    }
}