using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
namespace Spark.Web.Utilities;

public class TokenParser
{
    public string[] ClientScope;
    public string scopeLevel = "";

    public string listPatientId = "";
    public TokenParser(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (handler.CanReadToken(token))
        {
            var jwtToken = handler.ReadJwtToken(token);
            var scopeClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "scope");
            if (scopeClaim != null)
            {
                //Console.WriteLine("Scope: " + scopeClaim.Value);
                var scopes = scopeClaim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                ClientScope = scopes;
            }
            else
            {
                ClientScope = [];
            }
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");
            if (roleClaim != null) scopeLevel = roleClaim.Value;
            else scopeLevel = "system";
            var patientClaims = jwtToken.Claims
                .Where(c => c.Type == "patient")
                .Select(c => c.Value)
                .ToList();
            if (patientClaims != null) listPatientId = string.Join(",",patientClaims);
        }
        else
        {
            ClientScope = [];
        }
    }

}