using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nop.Plugin.Admin.OrderManagementList.Domain
{
    public class AOProductAttributeValue : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int ProductAttributeMappingId { get; set; }
        public string Name { get; set; }
    }
}
