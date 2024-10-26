using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Image
    {
        public int ImageId { get; set; }
        public int? RoomId { get; set; }
        public string Guid { get; set; }

        public virtual Room Room { get; set; }
    }
}
