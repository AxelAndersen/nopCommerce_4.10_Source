using System;
using System.Collections.Generic;
using System.Text;

namespace AO.Services.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceSpecialCharacters(this string str)
        {
            str = str.Replace('æ', 'e')
                 .Replace('ø', 'o')
                 .Replace('å', 'a')
                 .Replace('Æ', 'E')
                 .Replace('Ø', 'O')
                 .Replace('Å', 'A')
                 .Replace('ö', 'o')
                 .Replace('Ö', 'O')
                 .Replace('é', 'e')
                 .Replace('É', 'E')
                 .Replace('ä', 'a')
                 .Replace('Ä', 'A');            

                str = str.Normalize(NormalizationForm.FormKD);

            return str;
        }
    }
}
