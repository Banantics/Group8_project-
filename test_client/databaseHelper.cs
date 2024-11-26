using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using System.Text.Json;

using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Client;
using vals;

namespace Client
{
    public class databaseHelper
    {
        
        public void insert(values weatherData)
        {
            string connectionString = "Server=DESKTOP-BRPSKK1;Database=Weather;Trusted_Connection=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connected to SQL Server.");

                    string query = @"
                INSERT INTO WeatherData (City, Time, Temperature, AmbientLight, Humidity)
                VALUES (@City, @Time, @Temperature, @AmbientLight, @Humidity)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@City", weatherData.AplicationId);
                        command.Parameters.AddWithValue("@Time", weatherData.Time);
                        command.Parameters.AddWithValue("@Temperature", weatherData.Temperature);
                        command.Parameters.AddWithValue("@AmbientLight", weatherData.AmbientLight);
                        command.Parameters.AddWithValue("@Humidity", weatherData.Humidity);

                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Data inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting data into database: {ex.Message}");
            }
        }
    }



}
