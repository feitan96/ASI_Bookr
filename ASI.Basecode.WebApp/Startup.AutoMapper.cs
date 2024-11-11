using AutoMapper;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ASI.Basecode.WebApp
{
    // AutoMapper configuration
    internal partial class StartupConfigurer
    {
        /// <summary>
        /// Configure auto mapper
        /// </summary>
        private void ConfigureAutoMapper()
        {
            var mapperConfiguration = new MapperConfiguration(config =>
            {
                config.AddProfile(new AutoMapperProfileConfiguration());
            });

            this._services.AddSingleton<IMapper>(sp => mapperConfiguration.CreateMapper());
        }

        private class AutoMapperProfileConfiguration : Profile
        {
            public AutoMapperProfileConfiguration()
            {
                CreateMap<UserViewModel, User>();

                // Add mapping between RoomViewModel and Room
                CreateMap<RoomViewModel, Room>()
                    .ForMember(dest => dest.RoomAmenities, opt => opt.Ignore()) // Ignore RoomAmenities if you don't want to map it
                    .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.RoomId)) // Ignore RoomId if it's auto-generated
                    .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()) // Ignore CreatedDate (set server-side)
                    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Ignore CreatedBy (set server-side)
                    .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.Now)) // Set UpdatedDate on update
                    .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy)) // Assuming you get userId in RoomViewModel
                    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

                // Optionally, map RoomAmenities collection if needed
                CreateMap<RoomAmenityViewModel, RoomAmenity>();
                CreateMap<AmenityViewModel, Amenity>();
                CreateMap<ImageViewModel, Image>();
            }
        }
    }
}
