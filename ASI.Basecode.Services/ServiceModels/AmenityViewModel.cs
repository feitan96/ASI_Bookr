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

        public int AmenityId { get; set; }

        [Required(ErrorMessage = "Amenity Name is required.")]
        [StringLength(250, ErrorMessage = "Amenity value cannot exceed 250 characters.")]
        public string AmenityName { get; set; }
    }

    public class PagedResultAmenity<T>
    {
        public List<T> Items { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }
}
