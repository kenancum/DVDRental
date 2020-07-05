using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Npgsql;


namespace DVDRental
{
    class ClientMapper : IMapper<Client>
    {
        private static readonly string CONNECTION_STRING = "Server=localhost;Port=5432;Database=rental;User Id=postgres;Password=postgres;";
        private readonly Dictionary<int?, Client> _cache = new Dictionary<int?, Client>();

        public static ClientMapper Instance { get; } = new ClientMapper();
        // This is a singleton, so constructor is private
        private ClientMapper() { }

        public void Delete(Client t)
        {
            throw new NotImplementedException();
        }

        public Client GetByID(int id)
        {
            if (_cache.ContainsKey(id))
            {
                return _cache[id];
            }
            Client client = GetByIDFromDB(id);
            _cache.Add(client.id, client);
            return client;
        }

        private Client GetByIDFromDB(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM clients WHERE client_id = @ID", conn))
                {
                    command.Parameters.AddWithValue("@ID", id);

                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        var rentals = RentalMapper.Instance.GetByClientId(id);
                        return new Client((int)reader["client_id"], (string)reader["first_name"], (string)reader["last_name"], (string)reader["birthday"].ToString());
                    }
                }
            }
            return null;
        }

        private int getMaxClientID()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                int val = 0;
                using (var command = new NpgsqlCommand("select max(client_id) from clients", conn))
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

        public void Save(Client t)
        {
            int? clientID;
            if (t.id == null)
            {
                clientID = getMaxClientID();
            }
            else
                clientID = t.id;
            using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("INSERT INTO clients(client_id, first_name, last_name,birthday) " +
                    "VALUES (@ID, @firstName, @lastName,  @birthday::timestamp) " +
                    "ON CONFLICT (client_id) DO UPDATE " +
                    "SET first_name=@firstName, last_name = @lastName, birthday=@birthday::timestamp", conn))
                {
                    command.Parameters.AddWithValue("@ID", clientID);
                    command.Parameters.AddWithValue("@firstName", t.name);
                    command.Parameters.AddWithValue("@lastName", t.surname);
                    command.Parameters.AddWithValue("@birthday", t.birthday);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
