using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json; // For JSON deserialization
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Collections.Generic;
using System;

class Program
{
    static async Task Main(string[] args)
    {
        string broker = "eu1.cloud.thethings.network";
        int port = 8883;
        string clientId = Guid.NewGuid().ToString();
        string topic = "v3/project-software-engineering@ttn/devices/+/up"; // Adjust topic for your TTN setup
        string username = "project-software-engineering@ttn";
        string password = "NNSXS.DTT4HTNBXEQDZ4QYU6SG73Q2OXCERCZ6574RVXI.CQE6IG6FYNJOO2MOFMXZVWZE4GXTCC2YXNQNFDLQL4APZMWU6ZGA";

        // Create a MQTT client factory
        var factory = new MqttFactory();

        // Create a MQTT client instance
        var mqttClient = factory.CreateMqttClient();

        // Create MQTT client options
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(broker, port) // MQTT broker address and port
            .WithCredentials(username, password) // Set username and password
            .WithClientId(clientId)
            .WithCleanSession()
            .WithTls(
                o =>
                {
                    // Accept all certificates for simplicity (not recommended for production)
                    o.CertificateValidationHandler = _ => true;
                    o.SslProtocol = SslProtocols.Tls12;
                }
            )
            .Build();

        // Connect to MQTT broker
        var connectResult = await mqttClient.ConnectAsync(options);

        if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
        {
            Console.WriteLine("Connected to MQTT broker successfully.");

            // Subscribe to the topic
            await mqttClient.SubscribeAsync(topic);
            Console.WriteLine($"Subscribed to topic: {topic}");

            // Handle received messages
            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                try
                {
                    // Convert payload to string
                    string payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

                    Console.WriteLine($"Received raw message: {payload}\n");

                    // Parse JSON message
                    var jsonDocument = JsonDocument.Parse(payload);

                    // Extract specific data from the JSON (example: extract "message" field)
                    if (jsonDocument.RootElement.TryGetProperty("message", out JsonElement messageElement))
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
                await Task.CompletedTask;
            };

            // Prevent application from exiting immediately
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            // Unsubscribe and disconnect
            await mqttClient.UnsubscribeAsync(topic);
            await mqttClient.DisconnectAsync();
        }
        else
        {
            Console.WriteLine($"Failed to connect to MQTT broker: {connectResult.ResultCode}");
        }
    }
}