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
            _topic = "v3/project-software-engineering@ttn/devices";
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
                        o.CertificateValidationHandler = _ => true; // For simplicity, accept all certificates
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

                // Subscribe to the topic
                await _mqttClient.SubscribeAsync(_topic, MqttQualityOfServiceLevel.AtMostOnce);
                Console.WriteLine($"Subscribed to topic: {_topic}");

                // Handle incoming messages
                _mqttClient.ApplicationMessageReceivedAsync += async e =>
                {
                    try
                    {
                        // Convert the payload to string
                        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                        Console.WriteLine($"Received raw message: {payload}");

                        // Parse JSON message
                        var jsonDocument = JsonDocument.Parse(payload);

                        // Extract specific data from the JSON (example: "message" field)
                        if (jsonDocument.RootElement.TryGetProperty("message", out var messageElement))
                        {
                            string extractedMessage = messageElement.GetString();
                            Console.WriteLine($"Extracted message: {extractedMessage}");
                        }
                        else
                        {
                            Console.WriteLine("Message does not contain a 'message' field.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                    }

                    await Task.CompletedTask; // Required by the event handler
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
