using System.Collections.Generic;
using System.Linq;

namespace Beadwork.Web.App
{
    public class PictureService
    {
        private readonly IPictureRepository pictureRepository;
        public PictureService(IPictureRepository pictureRepository)
        {
            this.pictureRepository = pictureRepository;
        }
        public IReadOnlyCollection<PictureModel> GetAllByQuery(string query)
        {
            var pictures = Picture.IsItemNumber(query)
                         ? pictureRepository.GetAllByItemNumber(query)
                         : pictureRepository.GetAllByTitleOrAuthor(query);

            return pictures.Select(Map)
                           .ToArray();
        }

        public PictureModel GetById(int id)
        {
            var picture = pictureRepository.GetById(id);

            return Map(picture);
        }

        private PictureModel Map(Picture picture)
        {
            return new PictureModel
            {
                Id = picture.Id,
                ItemNumber = picture.ItemNumber,
                Title = picture.Title,
                Author = picture.Author,
                Description = picture.Description,
                Price = picture.Price,
            };
        }
    }
}
