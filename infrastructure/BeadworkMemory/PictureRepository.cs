using System;
using System.Linq;

namespace Beadwork.Memory
{
    public class PictureRepository : IPictureRepository
    {
        private readonly Picture[] pictures = new[]
        {
            new Picture(1,"Едвард Мунк", "IN-61466565","Крик"),
            new Picture(2, "Вінсент Ван Гог","IN-45781245","Зоряна ніч"),
            new Picture(3, "Густав Клімт","IN-14785236","Поцілунок"),
            new Picture(4, "Ян Вермеєр","IN-79584687","Дівчина з перловою сережкою"),
            new Picture(5, "Леонардо да Вінчі","IN-45798632", "Пані з горностаєм"),
        };
        public Picture[] GetAllByItemNumber(string item_number)
        {
            return pictures.Where(picture => picture.Item_number == item_number)
                           .ToArray();
        }
        public Picture[] GetAllByTitleOrAuthor(string query)
        {
            return pictures.Where(picture => picture.Title.Contains(query)
                                          || picture.Author.Contains(query))
                            .ToArray();
        }
    }
}
