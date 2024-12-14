using System;
using System.IO;
using System.Threading.Tasks;


namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {


            string topic = "v3/sensor-group8-25@ttn/devices/+/up";

            var client = new MqttClientWrapper();
            var priv_client= new MqttClientWrapper(topic);

            var pub = client.ConnectAndRunAsync();
            var priv = priv_client.ConnectAndRunAsync();

            await Task.WhenAll(pub, priv);
            
        }
    }
}
