using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedPizza.Core.Data.Entities
{
    [Serializable]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string OrderReferenceId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public Status Status { get; set; }
        public virtual ICollection<Pizza> Pizza { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public enum Status
    {
        Started,
        ReadyForDelivery,
        Delivering,
        Delivered
    }

    public enum PizzaStatus
    {
        Prep,
        Bake,
        PackagedForDelivery
    }

    public enum Trigger
    {
        UpdateOrder,
        UpdatePizza
    }
}
