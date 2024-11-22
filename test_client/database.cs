using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using System.Text.Json;


public class values
{   
    public string AplicationId { get; set; }
    public DateTime Time { get; set; }
    public double Temperature { get; set; }
    public double AmbientLight { get; set; }
    public double Humidity { get; set; }

}
public class valuegetter
    {
        public values SomeTable(JsonDocument document)
        {
            try
            {
                var weatherData = new values
                {
                    AplicationId = document.RootElement.GetProperty("end_device_ids").GetProperty("device_id").GetString(),
                    Time = DateTime.Parse(document.RootElement.GetProperty("received_at").GetString()),
                    Temperature = GetJsonValue(document, "uplink_message", "decoded_payload", "temperature"),
                    Humidity = GetJsonValue(document, "uplink_message", "decoded_payload", "humidity"),
                    AmbientLight = GetJsonValue(document, "uplink_message", "decoded_payload", "light")
                };

                return weatherData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SomeTable: {ex.Message}");
                return null; 
            }
        }
    
    
    
    
    double GetJsonValue(JsonDocument jsonDocument, string parentProperty, string childProperty, string targetProperty)
    {
        try
        {

            var parentElement = jsonDocument.RootElement.GetProperty(parentProperty);
            var childElement = parentElement.GetProperty(childProperty);
            return childElement.GetProperty(targetProperty).GetDouble();
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Error extracting property {targetProperty}: {ex.Message}");
            return 0;
        }
    }

}

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