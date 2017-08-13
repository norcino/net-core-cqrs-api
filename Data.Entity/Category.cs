﻿using System.Collections.Generic;

namespace Data.Entity
{
    public class Category
    {
        public Category()
        {
            Payments = new HashSet<Payment>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}
