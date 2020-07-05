using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using System.Linq;
using System.Threading.Tasks;

namespace DVDRental
{
    class CopyMapper : IMapper<Copy>
    {
        private static readonly string CONNECTION_STRING = "Server=localhost;Port=5432;Database=rental;User Id=postgres;Password=postgres;";
        private readonly Dictionary<int, Copy> _cache = new Dictionary<int, Copy>();
        public static CopyMapper Instance { get; } = new CopyMapper();

        private CopyMapper() { }
        public Copy GetByID(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM copies WHERE copy_id = @ID", conn))
                {
                    command.Parameters.AddWithValue("@ID", id);

                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return new Copy(id, (bool)reader["available"], (int)reader["movie_id"]);
                    }
                }
            }
            return null;
        }
        public List<Copy> GetByMovieId(int movieId)
        {
            List<Copy> list = new List<Copy>();
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM copies WHERE movie_id = @movieID", conn))
                {
                    command.Parameters.AddWithValue("@movieID", movieId);

                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new Copy((int)reader["copy_id"], (bool)reader["available"], (int)reader["movie_id"]));
                    }
                }
            }
            return list;
        }

        public int getMaxCopyID()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                int val = 0;
                using (var command = new NpgsqlCommand("select max(copy_id) from copies", conn))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        val = Int32.Parse(reader[0].ToString()) + 1;
                    }
                }
                return val;
            }
        }
        public void Delete(Copy t)
        {
            throw new NotImplementedException();
        }

        public void Save(Copy copy)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("INSERT INTO copies(copy_id, available, movie_id) " +
                    "VALUES (@ID, @available, @movieId) " +
                    "ON CONFLICT (copy_id) DO UPDATE " +
                    "SET available = @available, movie_id = @movieId", conn))
                {
                    command.Parameters.AddWithValue("@ID", copy.id);
                    command.Parameters.AddWithValue("@available", copy.available);
                    command.Parameters.AddWithValue("@movieId", copy.movieId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
