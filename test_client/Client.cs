using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Security.Authentication;


namespace Client
{
    public class MqttClientWrapper
    {
        private readonly string _broker;
        private readonly int _port;
        private readonly string _clientId;
        private readonly string _topic;
        private readonly string _username;
        private readonly string _password;
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _options;

        public MqttClientWrapper()
        {
            _broker = "eu1.cloud.thethings.network";
            _port = 8883;
            _clientId = Guid.NewGuid().ToString();
            _topic = "v3/project-software-engineering@ttn/devices/+/up";
            _username = "project-software-engineering@ttn";
            _password = "NNSXS.DTT4HTNBXEQDZ4QYU6SG73Q2OXCERCZ6574RVXI.CQE6IG6FYNJOO2MOFMXZVWZE4GXTCC2YXNQNFDLQL4APZMWU6ZGA";

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            _options = new MqttClientOptionsBuilder()
                .WithTcpServer(_broker, _port)
                .WithCredentials(_username, _password)
                .WithClientId(_clientId)
                .WithCleanSession()
                .WithTls(
                    o =>
                    {
                        o.CertificateValidationHandler = _ => true;
                        o.SslProtocol = SslProtocols.Tls12;
                    })
                .Build();
        }

        public async Task ConnectAndRunAsync()
        {
            // Connect to the MQTT broker
            var connectResult = await _mqttClient.ConnectAsync(_options);

            if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                Console.WriteLine("Connected to MQTT broker successfully.");

                
                await _mqttClient.SubscribeAsync(_topic);
                Console.WriteLine($"Subscribed to topic: {_topic}");

                
                _mqttClient.ApplicationMessageReceivedAsync += async e =>
                {
                     Console.WriteLine("Message received from broker!");
                    try
                    {
                        
                        
                        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                        Console.WriteLine($"Received raw message: {payload}");

                       
                        var jsonDocument = JsonDocument.Parse(payload);
                        var getter = new valuegetter();
                        var weatherdata = getter.SomeTable(jsonDocument);
                        var deviceid = getter.tableidentifier(jsonDocument);
                        Console.WriteLine(deviceid);
                        if (weatherdata != null)
                        {
                           
                            var helper = new databaseHelper();
                            helper.insert(weatherdata);
                        }
                        else
                        {
                            Console.WriteLine("failed to parse.");
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                    }

                    await Task.CompletedTask; 
                };

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();

                // Unsubscribe and disconnect when the app exits
                await _mqttClient.UnsubscribeAsync(_topic);
                await _mqttClient.DisconnectAsync();
            }
            else
            {
                Console.WriteLine($"Failed to connect to MQTT broker: {connectResult.ResultCode}");
            }
        }
    }
}

