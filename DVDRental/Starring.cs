using System;
using System.Collections.Generic;
using System.Text;

namespace DVDRental
{
    class Starring
    {
        public int actorId { get; private set; }
        public int movieId { get; private set; }
        public Starring(int actorId, int movieId)
        {
            this.actorId = actorId;
            this.movieId = movieId;
        }
    }
}
