using Nop.Core.Domain.Catalog;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Services
{
    public interface IManufacturerApiService
    {
        Manufacturer GetManufacturerById(int manufacturerId);

        IList<Manufacturer> GetManufacturers();

        int GetManufacturersCount();
    }
}
