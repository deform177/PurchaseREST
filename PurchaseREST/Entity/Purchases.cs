using System;
using System.Collections.Generic;

namespace PurchaseREST
{
    public partial class Purchases
    {
        public int PurchaseId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Details { get; set; }
        public decimal Price { get; set; }
        public int? UserId { get; set; }

        public virtual Users User { get; set; }
    }
}
