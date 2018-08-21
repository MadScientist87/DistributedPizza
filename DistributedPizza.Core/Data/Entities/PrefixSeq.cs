using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedPizza.Core.Data.Entities
{
    [Table("PrefixSeq")]
    public class PrefixSeq
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public IdType IdType { get; set; }

        [Required, StringLength(32)]
        public string Prefix { get; set; }

        public int Seq { get; set; }

        public int AlphaPrefix { get; set; }

        [Timestamp]
        public Byte[] Timestamp { get; set; }
    }

    public enum IdType
    {
        PizzaOrder
    }
}
