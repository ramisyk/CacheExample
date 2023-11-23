using StackExchange.Redis;

ConnectionMultiplexer redisConnection = await ConnectionMultiplexer.ConnectAsync("localhost:1453");

ISubscriber subscriber = redisConnection.GetSubscriber();

await subscriber.SubscribeAsync("myChannel", (channel, message) =>
{
    Console.WriteLine(message);
});

Console.Read();