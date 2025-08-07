using System.Threading.Tasks;

public interface IRedisBlacklistService
{
    Task BlacklistToken(string tid, string sid, string typ);
    Task<bool> IsBlacklisted(string tokenId);
}
