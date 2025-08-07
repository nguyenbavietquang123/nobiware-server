using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Spark.Web.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Spark.Web.Utilities;

public class FhirAuth
{


    // private static HttpClient _httpUserInfoClient = new()
    // {
    //     BaseAddress = new Uri("https://id-dev.nobiware.com")
    // };
     private readonly HttpClient _httpUserInfoClient;
    private readonly IRedisBlacklistService _redisService;
    private readonly HttpClient _httpClient = new();
    private static JsonWebKeySet _jwks;
    private readonly string _jwksUrl;
    private readonly string _validIssuer;
    public FhirAuth(IRedisBlacklistService redisService, HttpClient httpUserInfoClient, IOptions<FhirAuthOptions> options)
    {

        _jwksUrl = options.Value.JwksUrl;
        _httpUserInfoClient = httpUserInfoClient;
        _redisService = redisService;
        _validIssuer = options.Value.ValidIssuer;
    }
    public static string getUnauthorizeJson()
    {
        return @"{
                ""resourceType"": ""OperationOutcome"",
                ""issue"": [
                    {
                    ""severity"": ""error"",
                    ""diagnostics"": ""Unauthorize""
                    }
                ]
                }";
    }
    public static string getNotHavePermission()
    {
        return @"{
                ""resourceType"": ""OperationOutcome"",
                ""issue"": [
                    {
                    ""severity"": ""error"",
                    ""diagnostics"": ""You do not have permission to access this API""
                    }
                ]
                }";
    }
    public static string getNotHavePermissionToAccessResource()
    {
        return @"{
                ""resourceType"": ""OperationOutcome"",
                ""issue"": [
                    {
                    ""severity"": ""error"",
                    ""diagnostics"": ""You do not have permission to access this resource""
                    }
                ]
                }";
    }
    public static string getUnauthenticateJson()
    {
        return @"{
                ""resourceType"": ""OperationOutcome"",
                ""issue"": [
                    {
                    ""severity"": ""error"",
                    ""diagnostics"": ""Unauthenticate""
                    }
                ]
                }";
    }

    //Note: verifyAccessToken will be modified in the future to integrate with identity server.
    // public static async Task<string> verifyAccessToken(string accessToken, IntrospectSettings settings)
    // {
    //     var client = new HttpClient();
    //     client.BaseAddress = new Uri($"{settings.BaseUrl}/");
    //     var formData = new Dictionary<string, string>
    // {
    //     { "client_id", settings.ClientId },
    //     { "client_secret", settings.ClientSecret },
    //     { "token", accessToken }
    // };
    //     // Console.WriteLine("Client_ID", settings.ClientId);
    //     // Console.WriteLine("client_secret", settings.ClientSecret);
    //     // Console.WriteLine("IntrospectEndpoint", settings.IntrospectEndpoint);

    //     var content = new FormUrlEncodedContent(formData);
    //     var response = await client.PostAsync(settings.IntrospectEndpoint, content);
    //     if (response.IsSuccessStatusCode)
    //     {
    //         var responseContent = await response.Content.ReadAsStringAsync();

    //         var options = new JsonSerializerOptions
    //         {
    //             PropertyNameCaseInsensitive = true
    //         };

    //         var postResponse = System.Text.Json.JsonSerializer.Deserialize<IntrospectRespondData>(responseContent, options);
    //         ////Console.WriteLine("Post successful! ID: " + postResponse.active);
    //         return postResponse.active ? "" : getUnauthorizeJson();

    //     }
    //     else
    //     {
    //         return getUnauthenticateJson();
    //     }


    // }
    public async System.Threading.Tasks.Task<string> GetUserInfo(HttpClient httpClient, string bearerToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/realms/quang-fhir-server/protocol/openid-connect/userinfo");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        using var response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return "";
        }
        else
        {
            Console.WriteLine("[UserInfo] Failed");
            Console.WriteLine($"[UserInfo] Status code: {response.StatusCode}");
            return getUnauthorizeJson();
        }
    }
    public  async System.Threading.Tasks.Task<string> ValidateAccessToken(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwt;

        try
        {
            jwt = tokenHandler.ReadJwtToken(accessToken);
        }
        catch
        {
            Console.WriteLine("Invalid JWT format.");
            return getUnauthorizeJson();
        }
        if (_jwks == null)
        {
            var response = await _httpClient.GetStringAsync(_jwksUrl);
            Console.WriteLine("Create new JsonWebKey");
            _jwks = new JsonWebKeySet(response);

        }
        var validationParams = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuer = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = _validIssuer,
            ValidateAudience = true,
            ValidAudience = "account",
            RequireSignedTokens = true,
            IssuerSigningKeys = _jwks.Keys
        };
        try
        {
            tokenHandler.ValidateToken(accessToken, validationParams, out _);
            return ""; // Token is valid
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return getUnauthorizeJson();
        }


    }


    public async Task<string> verifyAccessToken(string accessToken, IntrospectSettings settings)
    {
        await ValidateAccessToken(accessToken);
        return await _redisService.IsBlacklisted(accessToken) ? getUnauthorizeJson() : "";
    }
    public string checkPermission(string accessToken, string permission)
    {
        TokenParser parser = new TokenParser(accessToken);
        if (parser.ClientScope.Contains(permission))
        {
            return "";
        }
        return getNotHavePermission();


    }
}