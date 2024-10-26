using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Services.ServiceModels
{
    public class RoomViewModel
    {

        public RoomViewModel() {}

        public RoomViewModel(Room room)
        {
            this.RoomId = room.RoomId;
            this.Name = room.Name;
            this.Description = room.Description;
            this.Type = room.Type;
            this.Image = room.Image;
            this.Capacity = room.Capacity;
            this.Location = room.Location;
            this.Amenities = room.Amenities;
        }

        public int RoomId { get; set; }

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
