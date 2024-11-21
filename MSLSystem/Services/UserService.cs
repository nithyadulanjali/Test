using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using MSLSystem.Models;

namespace MSLSystem.Services
{
    public class UserService
    {
        private readonly string ConnectionStrings;

        public UserService(string connectionString)
        {
            ConnectionStrings = connectionString;
        }

        public User GetUserByEmail(string E_MAIL)
        {
            if (string.IsNullOrEmpty(E_MAIL))
                return null;

            User user = null;

            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionStrings))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("SELECT USER_ID, E_MAIL, PASSWORD FROM USER_REG WHERE E_MAIL = :E_MAIL", conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("E_MAIL", E_MAIL));

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new User
                                {
                                    USER_ID = reader.IsDBNull(0) ? null : reader.GetString(0),
                                    E_MAIL = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    PASSWORD = reader.IsDBNull(2) ? null : reader.GetString(2)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return user;
        }

    }


}

