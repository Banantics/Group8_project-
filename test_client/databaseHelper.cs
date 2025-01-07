using System;
using System.Data.SqlClient;
using values;

namespace Client
{
    public class databaseHelper
    {

        public void insert(vals.values weatherData)
        {
            string connectionString =
                "Server=weathergroup8.database.windows.net;Database=weather8;User Id= Group8;Password=Zxcqwe123!!";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Insert into uplink_messages table
                    string insertUplinkMessagesQuery = @"
                        INSERT INTO wet.uplink_messages (device_id, session_key_id, f_port, f_cnt, frm_payload, timestamp)
                        VALUES (@DeviceId, @SessionKeyId, @FPort, @FCnt, @FrmPayload, @Timestamp);
                        SELECT SCOPE_IDENTITY();";

                    int messageId;
                    using (SqlCommand command = new SqlCommand(insertUplinkMessagesQuery, connection))
                    {
                        command.Parameters.AddWithValue("@DeviceId", weatherData.DeviceId);
                        command.Parameters.AddWithValue("@SessionKeyId", weatherData.SessionKeyId);
                        command.Parameters.AddWithValue("@FPort", weatherData.FPort);
                        command.Parameters.AddWithValue("@FCnt", weatherData.FCnt);
                        command.Parameters.AddWithValue("@FrmPayload", weatherData.FrmPayload);
                        command.Parameters.AddWithValue("@Timestamp", weatherData.Timestamp);
                        messageId = Convert.ToInt32(command.ExecuteScalar());
                    }

                    // Insert into rx_metadata table
                    string insertRxMetadataQuery = @"
                        INSERT INTO wet.rx_metadata (message_id, gateway_id, gateway_eui, time, timestamp, rssi, channel_rssi, snr, latitude, longitude, altitude)
                        VALUES (@MessageId, @GatewayId, @GatewayEUI, @MetadataTime, @MetadataTimestamp, @RSSI, @ChannelRSSI, @SNR, @Latitude, @Longitude, @Altitude)";

                    using (SqlCommand command = new SqlCommand(insertRxMetadataQuery, connection))
                    {
                        command.Parameters.AddWithValue("@MessageId", messageId);
                        command.Parameters.AddWithValue("@GatewayId", weatherData.GatewayId);
                        command.Parameters.AddWithValue("@GatewayEUI",
                            string.IsNullOrEmpty(weatherData.GatewayEUI)
                                ? (object)DBNull.Value
                                : weatherData.GatewayEUI);
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

                    // Insert into device-specific data table (MKR or LHT)
                    if (weatherData.DeviceId.StartsWith("mkr", StringComparison.OrdinalIgnoreCase))
                    {
                        string insertMkrDataQuery = @"
                            INSERT INTO wet.mkr_data (message_id, humidity, pressure, temperature)
                            VALUES (@MessageId, @Humidity, @Pressure, @Temperature)";

                        using (SqlCommand command = new SqlCommand(insertMkrDataQuery, connection))
                        {
                            command.Parameters.AddWithValue("@MessageId", messageId);
                            command.Parameters.AddWithValue("@Humidity", weatherData.Humidity);
                            command.Parameters.AddWithValue("@Pressure", weatherData.Pressure);
                            command.Parameters.AddWithValue("@Temperature", weatherData.Temperature);
                            command.ExecuteNonQuery();
                        }
                    }
                    else if (weatherData.DeviceId.StartsWith("lht", StringComparison.OrdinalIgnoreCase))
                    {
                        string insertLhtDataQuery = @"
                            INSERT INTO wet.lht_data (message_id, battery_voltage, battery_status, illumination, work_mode, humidity, temperature)
                            VALUES (@MessageId, @BatteryVoltage, @BatteryStatus, @Illumination, @WorkMode, @Humidity, @Temperature)";

                        using (SqlCommand command = new SqlCommand(insertLhtDataQuery, connection))
                        {
                            command.Parameters.AddWithValue("@MessageId", messageId);
                            command.Parameters.AddWithValue("@BatteryVoltage", weatherData.BatteryVoltage);
                            command.Parameters.AddWithValue("@BatteryStatus", weatherData.BatteryStatus);
                            command.Parameters.AddWithValue("@Illumination",
                                weatherData.Illumination ?? (object)DBNull.Value);

                            command.Parameters.AddWithValue("@WorkMode",
                                string.IsNullOrEmpty(weatherData.WorkMode)
                                    ? (object)DBNull.Value
                                    : weatherData.WorkMode);

                    command.Parameters.AddWithValue("@Humidity", weatherData.Humidity);
                    command.Parameters.AddWithValue("@Temperature", weatherData.Temperature);
                    command.ExecuteNonQuery();
                }
            }
        }

                Console.WriteLine("Inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting data: {ex.Message}");
            }
        }
        public void InsertIntoSpecificTable(loravalues loraWeatherData)
        {
            string connectionString = "Server=weathergroup8.database.windows.net;Database=weather8;User Id=Group8;Password=Zxcqwe123!!";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Define the INSERT query with placeholders for parameters
                    string insertQuery = @"
            INSERT INTO dbo.lora (Illumination, Pressure, Temperature, Humidity, Rssi, Timestamp)
            VALUES (@illumination, @pressure, @temperature, @humidity, @rssi, @timestamp)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        // Add parameters using AddWithValue
                        command.Parameters.AddWithValue("@illumination", loraWeatherData.Illumination);
                        command.Parameters.AddWithValue("@pressure", loraWeatherData.Pressure);
                        command.Parameters.AddWithValue("@temperature", loraWeatherData.Temperature);
                        command.Parameters.AddWithValue("@humidity", loraWeatherData.Humidity);
                        command.Parameters.AddWithValue("@rssi", loraWeatherData.Rssi);
                        command.Parameters.AddWithValue("@timestamp", loraWeatherData.Timestamp);

                        // Execute the INSERT query
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Data inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No rows affected. Check the data or table.");
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error inserting data into table: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error inserting data into table: {ex.Message}");
            }
        }

        
    }


}
