using StackExchange.Redis;

// ssl host, redis password vb are included to function as parameter like option => {} if they need...
ConnectionMultiplexer redisConnection = await ConnectionMultiplexer.ConnectAsync("localhost:1453");

ISubscriber subscriber = redisConnection.GetSubscriber();

while (true)
{
    Console.Write("Message: ");
    string message = Console.ReadLine();

    subscriber.PublishAsync("myChannel", message);
}