using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using System.Text.Json;


public class values
{
    // 'end_device_ids'
    public string DeviceId { get; set; }
    public string ApplicationId { get; set; }
    public string DevEUI { get; set; }
    public string JoinEUI { get; set; }
    public string DevAddress { get; set; }

    // 'uplink_message'
    public string SessionKeyId { get; set; }
    public int FPort { get; set; }
    public int FCnt { get; set; }
    public string FrmPayload { get; set; }
    public double Humidity { get; set; }
    public double Light { get; set; }
    public double Pressure { get; set; }
    public double Temperature { get; set; }
    public DateTime Timestamp { get; set; }

    // 'rx_metadata'
    public string GatewayId { get; set; }
    public string GatewayEUI { get; set; }
    public DateTime MetadataTime { get; set; }
    public long MetadataTimestamp { get; set; }
    public double RSSI { get; set; }
    public double ChannelRSSI { get; set; }
    public double SNR { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Altitude { get; set; }

    // 'settings'
    public int Bandwidth { get; set; }
    public int SpreadingFactor { get; set; }
    public string CodingRate { get; set; }
    public string Frequency { get; set; }
    public long SettingsTimestamp { get; set; }
}

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

                             // End_device
                             string insertEndDeviceQuery = @"
                INSERT INTO wet.end_device (device_id, application, devEUI, joinEUI, dev_address)
                VALUES (@DeviceId, @ApplicationId, @DevEUI, @JoinEUI, @DevAddress)";

                             using (SqlCommand command = new SqlCommand(insertEndDeviceQuery, connection))
                             {
                                 command.Parameters.AddWithValue("@DeviceId", weatherData.DeviceId);
                                 command.Parameters.AddWithValue("@ApplicationId", weatherData.ApplicationId);
                                 command.Parameters.AddWithValue("@DevEUI", weatherData.DevEUI);
                                 command.Parameters.AddWithValue("@JoinEUI", weatherData.JoinEUI);
                                 command.Parameters.AddWithValue("@DevAddress", weatherData.DevAddress);
                                 command.ExecuteNonQuery();
                             }

                             // Uplink_messages
                             string insertUplinkMessagesQuery = @"
                INSERT INTO wet.uplink_messages (device_id, sesion_key_id, FPort, FCnt, frm_Payload, humidity, pressure, temperature, timestamp)
                VALUES (@DeviceId, @SessionKeyId, @FPort, @FCnt, @FrmPayload, @Humidity, @Pressure, @Temperature, @Timestamp)";

                             using (SqlCommand command = new SqlCommand(insertUplinkMessagesQuery, connection))
                             {
                                 command.Parameters.AddWithValue("@DeviceId", weatherData.DeviceId);
                                 command.Parameters.AddWithValue("@SessionKeyId", weatherData.SessionKeyId);
                                 command.Parameters.AddWithValue("@FPort", weatherData.FPort);
                                 command.Parameters.AddWithValue("@FCnt", weatherData.FCnt);
                                 command.Parameters.AddWithValue("@FrmPayload", weatherData.FrmPayload);
                                 command.Parameters.AddWithValue("@Humidity", weatherData.Humidity);
                                 command.Parameters.AddWithValue("@Pressure", weatherData.Pressure);
                                 command.Parameters.AddWithValue("@Temperature", weatherData.Temperature);
                                 command.Parameters.AddWithValue("@Timestamp", weatherData.Timestamp);
                                 command.ExecuteNonQuery();
                             }

                             // Rx_metadata
                             string insertRxMetadataQuery = @"
                INSERT INTO wet.rx_metadata (message_id, gate_way_id, gate_way_EUI, time, timestamp, RSSI, channel_RSSI, SNR, latitude, longitude, altitude)
                VALUES (SCOPE_IDENTITY(), @GatewayId, @GatewayEUI, @MetadataTime, @MetadataTimestamp, @RSSI, @ChannelRSSI, @SNR, @Latitude, @Longitude, @Altitude)";

                             using (SqlCommand command = new SqlCommand(insertRxMetadataQuery, connection))
                             {
                                 command.Parameters.AddWithValue("@GatewayId", weatherData.GatewayId);
                                 command.Parameters.AddWithValue("@GatewayEUI", weatherData.GatewayEUI);
                                 command.Parameters.AddWithValue("@MetadataTime", weatherData.MetadataTime);
                                 command.Parameters.AddWithValue("@MetadataTimestamp", weatherData.MetadataTimestamp);
                                 command.Parameters.AddWithValue("@RSSI", weatherData.RSSI);
                                 command.Parameters.AddWithValue("@ChannelRSSI", weatherData.ChannelRSSI);
                                 command.Parameters.AddWithValue("@SNR", weatherData.SNR);
                                 command.Parameters.AddWithValue("@Latitude", weatherData.Latitude);
                                 command.Parameters.AddWithValue("@Longitude", weatherData.Longitude);
                                 command.Parameters.AddWithValue("@Altitude", weatherData.Altitude);
                                 command.ExecuteNonQuery();
                             }

                             // Setting
                             string insertSettingsQuery = @"
                INSERT INTO wet.settings (message_id, bandwith, spreading_factor, coding_rate, frequency, timestamp)
                VALUES (SCOPE_IDENTITY(), @Bandwidth, @SpreadingFactor, @CodingRate, @Frequency, @SettingsTimestamp)";

                             using (SqlCommand command = new SqlCommand(insertSettingsQuery, connection))
                             {
                                 command.Parameters.AddWithValue("@Bandwidth", weatherData.Bandwidth);
                                 command.Parameters.AddWithValue("@SpreadingFactor", weatherData.SpreadingFactor);
                                 command.Parameters.AddWithValue("@CodingRate", weatherData.CodingRate);
                                 command.Parameters.AddWithValue("@Frequency", weatherData.Frequency);
                                 command.Parameters.AddWithValue("@SettingsTimestamp", weatherData.SettingsTimestamp);
                                 command.ExecuteNonQuery();
                             }
                         }

                         Console.WriteLine("inserted succesfyllu.");
                     }
                     catch (Exception ex)
                     {
                         Console.WriteLine($"Error inserting data : {ex.Message}");
                     }
                 }
             }



         }