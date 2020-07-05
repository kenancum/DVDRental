using System;
using System.Collections.Generic;
using System.Text;

namespace DVDRental
{
    class Client
    {
        public int? id { get; private set; }
        public string name { get; private set; }
        public string surname { get; private set; }
        public string birthday { get; private set; }
        public List<Rental> Rentals{ get; private set; }

        public Client(int? id, string name, string surname, string birthday, List<Rental> Rentals = null)
        {
            this.id = id;
            this.name = name;
            this.surname = surname;
            this.birthday = birthday;
            this.Rentals = Rentals;
        }
    }
}
