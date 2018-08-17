using System;
using System.Text.RegularExpressions;

namespace Nop.Plugin.POS.Kaching.Extensions
{
    public static class KachingExtensions
    {
        public static string StripHTML(this string input)
        {
            string stripped = Regex.Replace(input, "<.*?>", String.Empty);
            stripped = stripped.Replace("[b]", "").Replace("[/b]", "").Replace("[br]", ",").Replace("[br/]", ",");

            return stripped.Trim();
        }
    }
}
