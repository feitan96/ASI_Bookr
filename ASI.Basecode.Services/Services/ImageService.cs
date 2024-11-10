using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;
using System.IO;

namespace ASI.Basecode.Services.Services
{
    public class ImageService : IImageService
    {

        private readonly IImageRepository _imagerepository;
        private readonly IMapper _mapper;


        public ImageService(IImageRepository imageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _imagerepository = imageRepository;
        }

        #region Create (CRUD)

        // Adds a new image if it doesn't already exist, otherwise throws an exception.
        public int AddImage(ImageViewModel model)
        {
            var image = new Image();
            if (!_imagerepository.ImageExists(model.ImageId))
            {
                _mapper.Map(model, image);
                _imagerepository.AddImage(image);
                return image.ImageId;
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.ImageExists);
            }
        }
        #endregion

        #region Read (CRUD)

        public List<ImageViewModel> GetAllImages()
        {
            var images = _imagerepository.GetImages().Select(image => new ImageViewModel(image)).ToList();
            return images;
        }

        #endregion

        public ImageViewModel GetImageById(int imageId)
        {
            var image = _imagerepository.GetImages().Where(image => image.ImageId == imageId).FirstOrDefault();
            var imageModel = new ImageViewModel(image);
            return imageModel;
        }

        public List<ImageViewModel> GetImagesByRoomId(int roomId)
        {
            var images = _imagerepository.GetImages().Where(image => image.RoomId == roomId).Select(image => new ImageViewModel(image)).ToList();
            return images;
        }

        #region UPDATE (CRUD)

        public void UpdateImage(ImageViewModel model)
        {
            var image = new Image();
            try
            {
                _mapper.Map(model, image);
                _imagerepository.UpdateImage(image);
            }
            catch (Exception)
            {
                throw; //rethrow error
            }
        }

        #endregion

        #region DELETE (CRUD)

        public void DeleteImageById(int imageId)
        {
            var image = _imagerepository.GetImages().Where(image => image.ImageId == imageId).FirstOrDefault();
            if(image != null)
            {
                _imagerepository.DeleteImage(image);
            }
        }
        public void DeleteImageByRoomId(int roomId) //either on or more image deletion
        {
            var images = _imagerepository.GetImages().Where(image => image.RoomId == roomId).ToList();
            foreach (var image in images) {
                if(image != null)
                {
                    _imagerepository.DeleteImage(image);
                }
            }
        }

        #endregion
    }
}
