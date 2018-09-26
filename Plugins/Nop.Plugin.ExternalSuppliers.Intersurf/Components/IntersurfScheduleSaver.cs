using AO.Services.PluginHelper;
using AO.Services.Products;
using AO.Services.Products.Models;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nop.Plugin.ExternalSuppliers.Intersurf.Components
{
    public class IntersurfScheduleSaver : IScheduleTask
    {
        private readonly ILogger _logger;
        private readonly IntersurfSettings _intersurfSettings;        
        private readonly IAOProductService _aoProductService;                

        private List<string> _usedEans = new List<string>();
        private List<VariantData> _variantData;
        private const string _updaterName = "Intersurf";
        private readonly string _destinationPath;

        public IntersurfScheduleSaver(ILogger logger, IntersurfSettings intersurfSettings, IAOProductService aoProductService)
        {
            this._logger = logger;
            this._intersurfSettings = intersurfSettings;
            this._destinationPath = AppDomain.CurrentDomain.BaseDirectory + @"\" + _intersurfSettings.CSVFileName;
            this._aoProductService = aoProductService;                        
        }

        public void Execute()
        {
            try
            {
                // Validates that every setting is set correct in the Configure area.
                Validation.ValidateSettings(_intersurfSettings);

                // Add data to VariantData list
                OrganizeData();

                // Save data and update stock
                _aoProductService.SaveVariantData(_variantData, _updaterName);


                _logger.Information("IntersurfScheduleSaver.Execute() done." + Environment.NewLine + _variantData.Count + " variants in total");
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                _logger.Error("IntersurfScheduleSaver.Execute(): " + inner.Message, ex);
            }
        }

        private void OrganizeData()
        {
            FileInfo fileInfo = new FileInfo(@"" + _destinationPath);
            if (fileInfo == null)
            {
                throw new IOException("No file found here: @" + _destinationPath);
            }

            if (FileHelper.IsFileLocked(fileInfo) == false)
            {
                _variantData = File.ReadAllLines(@"" + _destinationPath)
                   .Skip(1)
                   .Select(t => VariantData.FromCsv(t, ';'))
                   .Where(y => y != null)
                   .ToList();
            }
            else
            {
                _logger.Warning("File was locked, probably because its beeing written to by IntersurfScheduleFetcher. Its ok, we run later again.");
            }
        }
    }
}
