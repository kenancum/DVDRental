using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace DVDRental
{
    class Employee
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public string surname { get; private set; }
        public string city { get; private set; }
        public SqlSingle salary { get; private set; }

        public Employee(int id, string name, string surname, string city,SqlSingle salary)
        {
            this.id = id;
            this.name = name;
            this.surname = surname;
            this.city = city;
            this.salary = salary;
        }
    }
}
