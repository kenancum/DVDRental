using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
namespace DVDRental
{
    class MovieMapper: IMapper<Movie>
    {
        private static readonly string CONNECTION_STRING = "Server=localhost;Port=5432;Database=rental;User Id=postgres;Password=postgres;";
        private readonly Dictionary<int, Movie> _cache = new Dictionary<int, Movie>();

        public static MovieMapper Instance { get; } = new MovieMapper();
        // This is a singleton, so constructor is private
        private MovieMapper() { }
        public Movie GetByID(int id)
        {
            if (_cache.ContainsKey(id))
            {
                return _cache[id];
            }
            Movie movie = GetByIDFromDB(id);
            _cache.Add(movie.id, movie);
            return movie;
        }

        private Movie GetByIDFromDB(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM movies WHERE movie_id = @ID", conn))
                {
                    command.Parameters.AddWithValue("@ID", id);

                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        double pp = Convert.ToDouble(reader["price"]);
                        var copies = CopyMapper.Instance.GetByMovieId(id);
                        return new Movie((string)reader["title"], (int)reader["year"],(int)reader["age_restriction"], id, pp, copies);
                    }
                }
            }
            return null;
        }

        public int movieCount()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("select COUNT(title) from movies", conn))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return Int32.Parse(reader[0].ToString());
                    }
                }
            }
            return 0;
        }
        public int getMovieIDByCopyID(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("select*from movies m " +
                                                       "join copies c on c.movie_id = m.movie_id" +
                                                       "where copy_id=@copy_id", conn))
                {
                    command.Parameters.AddWithValue("@copy_id", id);

                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return Int32.Parse(reader["movie_id"].ToString());
                    }
                }
            }
            return 0;
        }

        public int getLenght()
        {

            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (var command = new NpgsqlCommand($"SELECT COUNT(*) FROM  movies as count", conn))
                {

                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        return int.Parse(reader["count"].ToString());
                    }
                    return 0;
                }
            }
        }
        public int getMaxMovieID()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                int val = 0;
                using (var command = new NpgsqlCommand("select max(movie_id) from movies", conn))
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
        public void Save(Movie movie)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                int movieID = getMaxMovieID();
                int copyID = CopyMapper.Instance.getMaxCopyID();

                conn.Open();
                // This is an UPSERT operation - if record doesn't exist in the database it is created, otherwise it is updated
                using (var command = new NpgsqlCommand("INSERT INTO movies(title, year,age_restriction,movie_id,price) " +
                    "VALUES (@title, @year, @age_restriction, @ID,@price);" +
                    "INSERT INTO copies(copy_id, available,movie_id) " +
                    "VALUES (@copy_id, true, @ID)", conn))
                {

                    command.Parameters.AddWithValue("@ID", getMaxMovieID());
                    command.Parameters.AddWithValue("@title", movie.title);
                    command.Parameters.AddWithValue("@year", movie.year);
                    command.Parameters.AddWithValue("@age_restriction", movie.ageRestriction);
                    command.Parameters.AddWithValue("@price", movie.price);
                    command.Parameters.AddWithValue("@copy_id", copyID);
                    command.ExecuteNonQuery();

                }
            }
            _cache[movie.id] = movie;
        }

        public void Delete(Movie t)
        {
            throw new NotImplementedException();
        }
    }
}
