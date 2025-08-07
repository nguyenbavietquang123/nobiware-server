using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using System.Net;
[ApiController]
[Route("fhir/[controller]")]
public class TokenBlacklistController : ControllerBase
{
    private readonly IRedisBlacklistService _redisService;

    public TokenBlacklistController(IRedisBlacklistService redisService)
    {
        _redisService = redisService;
    }
    [Produces("application/json")]
    [HttpPost]
    public async Task<IActionResult> BlacklistToken([FromForm] string token)
    {
        Console.WriteLine("Go to TokenBlackList");
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Token is missing");
            var response = new { error = "Missing Token" };
            string json = JsonSerializer.Serialize(response);

            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }
        var result = JwtHelper.ExtractTidAndSid(token);
        if (result == null)
        {
            Console.WriteLine("Token is invalid");
            var response = new { error = "Missing Token" };
            string json = JsonSerializer.Serialize(response);

            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }
        await _redisService.BlacklistToken(result.Value.tid, result.Value.sid, result.Value.typ);
        Console.WriteLine("Token is blacklisted");
        var res = new { success = "Token is blacklisted" };
        string jsonRes = JsonSerializer.Serialize(res);

            return new ContentResult
            {
                Content = jsonRes,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK
            };
    }
}

public class TokenRequest
{
    public string Token { get; set; }
}
