using System;

namespace Beadwork
{
    public class Picture
    {
        public int Id { get; }
        public string Title { get; }
        public Picture(int id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
