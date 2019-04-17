using System.Collections.Generic;
using Nop.Plugin.Shipping.GLS.Models;

namespace Nop.Plugin.Shipping.GLS.Services
{
    public interface IGLSService
    {
        List<AOGLSCountry> GetAllCountries();
        void UpdateCountries(List<AOGLSCountry> glsCountries);
        AOGLSCountry GetCountryByThreeLetterCode(string threeLetterISOCode);
    }
}