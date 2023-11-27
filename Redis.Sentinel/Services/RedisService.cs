using StackExchange.Redis;

namespace Redis.Sentinel.Services;

public class RedisService
{
    // define sentinel servers
    static ConfigurationOptions sentinelOptions => new()
    {
        EndPoints =
        {
            { "localhost", 6383 },
            { "localhost", 6384 },
            { "localhost", 6385 }
        },
        CommandMap = CommandMap.Sentinel,
        AbortOnConnectFail = false
    };

    // master options created for reach master
    static ConfigurationOptions masterOptions => new()
    {
        AbortOnConnectFail = false
    };

    static public async Task<IDatabase> RedisMasterDatabase()
    {
        ConnectionMultiplexer sentinelConnection = await ConnectionMultiplexer.SentinelConnectAsync(sentinelOptions);

        System.Net.EndPoint masterEndPoint = null;
        foreach (System.Net.EndPoint endpoint in sentinelConnection.GetEndPoints())
        {
            IServer server = sentinelConnection.GetServer(endpoint);
            if (!server.IsConnected)
                continue;

            // when connection is found, master end point is searched by server name
            masterEndPoint = await server.SentinelGetMasterAddressByNameAsync("mymaster");
            break;
        }

        // master end point refer to container's ip addresses and ports -> we need to local ports 
        var localMasterIP = masterEndPoint.ToString() switch
        {
            "172.18.0.2:6379" => "localhost:6379",
            "172.18.0.3:6379" => "localhost:6380",
            "172.18.0.4:6379" => "localhost:6381",
            "172.18.0.5:6379" => "localhost:6382",
        };

        ConnectionMultiplexer masterConnection = await ConnectionMultiplexer.ConnectAsync(localMasterIP);
        IDatabase database = masterConnection.GetDatabase();
        return database;
    }
}