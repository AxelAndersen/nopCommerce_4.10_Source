using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Admin.OrderManagementList.Domain;

namespace Nop.Plugin.Admin.OrderManagementList.Data
{
    public class AOOrderMap : NopEntityTypeConfiguration<AOOrder>
    {
        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<AOOrder> builder)
        {
            builder.ToTable("Order");
            builder.HasKey(o => o.Id);
        }
    }
}
