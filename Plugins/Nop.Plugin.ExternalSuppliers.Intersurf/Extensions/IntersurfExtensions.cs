using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.ExternalSuppliers.Intersurf.Extensions
{
    public static class IntersurfExtensions
    {
        public static string Clean(this string input)
        {
            string stripped = input.Replace("\"", "");
           
            return stripped.Trim();
        }
    }
}
