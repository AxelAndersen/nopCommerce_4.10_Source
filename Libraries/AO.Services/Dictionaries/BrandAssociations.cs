using System;
using System.Collections.Generic;
using System.Text;

namespace AO.Services.Dictionaries
{
    public class BrandAssociations
    {
        internal static Dictionary<string, string> Brands = new Dictionary<string, string>()
        {              
            {"Isbjorn of Sweden", "Isbjörn of Sweden"},
            {"Isbjorn of sweden", "Isbjörn of Sweden"},
            {"Isbjørn of Sweden", "Isbjorn of Sweden"},
            {"Isbjoern of Sweden", "Isbjorn of Sweden"},
            {"Isbjørn of sweden", "Isbjorn of Sweden"},
            {"Isbjoern of sweden", "Isbjorn of Sweden"},
            {"MUURIKKA", "Muurikka"},
        };
    }
}
