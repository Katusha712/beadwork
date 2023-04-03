using System;
using System.Linq;

namespace Beadwork.Memory
{
    public class PictureRepository : IPictureRepository
    {
        private readonly Picture[] pictures = new[]
        {
            new Picture(1, "Крик"),
            new Picture(2, "Зоряна ніч"),
            new Picture(3, "Поцілунок"),
            new Picture(4, "Дівчина з перловою сережкою"),
            new Picture(5, "Пані з горностаєм"),
        };

        public Picture[] GetAllByTitle(string titlePart)
        {
            return pictures.Where(picture => picture.Title.Contains(titlePart))
                            .ToArray();
        }
    }
}
