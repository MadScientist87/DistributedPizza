using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedPizza.Core.Data.Entities
{
    public class Pizza
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public SauceType SauceType { get; set; }
        public Size Size { get; set; }
        public virtual ICollection<Toppings> Toppings { get; set; }
    }

    public enum SauceType
    {
        None,
        Tomato,
        White
    }

    public enum Size
    {
        Small,
        Medium,
        Large
    }
}
