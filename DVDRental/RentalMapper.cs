using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using Npgsql;
namespace DVDRental
{
    class RentalMapper : IMapper<Rental>
    {
        private static readonly string CONNECTION_STRING = "Server=localhost;Port=5432;Database=rental;User Id=postgres;Password=postgres;";
        private readonly Dictionary<int, RentalMapper> _cache = new Dictionary<int, RentalMapper>();

        public static RentalMapper Instance { get; } = new RentalMapper();

        private RentalMapper() { }
        public Rental GetByID(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM rentals WHERE rental_id = @ID", conn))
                {
                    command.Parameters.AddWithValue("@ID", id);

                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return new Rental((int)reader["rental_id"],(int)reader["client_id"],(int)reader["copy_id"],(string)reader["date_of_rental"].ToString(),(string)reader["date_of_return"].ToString());
                    }
                }
            }
            return null;
        }
        public List<Rental> GetByClientId(int clientID)
        {
            List<Rental> list = new List<Rental>();
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM rentals WHERE client_id = @clientID", conn))
                {
                    command.Parameters.AddWithValue("@clientID", clientID);

                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new Rental((int)reader["rental_id"], (int)reader["client_id"], (int)reader["copy_id"], (string)reader["date_of_rental"].ToString(), (string)reader["date_of_return"].ToString()));
                    }
                }
            }
            return list;
        }
        
        
        public void Delete(Rental t)
        {
            throw new NotImplementedException();
        }

        public Rental getByCopyID(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                //BURDASIN
                using (var command = new NpgsqlCommand("select * from rentals where copy_id = @ID order by rental_id desc limit 1", conn))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    NpgsqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        return new Rental((int)reader["rental_id"], (int)reader["client_id"], (int)reader["copy_id"], (string)reader["date_of_rental"].ToString(), (string)reader["date_of_return"].ToString());
                    }
                    else return null;
                }
            }
            return null;
        }

        public int getSumPriceFromDate(string date)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("select SUM(price) as sum from rentals r join copies c on c.copy_id = r.copy_id join movies m on m.movie_id = c.movie_id where date_of_rental > @date::timestamp", conn))
                {
                    command.Parameters.AddWithValue("@date", date);

                    NpgsqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    if (reader["sum"].ToString()!= "")
                    {
                        return int.Parse(reader["sum"].ToString());
                    }
                    else return 0;
                    
                }
            }
        }
        private List<Rental> allRentals()
        {
            List<Rental> list = new List<Rental>();
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("SELECT * from rentals", conn))
                    {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                        {
                            list.Add(new Rental((int)reader["rental_id"], (int)reader["client_id"],(int)reader["copy_id"],(string)reader["date_of_rental"].ToString(), (string)reader["date_of_return"].ToString()));
                        }
                    }
            }
            return list;
        }
        public List<Rental> overdueRentals()
        {
            List<Rental> list = new List<Rental>();
            List<Rental> allRentals = RentalMapper.Instance.allRentals();

            string now = DateTime.Now.ToString();
            int daysBetween;
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                
                    using (var command = new NpgsqlCommand("SELECT DATE_PART('day',date_of_return - date_of_rental) as days,rental_id from rentals", conn))
                    {
                        conn.Open();
                        NpgsqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            daysBetween = Int32.Parse(reader[0].ToString());
                            if (daysBetween > 14)
                            {
                                list.Add(Instance.GetByID(Int32.Parse(reader[1].ToString())));
                            }
                        }
                    }               
            }
            return list;
        }
        private int getMaxRentalID()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                int val = 0;
                using (var command = new NpgsqlCommand("select max(rental_id) from rentals", conn))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        val = Int32.Parse(reader[0].ToString())+1;
                    }
                }
            return val;
            }
        }
        public void Save(Rental t)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                int? rentalID;
                conn.Open();
                if (t.id == null)
                {
                    rentalID = getMaxRentalID();
                }
                else
                    rentalID = t.id;
                using (var command = new NpgsqlCommand("insert into rentals (copy_id, client_id, date_of_rental, date_of_return, rental_id)" +
                                                       "values( @copy_id,@clientID, @timenow, null, @rental_id)" +
                                                       "ON CONFLICT(rental_id) DO UPDATE " +
                                                       "SET date_of_return = @timenow", conn))
                {
                    command.Parameters.AddWithValue("@clientID", t.clientId);
                    command.Parameters.AddWithValue("@copy_id", t.copyId);
                    command.Parameters.AddWithValue("@rental_id", rentalID);
                    command.Parameters.AddWithValue("@timenow", DateTime.Now);

                    NpgsqlDataReader reader = command.ExecuteReader();
                }
            }
        }
    }
}
