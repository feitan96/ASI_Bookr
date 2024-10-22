using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class RoomViewModel
    {
        //RoomId not included, since its an auto-incremented primary key

        [Required(ErrorMessage = "Room name is required.")]
        [StringLength(250, ErrorMessage = "Room name cannot exceed 250 characters.")]
        public string Name { get; set; }

        [StringLength(int.MaxValue, ErrorMessage = "Description is too long.")]
        public string Description { get; set; }

        [StringLength(250, ErrorMessage = "Type cannot exceed 250 characters.")]
        public string Type { get; set; }

        [StringLength(250, ErrorMessage = "Image path cannot exceed 250 characters.")]
        [Url(ErrorMessage = "Please enter a valid URL for the image.")]
        public string Image { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number.")]
        public int? Capacity { get; set; }

        [StringLength(250, ErrorMessage = "Location cannot exceed 250 characters.")]
        public string Location { get; set; }

        [StringLength(250, ErrorMessage = "Amenities cannot exceed 250 characters.")]
        public string Amenities { get; set; }
    }
}
