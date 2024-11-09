using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IImageRepository
    {
        IQueryable<Image> GetImages();
        bool ImageExists(int imageId);
        void AddImage(Image image);
        void UpdateImage(Image image);
        void DeleteImage(Image image);
    }
}
