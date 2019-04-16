using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Shipping.GLS.Data;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Shipping.GLS.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 1;
        private const string CONTEXT_NAME = "nop_object_context_gsl_countries";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //data context
            builder.RegisterPluginDataContext<GLSContext>(CONTEXT_NAME);
        }
    }
}
