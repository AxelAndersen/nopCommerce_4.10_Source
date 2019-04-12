using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Admin.OrderManagementList.Data;
using Nop.Plugin.Admin.OrderManagementList.Services;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Admin.OrderManagementList.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 1;
        private const string CONTEXT_NAME = "nop_object_context_order_management";        

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<OrderManagementService>().As<IOrderManagementService>().InstancePerLifetimeScope();
            builder.RegisterType<FTPService>().As<IFTPService>().InstancePerLifetimeScope();
            builder.RegisterType<GLSService>().As<IGLSService>().InstancePerLifetimeScope();
            builder.RegisterType<ReOrderService>().As<IReOrderService>().InstancePerLifetimeScope();

            //data context
            builder.RegisterPluginDataContext<OrderManagementContext>(CONTEXT_NAME);
        }
    }
}
