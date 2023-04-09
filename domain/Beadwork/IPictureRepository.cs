using System.Collections.Generic;

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
