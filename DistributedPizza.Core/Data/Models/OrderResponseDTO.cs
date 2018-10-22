using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedPizza.Core.Data.Models
{
    public class OrderResponseDTO
    {
        public string OrderReferenceId { get; set; }
        public int NumberOfRequests { get; set; }
    }
}
