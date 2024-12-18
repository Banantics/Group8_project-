using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Security.Authentication;
using vals;


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
        private IMqttClient _mqttClient;
        private MqttClientOptions _options;

        public MqttClientWrapper()
        {
            _broker = "eu1.cloud.thethings.network";
            _port = 8883;
            _clientId = Guid.NewGuid().ToString();
            _topic = "v3/project-software-engineering@ttn/devices/+/up";
            _username = "project-software-engineering@ttn";
            _password = "NNSXS.DTT4HTNBXEQDZ4QYU6SG73Q2OXCERCZ6574RVXI.CQE6IG6FYNJOO2MOFMXZVWZE4GXTCC2YXNQNFDLQL4APZMWU6ZGA";

            make_client();
        }

        public MqttClientWrapper(string top)
        {
            if (top == null || top != "v3/sensor-group8-25@ttn/devices/+/up") return;
            _broker = "eu1.cloud.thethings.network";
            _port = 8883;
            _clientId = Guid.NewGuid().ToString();
            _topic = top;
            _username = "sensor-group8-25@ttn";
            _password = "NNSXS.2LIHEIKN66OERLIUV5M2R3IM52NWOBNG3HBKX3A.ETIF35COU2IUGJYNHG4T3AGTVYR3GM5IWTNRX3NJGC564Y5BX3PA";

            make_client();
        }

        private void make_client() 
        {
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
                        var deviceid = getter.TableIdentifier(jsonDocument);
                        vals.values weatherdata = null;
                        if (deviceid.Contains("mkr", StringComparison.OrdinalIgnoreCase))
                        {
                            weatherdata = getter.table(jsonDocument , "mkr");
                            
                        }
                        else if (deviceid.Contains("lht", StringComparison.OrdinalIgnoreCase))
                        {
                            weatherdata = getter.table(jsonDocument , "lht");
                           
                        }else if (deviceid.Contains("group8-2425", StringComparison.OrdinalIgnoreCase))
                        {
                            weatherdata = getter.special_table(jsonDocument);
                        }
                        else
                        {
                            Console.WriteLine($"unknown id: {deviceid}.");
                        }
                       
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

                // disconnect when exiting 
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

