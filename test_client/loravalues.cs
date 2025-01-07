using System;

namespace values
{


    public class loravalues
    {
        public int MessageId { get; set; }  
        public double Illumination { get; set; }
        public double Pressure { get; set; }
        
        public double Humidity { get; set; }
        public double Temperature { get; set; }
        public double Rssi { get; set; }
        public DateTime Timestamp { get; set; }
    }
}