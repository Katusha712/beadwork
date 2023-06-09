﻿using System.Collections.Generic;
using System.Linq;

namespace Beadwork.Memory
{
    public class PictureRepository : IPictureRepository
    {
        private readonly Picture[] pictures = new[]
        {
            new Picture(1,"Едвард Мунк", "IN-61466565","Крик",
                "Крик — назва декількох експресіоністських полотен і відбитків норвезького художника Едварда Мунка, що зображують агонізуючу від жаху постать людини на тлі криваво-червоного неба. Тлом для твору є пейзаж фіорду Осло, що відкривається з пагорбу Екеберг в Кристіанії, Норвегія.",700m),
            new Picture(2, "Вінсент Ван Гог","IN-45781245","Зоряна ніч",
                "Зоряна ніч — картина голландського художника-постімпресіоніста Вінсента ван Гога, написана в червні 1889 року. Він зображує вид із східного вікна його кімнати притулку в Сен-Ремі-де-Прованс , незадовго до сходу сонця, з додаванням уявного села.",640m),
            new Picture(3, "Густав Клімт","IN-14785236","Поцілунок",
                "Поцілунок — картина австрійського художника і графіка Густава Клімта, одного з головних представників віденської сецесії. Створена в 1908–1909 роках. На картині зображена пара. Закохані одягнені в мозаїчну одежу.",1300m),
            new Picture(4, "Ян Вермеєр","IN-79584687","Дівчина з перловою сережкою",
                "Дівчина з перловою сережкою — одна з найвідоміших картин нідерландського художника Яна Вермера.Художник спробував зафіксувати момент, коли дівчина повертає голову у бік глядача до когось, кого вона щойно помітила. Відповідно до назви, увагу глядача сфокусовано на перловій сережці у вусі дівчини.",850m),
            new Picture(5, "Леонардо да Вінчі","IN-45798632", "Пані з горностаєм",
                "Пані з горностаєм — жіночий портрет пензля італійського художника Леонардо да Вінчі, написаний близько 1489—1490 років. На думку багатьох дослідників, зображує Цецилії Галлерані і написаний в період, коли вона була коханкою міланського герцога Людовіко Сфорца, а Леонардо був у герцога на службі.",700m),
        };

        public Picture[] GetAllByIds(IEnumerable<int> pictureIds)
        {
            var foundPictures = from picture in pictures
                                join pictureId in pictureIds on picture.Id equals pictureId
                                select picture;

            return foundPictures.ToArray();
        }

        public Picture[] GetAllByItemNumber(string item_number)
        {
            return pictures.Where(picture => picture.ItemNumber == item_number)
                           .ToArray();
        }
        public Picture[] GetAllByTitleOrAuthor(string query)
        {
            return pictures.Where(picture => picture.Title.Contains(query)
                                          || picture.Author.Contains(query))
                            .ToArray();
        }

        public Picture GetById(int id)
        {
            return pictures.Single(pictures => pictures.Id == id);
        }
    }
}
