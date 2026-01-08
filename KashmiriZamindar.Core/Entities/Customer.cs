using System;

namespace KashmiriZamindar.Core.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }  // PK
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool SubscribeMarketing { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
