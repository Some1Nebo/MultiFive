using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using MultiFive.Web.DataAccess;
using MultiFive.Web.Infrastructure;
using MultiFive.Web.Models.Messaging;

namespace MultiFive.Web
{
    public class IoCContainerConfig
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<ApplicationDbContext>().AsSelf().InstancePerHttpRequest();             
            builder.RegisterType<EFRepository>().As<IRepository>().InstancePerLifetimeScope();
            builder.RegisterType<MessageFactory>().As<IMessageFactory>().InstancePerLifetimeScope(); 
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance(); 

            builder.Register(c => HttpContext.Current.User).InstancePerHttpRequest(); 

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}