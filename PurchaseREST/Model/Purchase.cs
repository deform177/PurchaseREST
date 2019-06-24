using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseREST.Model
{
    public class Purchase
    {
        public int purchaseid { get; set; }
        [Required]
        [StringLength(20)]
        public string name { get; set; }
        [Required]
        public DateTime date { get; set; }
        [StringLength(100)]
        public string details { get; set; }
        [Required]
        public string price { get; set; }
    }
}
