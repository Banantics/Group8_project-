    using System;

    namespace vals
    {
        public class values
        {
            // 'end_device'
            public string DeviceId { get; set; }
            public string Application { get; set; }
            public string DevEUI { get; set; }
            public string JoinEUI { get; set; }
            public string DevAddress { get; set; }

            // 'uplink_messages' 
            public int MessageId { get; set; } 
            public string SessionKeyId { get; set; }
            public int FPort { get; set; }
            public int FCnt { get; set; }
            public string FrmPayload { get; set; }
            public DateTime Timestamp { get; set; }
             //LORA
            public double? HumidityLora { get; set; }
            public double? PressureLora { get; set; }
            public double? TemperatureLora { get; set; }
            public double? LightLora { get; set; }
            
            public double? Humidity { get; set; }
            public double? Pressure { get; set; }
            public double? Temperature { get; set; }

           
            public double? BatteryVoltage { get; set; }
            public int? BatteryStatus { get; set; }
            public double? Illumination { get; set; }
            public string WorkMode { get; set; }
            public double? TempC_DS { get; set; } 
            public double? TempC_SHT { get; set; }  

           
            public double? Light { get; set; } 

           
            public int MetadataId { get; set; } 
            public string GatewayId { get; set; }
            public string GatewayEUI { get; set; }
            public DateTime MetadataTime { get; set; }
            public long MetadataTimestamp { get; set; }
            public double? RSSI { get; set; }
            public double? ChannelRSSI { get; set; }
            public double? SNR { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public double? Altitude { get; set; }
        }
    }
