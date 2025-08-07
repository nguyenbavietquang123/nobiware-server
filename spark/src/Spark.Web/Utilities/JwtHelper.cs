using System.IdentityModel.Tokens.Jwt;
using System.Linq;

public static class JwtHelper
{
    public static (string tid, string sid, string typ)? ExtractTidAndSid(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token)) return null;

        var jwt = handler.ReadJwtToken(token);
        var tid = jwt.Claims.FirstOrDefault(c => c.Type == "jti")?.Value ?? jwt.Id;
        var sid = jwt.Claims.FirstOrDefault(c => c.Type == "sid")?.Value;
        var typ = jwt.Claims.FirstOrDefault(c => c.Type == "typ")?.Value;
        return string.IsNullOrEmpty(tid) ? null : (tid, sid, typ);
    }
}
