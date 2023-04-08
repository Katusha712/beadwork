using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beadwork
{
    public interface IPictureRepository
    {
        Picture[] GetAllByItemNumber(string item_number);
        Picture[] GetAllByTitleOrAuthor(string titleOrAuthorPart);
        Picture GetById(int id);
        Picture[] GetAllByIds(IEnumerable<int> pictureIds);
    }
}
