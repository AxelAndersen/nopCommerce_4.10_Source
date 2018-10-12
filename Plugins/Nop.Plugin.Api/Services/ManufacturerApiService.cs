using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DataStructures;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Api.Services
{
    public class ManufacturerApiService : IManufacturerApiService
    {
        private readonly IStoreMappingService _storeMappingService;
        private readonly IRepository<Manufacturer> _manufacturerRepository;        

        public ManufacturerApiService(IRepository<Manufacturer> manufacturerRepository, IStoreMappingService storeMappingService)
        {
            _manufacturerRepository = manufacturerRepository;            
            _storeMappingService = storeMappingService;
        }

        public Manufacturer GetManufacturerById(int manufacturerId)
        {
            throw new NotImplementedException();
        }

        public IList<Manufacturer> GetManufacturers()
        {
            var query = from manufacurers in _manufacturerRepository.Table select manufacurers;
            return new ApiList<Manufacturer>(query, 0, 10000);
        }

        public int GetManufacturersCount()
        {
            throw new NotImplementedException();
        }
    }
}
