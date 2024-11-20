using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IImageService
    {
        #nullable enable
        //CREATE
        int AddImage(ImageViewModel model);

        //READ
        List<ImageViewModel> GetAllImages();
        ImageViewModel GetImageById(int imageId);
        List<ImageViewModel> GetImagesByRoomId(int roomId);


        //UPDATE
        void UpdateImage(ImageViewModel model);

        //DELETE
        void DeleteImageById(int imageId);
        void DeleteImageByRoomId(int roomId); //either on or more image deletion

        //OTHERS

    }
}
