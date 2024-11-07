﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;
using Microsoft.AspNetCore.Http;

namespace ASI.Basecode.Services.ServiceModels
{
    //[RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
    public class RoomViewModel
    {

        public RoomViewModel() {
            //Added code for post request edit submission
            this.RoomAmenities = new List<RoomAmenityViewModel>();
        }

        public RoomViewModel(Room room)
        {
            this.RoomId = room.RoomId;
            this.Name = room.Name;
            this.Description = room.Description;
            this.Type = room.Type;
            this.ImagePath = room.Image;
            this.Capacity = room.Capacity;
            this.Location = room.Location;
        }

        private List<int> _roomAmenitiesId;

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
        public string ImagePath { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number.")]
        public int? Capacity { get; set; }

        [StringLength(250, ErrorMessage = "Location cannot exceed 250 characters.")]
        public string Location { get; set; }

        [StringLength(250, ErrorMessage = "Amenities cannot exceed 250 characters.")]
        public string Amenities { get; set; }

        public int UpdatedBy { get; set; }

        public IFormFile ImageFile { get; set; }

        public List<RoomAmenityViewModel> RoomAmenities { get; set; }

        public List<string> RoomAmenitiesName
        {
            get
            {
                return RoomAmenities.Select(x => x.AmenityName).ToList() ?? new List<string>();
            }

        }

        public List<int> RoomAmenitiesId
        {
            get
            {
                return this.RoomAmenities.Select(x => x.Amenity.AmenityId).ToList();
            }
            set
            {
                this._roomAmenitiesId = value;
            }

        }
    }
}
