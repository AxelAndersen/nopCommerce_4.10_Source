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
        private int sizeCategoryId;
        private int stockCount;
        private string originalCategory;
        private int webshopCategoryId;
        private string orgItemNumber;
        private string brand;
        private string variantSize;
        private string variantColor;

        public string SupplierProductId { get => supplierProductId; set => supplierProductId = value; }
        public string OriginalTitle { get => originalTitle; set => originalTitle = value; }
        public string Title { get => title; set => title = value; }
        public decimal CostPrice { get => costPrice; set => costPrice = value; }
        public decimal RetailPrice { get => retailPrice; set => retailPrice = value; }
        public string EAN { get => eAN; set => eAN = value; }
        public string ColorStr { get => colorStr; set => colorStr = value; }
        public string SizeStr { get => sizeStr; set => sizeStr = value; }
        public int SizeCategoryId { get => sizeCategoryId; set => sizeCategoryId = value; }
        public int StockCount { get => stockCount; set => stockCount = value; }
        public string OriginalCategory { get => originalCategory; set => originalCategory = value; }
        public int WebshopCategoryId { get => webshopCategoryId; set => webshopCategoryId = value; }
        public string OrgItemNumber { get => orgItemNumber; set => orgItemNumber = value; }
        public string Brand { get => brand; set => brand = value; }
        public string VariantSize { get => variantSize; set => variantSize = value; }
        public string VariantColor { get => variantColor; set => variantColor = value; }


        public static VariantData FromCsv(string csvLine)
        {
            string[] TxtSpk = csvLine.Split(',');
            VariantData variantData = new VariantData();
            variantData.SupplierProductId = TxtSpk[0];
            variantData.OriginalTitle = TxtSpk[1];

            return variantData;
        }
    }
}
