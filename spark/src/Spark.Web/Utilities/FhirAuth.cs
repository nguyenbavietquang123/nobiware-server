using Hl7.Fhir.Model;
using Spark.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
namespace Spark.Web.Utilities;

public class FhirAuth
{
    
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
    public static string verifyAccessToken(string accessToken, IntrospectSettings settings)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri($"{settings.BaseUrl}/");
        var formData = new Dictionary<string, string>
    {
        { "client_id", settings.ClientId },
        { "client_secret", settings.ClientSecret },
        { "token", accessToken }
    };
        // Console.WriteLine("Client_ID", settings.ClientId);
        // Console.WriteLine("client_secret", settings.ClientSecret);
        // Console.WriteLine("IntrospectEndpoint", settings.IntrospectEndpoint);

        var content = new FormUrlEncodedContent(formData);
        var response = client.PostAsync(settings.IntrospectEndpoint, content).Result;
        if (response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync().Result;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var postResponse = System.Text.Json.JsonSerializer.Deserialize<IntrospectRespondData>(responseContent, options);
            ////Console.WriteLine("Post successful! ID: " + postResponse.active);
            return postResponse.active ? "" : getUnauthorizeJson();

        }
        else
        {
            return getUnauthenticateJson();
        }


    }
    public static string checkPermission(string accessToken, string permission)
    {
        TokenParser parser = new TokenParser(accessToken);
        if (parser.ClientScope.Contains(permission))
        {
            return "";
        }
        return getNotHavePermission();

        
    }
}