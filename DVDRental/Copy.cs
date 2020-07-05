using System;
using System.Collections.Generic;
using System.Text;

namespace DVDRental
{
    class Copy
    {
        public int id { get; private set; }
        public bool available { get; private set; }
        public int movieId { get; private set; }
        public DateTime? rentalTime { get; private set; }
        public int? clientID { get; private set; }


        public Copy(int id, bool available, int movieId, DateTime? rentalTime = null, int? clientID = null)
        {
            this.id = id;
            this.available = available;
            this.movieId = movieId;
            this.rentalTime = rentalTime;
            this.clientID = clientID;
        }
    }
}
