﻿using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class RoomAmenityViewModel
    {

        public RoomAmenityViewModel()
        {
            this.Amenity = new AmenityViewModel();
        }

        public RoomAmenityViewModel(RoomAmenity roomAmenity)
        {
            this.Id = roomAmenity.Id;
            this.RoomId = roomAmenity.RoomId;
            this.AmenityId = roomAmenity.AmenityId;
        }


        public int Id { get; set; }

        [Required(ErrorMessage = "Room ID is required.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Amenity ID is required.")]
        public int AmenityId { get; set; }

        public AmenityViewModel Amenity { get; set; }

        public string AmenityName {

            get
            {
                return this.Amenity.AmenityName; //return Amenity value
            }
                
        }


    }

}

