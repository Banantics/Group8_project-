using System;
using System.Threading.Tasks;
             

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new MqttClientWrapper();
            await client.ConnectAndRunAsync();
            
        }
    }
}
