using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class AmenityViewModel
    {

        public AmenityViewModel() { }

        public AmenityViewModel(Amenity amenity)
        {
            this.AmenityId = amenity.AmenityId;
            this.AmenityName = amenity.AmenityName;
        }

        [Required(ErrorMessage = "Amenity ID is required.")]
        public int AmenityId { get; set; }

        [StringLength(250, ErrorMessage = "Amenity value cannot exceed 250 characters.")]
        public string AmenityName { get; set; }
    }
}
