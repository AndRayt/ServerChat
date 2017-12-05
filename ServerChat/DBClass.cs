using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using System;

namespace ServerChat
{
    public class DBClass
    {
        private string connstr;
        private MySqlConnection connection;

        public DBClass()
        {
            connstr = "server=localhost;user=root;database=chat;password=admin";
            connection = new MySqlConnection(connstr);
        }

        ~DBClass()
        {
            connection.Close();
        }

        public string getAllMessage()
        {
            StringBuilder result = new StringBuilder("");
            using (connection)
            {
                connection.Open();
                string sql = "SELECT namePerson, text FROM messege";
                MySqlCommand command = new MySqlCommand(sql, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string msg = reader[0].ToString() + ": " + reader[1].ToString();
                    result.Append(msg);
                    result.Append("\n");
                }
            }
            return result.ToString();
        }

        public bool getNameDB(string name)
        {
            string sql = "SELECT name FROM person WHERE name = \"" + name + "\"";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader data = command.ExecuteReader();
            if (data.Read())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void addMessage(string name, string text)
        {
            using (connection)
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                command.CommandText = "INSERT INTO messege(namePerson, text) VALUES(?namePerson, ?text)";
                command.Parameters.Add("?namePerson", MySqlDbType.VarChar).Value = name;
                command.Parameters.Add("?text", MySqlDbType.VarChar).Value = text;
                command.ExecuteNonQuery();
            }
        }
    }
}
