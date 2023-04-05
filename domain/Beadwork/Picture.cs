using System;
using System.Text.RegularExpressions;

namespace Beadwork
{
    public class Picture
    {
        public int Id { get; }
        public string Item_number { get; }
        public string Author { get; }
        public string Title { get; }
        public Picture(int id,string author, string item_number, string title)
        {
            Id = id;
            Author = author;
            Item_number = item_number;
            Title = title;
        }
        internal static bool IsItemNumber(string s)
        {
            if (s == null)
                return false;

            s = s.Replace("-", "")
                .Replace(" ", "")
                .ToUpper();

            return Regex.IsMatch(s, "^IN\\d{8}$");
        }
    }
}
