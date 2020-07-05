using System;
using System.Collections.Generic;
using System.Text;

namespace DVDRental
{
    class Actor
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public string surname { get; private set; }
        public string birthday { get; private set; }
        
        public Actor(int id, string name, string surname, string birthday)
        {
            this.id = id;
            this.name = name;
            this.surname = surname;
            this.birthday = birthday;
        }
    }
}
