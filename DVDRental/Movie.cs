using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace DVDRental
{
    class Movie
    {
        public string title { get; private set; }
        public int year{ get; private set; }
        public int ageRestriction { get; private set; }
        public int id { get; private set; }
        public double price { get; private set; }
        public List<Copy> Copies { get; private set; }

        public Movie(string title, int year, int ageRestriction, int id, double price, List<Copy> copies = null)
        {
            this.title = title;
            this.year = year;
            this.ageRestriction = ageRestriction;
            this.id = id;
            this.price = price;
            this.Copies = copies;
        }
    }
}
