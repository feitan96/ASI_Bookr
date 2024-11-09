using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Services.ServiceModels
{
    public class ImageViewModel
    {
        public ImageViewModel() { }

        public ImageViewModel(int roomId, string guid)
        {
            this.RoomId = roomId;
            this.Guid = guid;
        }

        public ImageViewModel(Image image)
        {
            this.ImageId = image.ImageId;
            this.RoomId = image.RoomId;
            this.Guid = image.Guid;
        }

        public int ImageId { get; set; } //Primary Key

        [Range(0, int.MaxValue, ErrorMessage = "RoomId value must be given")]
        public int? RoomId { get; set; }

        [Required(ErrorMessage = "Guid string for the image is required.")]
        public string Guid { get; set; }

    }
}
