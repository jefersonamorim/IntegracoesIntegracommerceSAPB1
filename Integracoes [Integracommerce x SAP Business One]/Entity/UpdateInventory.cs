using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracommerce.Entity
{
    public class UpdateInventory
    {
        public List<Skus> SkuList { get; set; }
    }

    public class Skus
    {
        public string IdSku { get; set; }
        public int Quantity { get; set; }
    }

}
