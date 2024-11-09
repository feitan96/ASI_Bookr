using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class ImageRepository : BaseRepository, IImageRepository
    {
        public ImageRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IQueryable<Image> GetImages()
        {
            var images = this.GetDbSet<Image>();
            return images;
        }
        public bool ImageExists(int imageId)
        {
            return this.GetDbSet<Image>().Any(x => x.ImageId == imageId);
        }
        public void AddImage(Image image)
        {
            this.GetDbSet<Image>().Add(image);
            UnitOfWork.SaveChanges();
        }

        public void UpdateImage(Image image)
        {
            this.GetDbSet<Image>().Update(image);
            UnitOfWork.SaveChanges();
        }

        public void DeleteImage(Image image)
        {
            this.GetDbSet<Image>().Remove(image);
            UnitOfWork.SaveChanges();
        }
    }
}
