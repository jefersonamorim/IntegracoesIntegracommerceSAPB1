using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracommerce.Entity
{
    public class Invoice
    {
        public string IdOrder { get; set; }
        public string OrderStatus { get; set; }
        public string InvoicedNumber { get; set; }
        public int InvoicedLine { get; set; }
        public string InvoicedIssueDate { get; set; }
        public string InvoicedKey { get; set; }
        public string InvoicedDanfeXml { get; set; }
    }

}
