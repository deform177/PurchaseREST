using System;
using System.Collections.Generic;

namespace PurchaseREST
{
    public partial class Users
    {
        public Users()
        {
            Purchases = new HashSet<Purchases>();
        }

        public int UserId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Purchases> Purchases { get; set; }
    }
}
