using System;
using System.Text.Json;
using values;

public class valuegetter
{
    
 public loravalues specifictable(JsonDocument document)
{
    try
    {
        var valuesLora = new loravalues();

        // Accessing the root element
        var root = document.RootElement;

        // Accessing uplink_message and decoded_payload
        if (root.TryGetProperty("uplink_message", out var uplinkMessage))
        {
            if (uplinkMessage.TryGetProperty("decoded_payload", out var decodedPayload))
            {
                // Reading individual properties directly
                if (decodedPayload.TryGetProperty("temperature", out var temp))
                {
                    Console.WriteLine($"Temperature: {temp}");
                    valuesLora.Temperature = temp.GetDouble();
                }
                if (decodedPayload.TryGetProperty("humidity", out var humidity))
                {
                    Console.WriteLine($"Humidity: {humidity}");
                    valuesLora.Humidity = humidity.GetDouble();
                }
                if (decodedPayload.TryGetProperty("light", out var light))
                {
                    Console.WriteLine($"Light: {light}");
                    valuesLora.Illumination = light.GetDouble();
                }
                if (decodedPayload.TryGetProperty("pressure", out var pressure))
                {
                    Console.WriteLine($"Pressure: {pressure}");
                    valuesLora.Pressure = pressure.GetDouble();
                }
            }

            // Accessing rx_metadata
            if (uplinkMessage.TryGetProperty("rx_metadata", out var rxMetadataArray))
            {
                foreach (var metadata in rxMetadataArray.EnumerateArray())
                {
                    if (metadata.TryGetProperty("rssi", out var rssi))
                    {
                        Console.WriteLine($"RSSI: {rssi}");
                        valuesLora.Rssi = rssi.GetDouble();
                        break; // Use the first RSSI value
                    }
                }
            }
        }

        // Setting the timestamp
        valuesLora.Timestamp = DateTime.Now;

        return valuesLora;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error parsing JSON: {ex.Message}");
        return null;
    }
}




    public vals.values table(JsonDocument document, string deviceType)
    {
        try
        {
            var weatherData = new vals.values
            {
                DeviceId = TryGetString(document, "end_device_ids", "device_id"),
                Application = TryGetString(document, "end_device_ids", "application_ids", "application_id"),
                DevEUI = TryGetString(document, "end_device_ids", "dev_eui"),
                JoinEUI = TryGetString(document, "end_device_ids", "join_eui"),
                DevAddress = TryGetString(document, "end_device_ids", "dev_addr"),
                SessionKeyId = TryGetString(document, "uplink_message", "session_key_id"),
                FPort = TryGetInt32(document, "uplink_message", "f_port"),
                FCnt = TryGetInt32(document, "uplink_message", "f_cnt"),
                FrmPayload = TryGetString(document, "uplink_message", "frm_payload"),
                Timestamp = DateTime.Now,
                GatewayId = TryGetString(document, "uplink_message", "rx_metadata", 0, "gateway_ids", "gateway_id"),
                GatewayEUI = TryGetString(document, "uplink_message", "rx_metadata", 0, "gateway_ids", "eui"),
                MetadataTime = TryGetDateTime(document, "uplink_message", "rx_metadata", 0, "time"),
                MetadataTimestamp = TryGetInt64(document, "uplink_message", "rx_metadata", 0, "timestamp"),
                RSSI = TryGetDouble(document, "uplink_message", "rx_metadata", 0, "rssi"),
                ChannelRSSI = TryGetDouble(document, "uplink_message", "rx_metadata", 0, "channel_rssi"),
                SNR = TryGetDouble(document, "uplink_message", "rx_metadata", 0, "snr"),
                Latitude = TryGetDouble(document, "uplink_message", "rx_metadata", 0, "location", "latitude"),
                Longitude = TryGetDouble(document, "uplink_message", "rx_metadata", 0, "location", "longitude"),
                Altitude = TryGetDouble(document, "uplink_message", "rx_metadata", 0, "location", "altitude")
            };

            if (deviceType.Equals("mkr", StringComparison.OrdinalIgnoreCase))
            {
                weatherData.Humidity = GetJsonValue(document, "uplink_message", "decoded_payload", "humidity");
                weatherData.Pressure = GetJsonValue(document, "uplink_message", "decoded_payload", "pressure");
                weatherData.Temperature = GetJsonValue(document, "uplink_message", "decoded_payload", "temperature");
                weatherData.Illumination = GetJsonValue(document, "uplink_message", "decoded_payload", "light");
            }
            else if (deviceType.Equals("lht", StringComparison.OrdinalIgnoreCase))
            {
                weatherData.Humidity = GetJsonValue(document, "uplink_message", "decoded_payload", "Hum_SHT");
                weatherData.Pressure = null; 
                weatherData.Temperature = GetJsonValue(document, "uplink_message", "decoded_payload", "TempC_SHT");
                weatherData.BatteryVoltage = GetJsonValue(document, "uplink_message", "decoded_payload", "BatV");
                weatherData.BatteryStatus = TryGetInt32(document, "uplink_message", "decoded_payload", "Bat_status");
                weatherData.WorkMode = TryGetString(document, "uplink_message", "decoded_payload", "Work_mode");
                weatherData.TempC_DS = GetJsonValue(document, "uplink_message", "decoded_payload", "TempC_DS");
                weatherData.Illumination = GetJsonValue(document, "uplink_message", "decoded_payload", "ILL_lx");
            }
            else
            {
                throw new Exception($"Unknown device type: {deviceType}");
            }

            if (string.IsNullOrEmpty(weatherData.GatewayEUI))
            {
                Console.WriteLine($"Warning: GatewayEUI is missing for device {weatherData.DeviceId}");
            }

            if (weatherData.Illumination == null)
            {
                Console.WriteLine($"Warning: Illumination is missing or not parsed for device {weatherData.DeviceId}");
            }

            return weatherData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing document for {deviceType} device: {ex.Message}");
            return null;
        }
    }

