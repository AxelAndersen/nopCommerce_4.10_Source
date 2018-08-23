
using Nop.Plugin.ExternalSuppliers.STM.Models;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nop.Plugin.ExternalSuppliers.STM.Components
{
    public class STMSchedule : IScheduleTask
    {
        private readonly ILogger _logger;
        private readonly STMSettings _stmSettings;
        private readonly string _destinationPath;

        public STMSchedule(ILogger logger, STMSettings stmSettings)
        {
            this._logger = logger;
            this._stmSettings = stmSettings;
            this._destinationPath = AppDomain.CurrentDomain.BaseDirectory + @"\" + _stmSettings.CSVFileName;
        }

        public void Execute()
        {

            //return;

            //try
            //{
            //    ValidateSettings();


            //    IntersurfProducts.GetCSVStringResponse csvContent = null;

            //    using (IntersurfProducts.CSV_ServiceSoapClient client = new IntersurfProducts.CSV_ServiceSoapClient(IntersurfProducts.CSV_ServiceSoapClient.EndpointConfiguration.CSV_ServiceSoap12,_stmSettings.EndpointAddress))
            //    {
            //        var serviceHeader = new IntersurfProducts.SecuredWebServiceHeader();
            //        serviceHeader.Username = _stmSettings.Username; // "60750600";
            //        serviceHeader.Password = _stmSettings.Password; // "friliv";

            //        IntersurfProducts.AuthenticateUserResponse response = await client.AuthenticateUserAsync(serviceHeader);
            //        serviceHeader.AuthenticatedToken = response.AuthenticateUserResult;

            //        // Both timeouts are needed (the call takes about 3 minutes)
            //        client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 25, 00); // 25 minutes                    
            //        client.InnerChannel.OperationTimeout = new TimeSpan(0, 25, 00); // 25 minutes

            //        csvContent = await client.GetCSVStringAsync(serviceHeader);
            //    }
                                
            //    System.IO.File.WriteAllText(_destinationPath, csvContent.GetCSVStringResult.Replace(System.Environment.NewLine, ""));

            //}
            //catch (Exception ex)
            //{
            //    _logger.Error("STMSchedule.Execute()", ex);                
            //}
            //throw new NotImplementedException();
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
            if(this._stmSettings == null)
            {
                throw new Exception("No STM settings found, aborting task");
            }

            if (string.IsNullOrEmpty(this._stmSettings.EndpointAddress))
            {
                throw new Exception("No EndpointAddress found in STM settings, aborting task");
            }

            if (string.IsNullOrEmpty(this._stmSettings.Username))
            {
                throw new Exception("No Username found in STM settings, aborting task");
            }

            if (string.IsNullOrEmpty(this._stmSettings.Password))
            {
                throw new Exception("No Password found in STM settings, aborting task");
            }

            if (string.IsNullOrEmpty(this._stmSettings.CSVFileName))
            {
                throw new Exception("No CSVFileName found in STM settings, aborting task");
            }

            if (this._stmSettings.CSVFileName.EndsWith(".csv") == false)
            {
                throw new Exception("CSVFileName must end with '.csv', aborting task");
            }

        }
    }
}
