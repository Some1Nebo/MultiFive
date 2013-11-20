using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using MultiFive.Web.Models;

namespace MultiFive.Web
{
    public class IoCContainerConfig
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<ApplicationDbContext>().As<IRepository>().InstancePerHttpRequest();

            /* sample use: 
            builder.RegisterType<AuthorRepository>().As<IAuthorRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectOmegaContext>().AsSelf().InstancePerHttpRequest();
            */

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}