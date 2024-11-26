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
       public string tableidentifier(JsonDocument document){

            try
            {
                string id = document.RootElement.GetProperty("end_device_ids").GetProperty("device_id").GetString();

                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can not identify the table: {ex.Message}");
                return string.Empty;

            }
        }
}

