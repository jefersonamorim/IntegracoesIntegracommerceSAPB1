using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracommerce.Entity
{

    public class OrderFiltered
    {
        internal OrderFiltered() {
        }
        public List[] list { get; set; }
        public object[] facets { get; set; }
        public Paging paging { get; set; }
        public Stats stats { get; set; }
    }

    public class Paging
    {
        public int total { get; set; }
        public int pages { get; set; }
        public int currentPage { get; set; }
        public int perPage { get; set; }
    }

    public class Stats
    {
        public Stats1 stats { get; set; }
    }

    public class Stats1
    {
        public Totalvalue totalValue { get; set; }
        public Totalitems totalItems { get; set; }
    }

    public class Totalvalue
    {
        public int Count { get; set; }
        public float Max { get; set; }
        public float Mean { get; set; }
        public float Min { get; set; }
        public int Missing { get; set; }
        public float StdDev { get; set; }
        public float Sum { get; set; }
        public float SumOfSquares { get; set; }
        public Facets Facets { get; set; }
    }

    public class Facets
    {
    }

    public class Totalitems
    {
        public int Count { get; set; }
        public float Max { get; set; }
        public float Mean { get; set; }
        public float Min { get; set; }
        public int Missing { get; set; }
        public float StdDev { get; set; }
        public float Sum { get; set; }
        public float SumOfSquares { get; set; }
        public Facets1 Facets { get; set; }
    }

    public class Facets1
    {
    }

    public class List
    {
        public string orderId { get; set; }
        public DateTime creationDate { get; set; }
        public string clientName { get; set; }
        public object items { get; set; }
        public float totalValue { get; set; }
        public string paymentNames { get; set; }
        public string status { get; set; }
        public string statusDescription { get; set; }
        public object marketPlaceOrderId { get; set; }
        public string sequence { get; set; }
        public string salesChannel { get; set; }
        public string affiliateId { get; set; }
        public string origin { get; set; }
        public bool workflowInErrorState { get; set; }
        public bool workflowInRetry { get; set; }
        public string lastMessageUnread { get; set; }
        public object ShippingEstimatedDate { get; set; }
        public object ShippingEstimatedDateMax { get; set; }
        public object ShippingEstimatedDateMin { get; set; }
        public bool orderIsComplete { get; set; }
        public object listId { get; set; }
        public object listType { get; set; }
        public object authorizedDate { get; set; }
        public object callCenterOperatorName { get; set; }
        public int totalItems { get; set; }
        public string currencyCode { get; set; }
        public string hostname { get; set; }
        public object invoiceOutput { get; set; }
        public object invoiceInput { get; set; }
    }

}
