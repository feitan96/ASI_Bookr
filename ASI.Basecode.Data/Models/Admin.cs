using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Admin
    {
        public int AdminId { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
