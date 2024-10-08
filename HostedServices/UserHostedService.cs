using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UsersMS.Models;
using UsersMS.Options;
using MongoDB.Driver;

namespace UsersMS.HostedServices;

public class UserHostedService : IHostedService
{
    public static List<User> Users { get; set; }

    private readonly ConnectionFactory rabbitMqConnectionFactory;
    private readonly IConnection connection;
    private readonly IModel model;
    private const string QUEUE_NAME = "new_user";
    private readonly string connectionString;

    public UserHostedService(IOptions<RabbitMqOptions> optionsSnapshot, IConfiguration configuration)
    {
        this.rabbitMqConnectionFactory = new ConnectionFactory()
        {
            HostName = optionsSnapshot.Value.HostName,
            UserName = optionsSnapshot.Value.UserName,
            Password = optionsSnapshot.Value.Password
        };

        this.connection = this.rabbitMqConnectionFactory.CreateConnection();
        this.model = connection.CreateModel();
        this.connectionString = configuration.GetConnectionString("MongoDbUsers");
    }
    static UserHostedService() {
        var client = new MongoClient("mongodb://localhost:27017");

        var database = client.GetDatabase("UsersDb");

        var collection = database.GetCollection<User>("Users");

        var findAllQuery = collection.Find(user => true);

        Users = findAllQuery.ToList();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var result = this.model.QueueDeclare(
            queue: QUEUE_NAME,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        var consumer = new EventingBasicConsumer(this.model);

        consumer.Received += (sender, deliverEventArgs) =>
        {
            string? newUserJson = null;

            try {
                newUserJson = Encoding.ASCII.GetString(deliverEventArgs.Body.ToArray());

                var newUser = JsonSerializer.Deserialize<User>(newUserJson)!;

                var client = new MongoClient(connectionString);

                var database = client.GetDatabase("UsersDb");

                var collection = database.GetCollection<User>("Users");

                var findAllQuery = collection.Find(user => true);

                Users = findAllQuery.ToList();

                collection.InsertOneAsync(newUser).Wait();
            }
            catch(Exception ex) {
                System.Console.WriteLine($"Couldn't pull new user: '{ex}' | Body: {newUserJson}");
            }
        };

        this.model.BasicConsume(
            queue: QUEUE_NAME,
            autoAck: true,
            consumer: consumer
        );

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}