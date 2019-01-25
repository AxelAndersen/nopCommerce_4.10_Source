using AO.Services.Emails;
using AO.Services.Products;
using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace AO.Services
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 2; }
        }

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<AOProductService>().As<IAOProductService>().InstancePerLifetimeScope();
            //builder.RegisterType<AOOrderService>().As<IAOOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageService>().As<IMessageService>().InstancePerLifetimeScope();
            //builder.RegisterType<OrderMangementContext>().As<IOrderMangementContext>().InstancePerLifetimeScope();            
        }
    }
}
