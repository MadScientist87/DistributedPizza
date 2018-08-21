using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedPizza.Core.Data.Entities
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string OrderReferenceId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public virtual ICollection<Pizza> Pizza { get; set; }
    }
}
