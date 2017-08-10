﻿using System;
using System.Collections.Generic;

namespace Data.Entity
{
    public partial class Transaction
    {
        public int Id { get; set; }
        public DateTime Recorded { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}
