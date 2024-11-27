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
                    // End_device
                    DeviceId = document.RootElement.GetProperty("end_device_ids").GetProperty("device_id").GetString(),
                    ApplicationId = document.RootElement.GetProperty("end_device_ids").GetProperty("application_ids").GetProperty("application_id").GetString(),
                    DevEUI = document.RootElement.GetProperty("end_device_ids").GetProperty("dev_eui").GetString(),
                    JoinEUI = document.RootElement.GetProperty("end_device_ids").GetProperty("join_eui").GetString(),
                    DevAddress = document.RootElement.GetProperty("end_device_ids").GetProperty("dev_addr").GetString(),

                    // Uplink_messages 
                    SessionKeyId = document.RootElement.GetProperty("uplink_message").GetProperty("session_key_id").GetString(),
                    FPort = document.RootElement.GetProperty("uplink_message").GetProperty("f_port").GetInt32(),
                    FCnt = document.RootElement.GetProperty("uplink_message").GetProperty("f_cnt").GetInt32(),
                    FrmPayload = document.RootElement.GetProperty("uplink_message").GetProperty("frm_payload").GetString(),
                    Humidity = GetJsonValue(document, "uplink_message", "decoded_payload", "humidity"),
                    Light = GetJsonValue(document, "uplink_message", "decoded_payload", "light"),
                    Pressure = GetJsonValue(document, "uplink_message", "decoded_payload", "pressure"),
                    Temperature = GetJsonValue(document, "uplink_message", "decoded_payload", "temperature"),
                    Timestamp = DateTime.Parse(document.RootElement.GetProperty("uplink_message").GetProperty("settings").GetProperty("time").GetString()),

                    // Rx_metadata
                    GatewayId = document.RootElement.GetProperty("uplink_message").GetProperty("rx_metadata")[0].GetProperty("gateway_ids").GetProperty("gateway_id").GetString(),
                    GatewayEUI = document.RootElement.GetProperty("uplink_message").GetProperty("rx_metadata")[0].GetProperty("gateway_ids").GetProperty("eui").GetString(),
                    MetadataTime = DateTime.Parse(document.RootElement.GetProperty("uplink_message").GetProperty("rx_metadata")[0].GetProperty("time").GetString()),
                    MetadataTimestamp = document.RootElement.GetProperty("uplink_message").GetProperty("rx_metadata")[0].GetProperty("timestamp").GetInt64(),
                    RSSI = document.RootElement.GetProperty("uplink_message").GetProperty("rx_metadata")[0].GetProperty("rssi").GetDouble(),
                    ChannelRSSI = document.RootElement.GetProperty("uplink_message").GetProperty("rx_metadata")[0].GetProperty("channel_rssi").GetDouble(),
                    SNR = document.RootElement.GetProperty("uplink_message").GetProperty("rx_metadata")[0].GetProperty("snr").GetDouble(),
                    Latitude = document.RootElement.GetProperty("uplink_message").GetProperty("rx_metadata")[0].GetProperty("location").GetProperty("latitude").GetDouble(),
                    Longitude = document.RootElement.GetProperty("uplink_message").GetProperty("rx_metadata")[0].GetProperty("location").GetProperty("longitude").GetDouble(),
                    Altitude = document.RootElement.GetProperty("uplink_message").GetProperty("rx_metadata")[0].GetProperty("location").GetProperty("altitude").GetDouble(),

                    // Settings 
                    Bandwidth = document.RootElement.GetProperty("uplink_message").GetProperty("settings").GetProperty("data_rate").GetProperty("lora").GetProperty("bandwidth").GetInt32(),
                    SpreadingFactor = document.RootElement.GetProperty("uplink_message").GetProperty("settings").GetProperty("data_rate").GetProperty("lora").GetProperty("spreading_factor").GetInt32(),
                    CodingRate = document.RootElement.GetProperty("uplink_message").GetProperty("settings").GetProperty("data_rate").GetProperty("lora").GetProperty("coding_rate").GetString(),
                    Frequency = document.RootElement.GetProperty("uplink_message").GetProperty("settings").GetProperty("frequency").GetString(),
                    SettingsTimestamp = document.RootElement.GetProperty("uplink_message").GetProperty("settings").GetProperty("timestamp").GetInt64()
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