using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracommerce.Entity
{
    public class OrderIntegraCommerce
    {
        public string OrderStatus { get; set; }
        public string IdOrder { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string TotalAmount { get; set; }
        public string TotalFreight { get; set; }
        public string TotalDiscount { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string ReceiverName { get; set; }
        public string CustomerMail { get; set; }
        public string TotalTax { get; set; }
        public string IdOrderMarketplace { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime PurchasedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string MarketplaceName { get; set; }
        public string StoreName { get; set; }
       // public bool UpdatedMarketplaceStatus { get; set; }
        //public bool InsertedErp { get; set; }
        public string CustomerPfCpf { get; set; }
        public string CustomerPfName { get; set; }
        public string CustomerPjCnpj { get; set; }
        public string CustomerPjCorporatename { get; set; }
        public string DeliveryAddressStreet { get; set; }
        public string DeliveryAddressAdditionalInfo { get; set; }
        public string DeliveryAddressZipcode { get; set; }
        public string DeliveryAddressNeighborhood { get; set; }
        public string DeliveryAddressCity { get; set; }
        public string DeliveryAddressReference { get; set; }
        public string DeliveryAddressState { get; set; }
        public string DeliveryAddressNumber { get; set; }
        public string TelephoneMainNumber { get; set; }
        public string TelephoneSecundaryNumber { get; set; }
        public string TelephoneBusinessNumber { get; set; }
        public string CustomerBirthDate { get; set; }
        public string CustomerPjIe { get; set; }
        public string InvoicedNumber { get; set; }
        ///public int InvoicedLine { get; set; }
        //public DateTime InvoicedIssueDate { get; set; }
        public string InvoicedKey { get; set; }
        public string InvoicedDanfeXml { get; set; }
        public string ShippedTrackingUrl { get; set; }
        public string ShippedTrackingProtocol { get; set; }
        public string ShippedEstimatedDelivery { get; set; }
        public string ShippedCarrierDate { get; set; }
        public string ShippedCarrierName { get; set; }
        public string ShipmentExceptionObservation { get; set; }
        //public DateTime ShipmentExceptionOccurrenceDate { get; set; }
        public string DeliveredDate { get; set; }
        public string ShippedCodeERP { get; set; }
        public Product[] Products { get; set; }
        //public Payment[] Payments { get; set; }
    }

    public class Product
    {
        public string IdSku { get; set; }
        public int Quantity { get; set; }
        public string Price { get; set; }
        public string Freight { get; set; }
        public string Discount { get; set; }
        public int IdOrderPackage { get; set; }
    }

    /*public class Payment
    {
        public string Name { get; set; }
        public int Installments { get; set; }
        public int Amount { get; set; }
    }*/

}
