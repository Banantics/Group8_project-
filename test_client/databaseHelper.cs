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
            // string connectionString = "Server=DESKTOP-BRPSKK1;Database=weather;Trusted_Connection=True;";
            string connectionString = "Server=Alians;Database=weather;Trusted_Connection=True;";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connected to SQL Server.");

                    string  checkDeviceQuery = "SELECT 1 FROM wet.end_device WHERE device_id = @DeviceId";
                    bool deviceExists = false;

                    using (SqlCommand checkCommand = new SqlCommand(checkDeviceQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@DeviceId", weatherData.DeviceId);
                        object result = checkCommand.ExecuteScalar();
                        deviceExists = result != null;
                    }

                    if (!deviceExists)
                    {

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
                    }
                    else
                    {
                        Console.WriteLine($"Device with ID {weatherData.DeviceId} already exists in the end_device table. Skipping insertion.");
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
