using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Spark.Engine.Extensions;
using Spark.Web.Models;
using Spark.Web.Utilities;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

public class AuthorizeFhirAttribute : Attribute, IAsyncAuthorizationFilter
{
    private string _permissionPrefix;
    static public string[] resourceRelatedToPatient = [
  "AllergyIntolerance",
  "CarePlan",
  "CareTeam",
  "Coverage",
  "Condition",
  "Device",
  "DiagnosticReport",
  "DocumentReference",
  "Encounter",
  "Goal",
  "Immunization",
  "Media",
  "MedicationDispense",
  "MedicationRequest",
  "Observation",
  "Procedure",
  "RelatedPerson",
  "ServiceRequest"
];
    static public string[] resourceNotRelatedToPatient = [
    "Location",
    "Medication",
    "Organization",
    "Practitioner",
    "PractitionerRole",
    "Provenance",
    "QuestionnaireResponse",
    "Specimen"
    ];
    public AuthorizeFhirAttribute(string permissionPrefix)
    {
        _permissionPrefix = permissionPrefix;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;
        string resourceType = context.RouteData.Values["type"]?.ToString();
        //Console.WriteLine("start authorization custom attribnute");
        if (string.IsNullOrEmpty(resourceType))
        {
            if (_permissionPrefix == "Bundle")
            {
                //Console.WriteLine("custom attribnute line 60");
                resourceType = "Bundle";
            }
            else
            {
                //Console.WriteLine("custom attribnute line 66");
                context.Result = new BadRequestObjectResult("Missing 'type' route parameter.");
                return;
            }

        }
        ////Console.WriteLine("custom attribnute line 70");
        
        var introspectSettings = httpContext.RequestServices.GetService<IOptions<IntrospectSettings>>()?.Value;
        //Console.WriteLine("Go to OnAuthorization");
        var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            //Console.WriteLine("missing accesstooken");
            context.Result = new ObjectResult(FhirFileImport.ImportData(FhirAuth.getUnauthorizeJson()).First()) { StatusCode = (int)HttpStatusCode.Unauthorized };
            return;
        }

        var token = authHeader.Split(' ').Last();
        // Console.WriteLine("Go To verifyAccessToken");
        string verifyError = FhirAuth.verifyAccessToken(token, introspectSettings);
        if (!string.IsNullOrEmpty(verifyError))
        {
            var outcome = FhirFileImport.ImportData(verifyError).First();
            context.Result = new ObjectResult(outcome) { StatusCode = (int)HttpStatusCode.Unauthorized };
            return;
        }
        TokenParser parser = new TokenParser(token);
        string requiredPermission = resourceType == "Bundle" ? $"{parser.scopeLevel}/{resourceType}.c"  : $"{parser.scopeLevel}/{resourceType}.{_permissionPrefix}";
        // Console.WriteLine("requiredPermission" + requiredPermission);

        string permissionError = FhirAuth.checkPermission(token, requiredPermission);
        if (!string.IsNullOrEmpty(permissionError))
        {
            var outcome = FhirFileImport.ImportData(permissionError).First();
            context.Result = new ObjectResult(outcome) { StatusCode = (int)HttpStatusCode.Unauthorized };
            return;
        }
        string resource_id = context.RouteData.Values["id"]?.ToString();
        if (resource_id == null)
        {
            //Console.WriteLine("Khong co ID");
        }
       
        if (resource_id != null)
        {
            var searchParams = httpContext.Request.GetSearchParams();
            if (parser.listPatientId != "")
            {
                if (_permissionPrefix == "c")
                {
                    if (parser.scopeLevel != "system")
                    {
                        string error = FhirAuth.getNotHavePermission();
                        var outcome = FhirFileImport.ImportData(error).First();
                        context.Result = new ObjectResult(outcome) { StatusCode = (int)HttpStatusCode.Unauthorized };
                    }
                }
                if (resourceType == "Patient")
                {
                    if (!parser.listPatientId.Contains(resource_id))
                    {
                        string error = FhirAuth.getNotHavePermissionToAccessResource();
                        var outcome = FhirFileImport.ImportData(error).First();
                        context.Result = new ObjectResult(outcome) { StatusCode = (int)HttpStatusCode.Unauthorized };
                        return;
                    }
                }
                else if (resourceRelatedToPatient.Contains(resourceType))
                {
                    searchParams.Add("_id", resource_id);
                    searchParams.Add("patient", parser.listPatientId);
                    searchParams.Add("_summary", "count");
                    context.HttpContext.Items["SearchParams"] = searchParams;
                }
                else
                {
                    if (parser.scopeLevel != "system")
                    {
                        string error = FhirAuth.getNotHavePermissionToAccessResource();
                        var outcome = FhirFileImport.ImportData(error).First();
                        context.Result = new ObjectResult(outcome) { StatusCode = (int)HttpStatusCode.Unauthorized };
                        return;
                    }
                }
            
        }
        }
        else
        {
            if (_permissionPrefix == "c" || _permissionPrefix == "Bundle")
            {
                if (parser.scopeLevel != "system")
                {
                    string error = FhirAuth.getNotHavePermission();
                    var outcome = FhirFileImport.ImportData(error).First();
                    context.Result = new ObjectResult(outcome) { StatusCode = (int)HttpStatusCode.Unauthorized };
                    return;
                }
            }
            else
            {
                //Console.WriteLine("Line 154");
            var searchParams = httpContext.Request.GetSearchParams();
            var bodySearchParams = httpContext.Request.GetSearchParamsFromBody();
            if (parser.listPatientId != "")
            {

                if (resourceType == "Patient")
                {
                    //Console.WriteLine("Line 162");
                    searchParams.Add("_id", parser.listPatientId);
                    bodySearchParams.Add("_id", parser.listPatientId);
                }

                else if (resourceRelatedToPatient.Contains(resourceType))
                {
                    searchParams.Add("patient", parser.listPatientId);
                    bodySearchParams.Add("patient", parser.listPatientId);
                }
                else
                {
                    if (parser.scopeLevel != "system")
                    {
                        string error = FhirAuth.getNotHavePermissionToAccessResource();
                        var outcome = FhirFileImport.ImportData(error).First();
                        context.Result = new ObjectResult(outcome) { StatusCode = (int)HttpStatusCode.Unauthorized };
                        return;
                    }
                }
                //Console.WriteLine("Line 182");
                context.HttpContext.Items["SearchParams"] = searchParams;
                context.HttpContext.Items["BodySearchParams"] = bodySearchParams;
                //Console.WriteLine("Line 185");
            }
            }
            
        }
        await Task.CompletedTask;
    }

    // Task IAsyncAuthorizationFilter.OnAuthorizationAsync(AuthorizationFilterContext context) => throw new NotImplementedException();
}
