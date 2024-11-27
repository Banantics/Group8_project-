

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using System.Text.Json;

namespace vals
{
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
}

