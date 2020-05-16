using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracommerce.Entity
{
    public class OrderQueue
    {
        public int Total { get; set; }
        public List<OrderQueues> OrderQueues { get; set; }
    }

    public class OrderQueues
    {
        public int Id { get; set; }
        public string IdOrder { get; set; }
        public string IdOrderMarketplace { get; set; }
        public DateTime InsertedDate { get; set; }
        public string OrderStatus { get; set; }
    }

}