    public string TableIdentifier(JsonDocument document)
    {
        try
        {
            return TryGetString(document, "end_device_ids", "device_id") ?? string.Empty;
        }
        catch
        {   
            return string.Empty;
        }
    }

    private string TryGetString(JsonDocument document, params object[] keys)
    {
        try
        {
            var element = GetNestedProperty(document.RootElement, keys);
            return element.ValueKind != JsonValueKind.Null ? element.GetString() : null;
        }
        catch
        {
            return null;
        }
    }

    private int TryGetInt32(JsonDocument document, params object[] keys)
    {
        try
        {
            var element = GetNestedProperty(document.RootElement, keys);
            return element.GetInt32();
        }
        catch
        {
            return 0;
        }
    }

    private long TryGetInt64(JsonDocument document, params object[] keys)
    {
        try
        {
            var element = GetNestedProperty(document.RootElement, keys);
            return element.GetInt64();
        }
        catch
        {
            return 0L;
        }
    }

    private double? TryGetDouble(JsonDocument document, params object[] keys)
    {
        try
        {
            var element = GetNestedProperty(document.RootElement, keys);
            return element.ValueKind == JsonValueKind.Null ? null : element.GetDouble();
        }
        catch
        {
            return null;
        }
    }

    private DateTime TryGetDateTime(JsonDocument document, params object[] keys)
    {
        try
        {
            var element = GetNestedProperty(document.RootElement, keys);
            string dateString = element.GetString();

            DateTime date = DateTime.Parse(dateString);
            return date < new DateTime(1753, 1, 1) ? new DateTime(1753, 1, 1) : date;
        }
        catch
        {
            return new DateTime(1753, 1, 1);
        }
    }

    private JsonElement GetNestedProperty(JsonElement element, params object[] keys)
    {
        foreach (var key in keys)
        {
            if (key is int index && element.ValueKind == JsonValueKind.Array)
            {
                element = element[index];
            }
            else if (key is string propertyName && element.TryGetProperty(propertyName, out var tempElement))
            {
                element = tempElement;
            }
            else
            {
                throw new KeyNotFoundException($"Key '{key}' not found.");
            }
        }
        return element;
    }

    private double? GetJsonValue(JsonDocument jsonDocument, string parentProperty, string childProperty, string targetProperty)
    {
        try
        {
            if (jsonDocument.RootElement.TryGetProperty(parentProperty, out var parentElement) &&
                parentElement.TryGetProperty(childProperty, out var childElement) &&
                childElement.TryGetProperty(targetProperty, out var targetElement))
            {
                return targetElement.ValueKind == JsonValueKind.Null ? null : targetElement.GetDouble();
            }
        }
        catch
        {
        }
        return null;
    }
}
