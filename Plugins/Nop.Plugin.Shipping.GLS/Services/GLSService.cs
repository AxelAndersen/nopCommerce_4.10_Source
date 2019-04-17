using Nop.Plugin.Shipping.GLS.Data;
using Nop.Plugin.Shipping.GLS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Shipping.GLS.Services
{
    public class GLSService : IGLSService
    {
        private readonly GLSContext _context;

        public GLSService(GLSContext context)
        {
            this._context = context;
        }

        public List<AOGLSCountry> GetAllCountries()
        {
            List<AOGLSCountry> countries = _context.AOGLSCountries.OrderBy(o => o.SortOrder).ToList();
            return countries;
        }

        public void UpdateCountries(List<AOGLSCountry> glsCountries)
        {
            foreach (AOGLSCountry country in glsCountries)
            {
                _context.AOGLSCountries.Update(country);
            }
            _context.SaveChanges();
        }

        public AOGLSCountry GetCountryByThreeLetterCode(string threeLetterISOCode)
        {
            var country = _context.AOGLSCountries.Where(c => c.ThreeLetterISOCode == threeLetterISOCode).FirstOrDefault();
            if(country == null)
            {
                country = _context.AOGLSCountries.Where(c => c.ThreeLetterISOCode == "XXX").FirstOrDefault();
            }
            return country;
        }
    }
}
