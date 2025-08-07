using StackExchange.Redis;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class RedisBlacklistService : IRedisBlacklistService
{
    private readonly IDatabase _db;

    public RedisBlacklistService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task BlacklistToken(string tid, string sid, string typ)
    {
        await _db.StringSetAsync($"blacklist:tid:{tid}", "1", TimeSpan.FromDays(1));
        if (!string.IsNullOrEmpty(sid) && typ == "Refresh")
        {
            await _db.StringSetAsync($"blacklist:sid:{sid}", "1", TimeSpan.FromDays(1));
        }
    }

    public async Task<bool> IsBlacklisted(string token)
    {
        var result = JwtHelper.ExtractTidAndSid(token);
        if (token == null)
        {
            Console.WriteLine("BlackListServices: No token");
            return true;
        }

        if (await _db.KeyExistsAsync($"blacklist:tid:{result.Value.tid}"))
        {   Console.WriteLine("BlackListServices: tid in BlackList");
            return true;
        }
        if (await _db.KeyExistsAsync($"blacklist:sid:{result.Value.sid}"))
        {
            Console.WriteLine("BlackListServices: sid in BlackList");
            return true;
        }
        Console.WriteLine("BlackListServices: Not in BlackList");
        return false;
    }
}
