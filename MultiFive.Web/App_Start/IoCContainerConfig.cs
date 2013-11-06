using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace MultiFive.Web
{
    public class IoCContainerConfig
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            /* How to use? 
                1. Register controllers. Line above registers all the controllers in the assembly
                2. Register DbContext. Example: builder.RegisterType<EFDbContext>().AsSelf().InstancePerHttpRequest();
                   Good practice to create one connection per Http request
                3. Register service per dependency. Example: builder.RegisterType<EmailOrderProcessor>().As<IOrderProcessor>().InstancePerDependency();
                4. Register repository. Usually we create 1 repository per lifetime of the controller.
                   Example: builder.RegisterType<EFProductRepository>().As<IProductRepository>().InstancePerLifetimeScope();
            */

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}