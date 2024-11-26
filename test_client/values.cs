

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
        public string AplicationId { get; set; }
        public DateTime Time { get; set; }
        public double Temperature { get; set; }
        public double AmbientLight { get; set; }
        public double Humidity { get; set; }

    }
}

