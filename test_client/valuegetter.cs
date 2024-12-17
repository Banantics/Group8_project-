using System;
using System.Reflection.Metadata;
using System.Text.Json;

public class valuegetter
{
    ///is empty cause im lazy
    private vals.values _weatherData = new vals.values();

    public vals.values table(JsonDocument document, string deviceType)
    {
        try
        {
            get_common_values(document);

            if (deviceType.Equals("mkr", StringComparison.OrdinalIgnoreCase))
            {
                _weatherData.Humidity = GetJsonValue(document, "uplink_message", "decoded_payload", "humidity");
                _weatherData.Pressure = GetJsonValue(document, "uplink_message", "decoded_payload", "pressure");
                _weatherData.Temperature = GetJsonValue(document, "uplink_message", "decoded_payload", "temperature");
                _weatherData.Illumination = GetJsonValue(document, "uplink_message", "decoded_payload", "light");
            }
            else if (deviceType.Equals("lht", StringComparison.OrdinalIgnoreCase))
            {
                _weatherData.Humidity = GetJsonValue(document, "uplink_message", "decoded_payload", "Hum_SHT");
                //_weatherData.Pressure = null;
                _weatherData.Temperature = GetJsonValue(document, "uplink_message", "decoded_payload", "TempC_SHT");
                _weatherData.BatteryVoltage = GetJsonValue(document, "uplink_message", "decoded_payload", "BatV");
                _weatherData.BatteryStatus = TryGetInt32(document, "uplink_message", "decoded_payload", "Bat_status");
                _weatherData.WorkMode = TryGetString(document, "uplink_message", "decoded_payload", "Work_mode");
                _weatherData.TempC_DS = GetJsonValue(document, "uplink_message", "decoded_payload", "TempC_DS");
                _weatherData.Illumination = GetJsonValue(document, "uplink_message", "decoded_payload", "ILL_lx");
            }
            else if (_weatherData.DeviceId =="group8-2425") 
            {
                deviceType = "mkr";
                _weatherData.Temperature = TryGetDouble(document, "uplink_message", "decoded_payload", "temp");
                _weatherData.Humidity = TryGetDouble(document, "uplink_message", "decoded_payload", "humid");
                _weatherData.Illumination = TryGetDouble(document, "uplink_message", "decoded_payload", "light");
                _weatherData.Pressure = TryGetDouble(document, "uplink_message", "decoded_payload", "press");
            }
            else
            {
                throw new Exception($"Unknown device type: {deviceType}");
            }

            if (string.IsNullOrEmpty(_weatherData.GatewayEUI))
            {
                Console.WriteLine($"Warning: GatewayEUI is missing for device {_weatherData.DeviceId}");
                _weatherData.GatewayEUI = "n/a";
            }

            if (_weatherData.Illumination == null)
            {
                Console.WriteLine($"Warning: Illumination is missing or not parsed for device {_weatherData.DeviceId}");
            }


            timestamp_correct();
            
            return  _weatherData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing document for {deviceType} device: {ex.Message}");
            return null;
        }
    }


    public void get_common_values(JsonDocument document)
    {
        _weatherData = new vals.values
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
            Timestamp = TryGetDateTime(document, "uplink_message", "settings", "time"),
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
    }


    private void timestamp_correct() 
    {
       if ( _weatherData.MetadataTimestamp is 0) 
        {
            DateTime unixTime= new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            _weatherData.MetadataTimestamp = (long)(_weatherData.MetadataTime.ToUniversalTime() - unixTime).TotalSeconds;
        }
       return;
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
