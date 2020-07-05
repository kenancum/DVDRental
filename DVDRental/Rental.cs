using System;
using System.Collections.Generic;
using System.Text;

namespace DVDRental
{
    class Rental
    {
        public int? id { get; private set; }
        public int clientId { get; private set; }
        public int copyId { get; private set; }
        public string dateOfRental { get; private set; }
        public string dateOfReturn { get; private set; }


        public Rental(int? id, int clientId, int copyId, string dateOfRental = null, string dateOfReturn = null)
        {
            this.id = id;
            this.clientId = clientId;
            this.copyId = copyId;
            this.dateOfRental = dateOfRental;
            this.dateOfReturn = dateOfReturn;
        }
    }
}
