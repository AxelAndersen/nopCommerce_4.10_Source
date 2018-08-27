using Nop.Plugin.ExternalSuppliers.Intersurf.Extensions;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.ExternalSuppliers.Intersurf.Models
{
    public class VariantData
    {
        private string supplierProductId;
        private string originalTitle;
        private string title;
        private decimal costPrice;
        private decimal retailPrice;
        private string eAN;
        private string colorStr;
        private string sizeStr;        
        private int stockCount;
        private string originalCategory;
        private int webshopCategoryId;
        private string orgItemNumber;
        private string brand;

        public string SupplierProductId { get => supplierProductId; set => supplierProductId = value; }
        public string OriginalTitle { get => originalTitle; set => originalTitle = value; }
        public string Title { get => title; set => title = value; }
        public decimal CostPrice { get => costPrice; set => costPrice = value; }
        public decimal RetailPrice { get => retailPrice; set => retailPrice = value; }
        public string EAN { get => eAN; set => eAN = value; }
        public string ColorStr { get => colorStr; set => colorStr = value; }
        public string SizeStr { get => sizeStr; set => sizeStr = value; }        
        public int StockCount { get => stockCount; set => stockCount = value; }
        public string OriginalCategory { get => originalCategory; set => originalCategory = value; }
        public int WebshopCategoryId { get => webshopCategoryId; set => webshopCategoryId = value; }
        public string OrgItemNumber { get => orgItemNumber; set => orgItemNumber = value; }
        public string Brand { get => brand; set => brand = value; }


        public static VariantData FromCsv(string csvLine, ILogger logger)
        {
            if(string.IsNullOrEmpty(csvLine))
            {
                return null;
            }

            string[] data = csvLine.Split(';');
            if (data.Length < 9)
            {
                return null;
            }

            VariantData variantData = new VariantData();
            try
            {                
                variantData.SupplierProductId = data[0].Clean();
                variantData.OriginalTitle = data[1].Clean();
                variantData.ColorStr = data[2].Clean();
                variantData.SizeStr = data[3].Clean();
                variantData.EAN = data[5].Clean().Replace(" ", "");

                int stockCount = 0;
                int.TryParse((data[4].Clean()), out stockCount);
                variantData.StockCount = stockCount;
                
                decimal costPrice = 0;
                decimal.TryParse(data[6].Clean(), out costPrice);
                variantData.CostPrice = costPrice;

                decimal retailPrice = 0;
                decimal.TryParse(data[7].Clean(), out retailPrice);
                variantData.RetailPrice = retailPrice;

                variantData.OriginalCategory = data[8].Clean();                
            }
            catch(Exception ex)
            {
                logger.Error("Error in FromCsv(): " + ex.Message + Environment.NewLine + "csvLine: " + csvLine, ex);
            }
            return variantData;
        }
    }
}
