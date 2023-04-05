using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beadwork
{
    public class PictureService
    {
        private readonly IPictureRepository pictureRepository;
        public PictureService(IPictureRepository pictureRepository)
        {
            this.pictureRepository = pictureRepository;
        }
        public Picture[] GetAllByQuery(string query)
        {
            if (Picture.IsItemNumber(query))
                return pictureRepository.GetAllByItemNumber(query);

            return pictureRepository.GetAllByTitleOrAuthor(query);
        }
    }
}
