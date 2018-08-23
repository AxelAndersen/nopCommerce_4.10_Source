using Nop.Plugin.ExternalSuppliers.Intersurf.Models;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nop.Plugin.ExternalSuppliers.Intersurf.Components
{
    public class IntersurfSchedule : IScheduleTask
    {
        private readonly ILogger _logger;
        private readonly IntersurfSettings _intersurfSettings;
        private readonly string _destinationPath;

        public IntersurfSchedule(ILogger logger, IntersurfSettings intersurfSettings)
        {
            this._logger = logger;
            this._intersurfSettings = intersurfSettings;
            this._destinationPath = AppDomain.CurrentDomain.BaseDirectory + @"\" + _intersurfSettings.CSVFileName;
        }

        public async void Execute()
        {
            try
            {
                ValidateSettings();

                IntersurfSR.GetCSVStringResponse csvContent = null;

                using (IntersurfSR.CSV_ServiceSoapClient client = new IntersurfSR.CSV_ServiceSoapClient(IntersurfSR.CSV_ServiceSoapClient.EndpointConfiguration.CSV_ServiceSoap12,_intersurfSettings.EndpointAddress))
                {
                    var serviceHeader = new IntersurfSR.SecuredWebServiceHeader();
                    serviceHeader.Username = _intersurfSettings.Username; // "60750600";
                    serviceHeader.Password = _intersurfSettings.Password; // "friliv";

                    IntersurfSR.AuthenticateUserResponse response = await client.AuthenticateUserAsync(serviceHeader);
                    serviceHeader.AuthenticatedToken = response.AuthenticateUserResult;

                    // Both timeouts are needed (the call takes about 3 minutes)
                    client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 25, 00); // 25 minutes                    
                    client.InnerChannel.OperationTimeout = new TimeSpan(0, 25, 00); // 25 minutes

                    csvContent = await client.GetCSVStringAsync(serviceHeader);
                }
                                
                System.IO.File.WriteAllText(_destinationPath, csvContent.GetCSVStringResult.Replace(System.Environment.NewLine, ""));

            }
            catch (Exception ex)
            {
                _logger.Error("IntersurfSchedule.Execute()", ex);                
            }            
        }

        private void ReadDataFromFile()
        {

            List<VariantData> VarData = File.ReadAllLines(@"" + _destinationPath)
               .Skip(1)
               .Select(t => VariantData.FromCsv(t))
               .ToList();

            var test = VarData;
            //Console.WriteLine("Reading data from csv file to List in memory");
            //using (TextFieldParser parser = new TextFieldParser(_filePath))
            //{
            //    parser.TextFieldType = FieldType.Delimited;
            //    parser.SetDelimiters(";");
            //    parser.HasFieldsEnclosedInQuotes = false;
            //    VariantData data;
            //    _variantData = new List<VariantData>();
            //    bool firstRow = true;

            //    string missing = "";
            //    while (!parser.EndOfData)
            //    {
            //        string[] row = parser.ReadFields();
            //        if (firstRow)
            //        {
            //            firstRow = false;
            //            continue;
            //        }
            //        try
            //        {
            //            if (row.Length != 9 || row[0].ToString().Length == 0)
            //                continue;

            //            data = new VariantData();
            //            data.SupplierProductId = row[0].ToString().Replace("\"", "");
            //            data.OriginalTitle = data.Title = row[1].ToString().Replace("\"", "");
            //            data.ColorStr = row[2].ToString().Replace("\"", "").Trim();
            //            data.SizeStr = row[3].ToString().Replace("\"", "").Trim();
            //            data.StockCount = Convert.ToInt32(row[4].ToString().Replace("\"", ""));

            //            data.EAN = row[5].ToString().Replace("\"", "").Replace(" ", "").Trim();
            //            if (string.IsNullOrEmpty(data.EAN) || data.EAN.Length < 11)
            //                continue;

            //            data.CostPrice = decimal.Parse(row[6].ToString().Replace("\"", ""));
            //            data.RetailPrice = decimal.Parse(row[7].ToString().Replace("\"", ""));
            //            data.OriginalCategory = row[8].ToString().Replace("\"", "");

            //            if (Categories.Associations.ContainsKey(data.OriginalCategory))
            //            {
            //                data.WebshopCategoryId = Categories.Associations[data.OriginalCategory];
            //            }
            //            else if (missing.IndexOf(data.OriginalCategory + "';") == -1)
            //            {
            //                missing += "Missing category: '" + data.OriginalCategory + "'; ";
            //            }

            //            // Should be changed if we get more information from Intersurf
            //            data.SizeCategoryId = (int)Enumerations.SizeCategories.Mics;

            //            _variantData.Add(data);
            //        }
            //        catch (Exception ex)
            //        {
            //            HandleError(ex);
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(missing))
            //    {
            //        HandleError(new Exception("Missing category association in InterSurfProducts.cs: " + missing));
            //    }
            //}
        }

        private void ValidateSettings()
        {
            if(this._intersurfSettings == null)
            {
                throw new Exception("No Intersurf settings found, aborting task");
            }

            if (string.IsNullOrEmpty(this._intersurfSettings.EndpointAddress))
            {
                throw new Exception("No EndpointAddress found in Intersurf settings, aborting task");
            }

            if (string.IsNullOrEmpty(this._intersurfSettings.Username))
            {
                throw new Exception("No Username found in Intersurf settings, aborting task");
            }

            if (string.IsNullOrEmpty(this._intersurfSettings.Password))
            {
                throw new Exception("No Password found in Intersurf settings, aborting task");
            }

            if (string.IsNullOrEmpty(this._intersurfSettings.CSVFileName))
            {
                throw new Exception("No CSVFileName found in Intersurf settings, aborting task");
            }

            if (this._intersurfSettings.CSVFileName.EndsWith(".csv") == false)
            {
                throw new Exception("CSVFileName must end with '.csv', aborting task");
            }

        }
    }
}
