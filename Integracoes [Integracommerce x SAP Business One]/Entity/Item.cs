using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracommerce.Entity
{
    public class Item
    {
        public string IdSku { get; set; }
        public string IdSkuErp { get; set; }
        public string IdProduct { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }

        /*public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public int Weight { get; set; }
        public string CodeEan { get; set; }
        public string CodeNcm { get; set; }
        public string CodeIsbn { get; set; }
        public string CodeNbm { get; set; }
        public string Variation { get; set; }
        public Price Price { get; set; }
        public int StockQuantity { get; set; }
        public string MainImageUrl { get; set; }
        public string[] UrlImages { get; set; }
        public Attribute[] Attributes { get; set; }
        */
    }

    public class Price
    {
        public int ListPrice { get; set; }
        public int SalePrice { get; set; }
    }

    public class Attribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

}
