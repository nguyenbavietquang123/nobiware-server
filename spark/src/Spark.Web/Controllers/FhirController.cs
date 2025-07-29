/*
 * Copyright (c) 2014-2018, Firely <info@fire.ly>
 * Copyright (c) 2019-2025, Incendi <info@incendi.no>
 *
 * SPDX-License-Identifier: BSD-3-Clause
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Spark.Engine;
using Spark.Engine.Core;
using Spark.Engine.Extensions;
using Spark.Engine.Service;
using Spark.Web.Models;
using Spark.Web.Utilities;

namespace Spark.Web.Controllers;

[Route("fhir"), ApiController, EnableCors]
//[Authorize]
public class FhirController : ControllerBase
{
    private readonly IFhirService _fhirService;
    private readonly SparkSettings _settings;
    private readonly IntrospectSettings _introspectSettings;
    private readonly ILogger<FhirController> _logger;
    private readonly IConfiguration _configuration;

    public FhirController(IFhirService fhirService, SparkSettings settings, IOptions<IntrospectSettings> introspectSettings, ILogger<FhirController> logger, IConfiguration configuration)
    {
        _fhirService = fhirService ?? throw new ArgumentNullException(nameof(fhirService));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _introspectSettings = introspectSettings.Value;
        _logger = logger;
        _configuration = configuration;
    }
    private async Task<FhirResponse?> CheckSearchPermissionAsync(string type)
    {
        var offset = Request.GetPagingOffsetParameter();
        var searchparams = HttpContext.Items["SearchParams"] as SearchParams;

        if (searchparams != null)
        {
            var fhirResponse = await _fhirService.SearchAsync(type, searchparams, offset).ConfigureAwait(false);
            if (fhirResponse.Resource is Bundle bundle && bundle.Total <= 0)
            {
                string error = FhirAuth.getNotHavePermissionToAccessResource();
                Resource operationOutcome = FhirFileImport.ImportData(error).First();
                return new FhirResponse(HttpStatusCode.Unauthorized, operationOutcome);
            }
        }
        else
        {
            //Console.WriteLine("Search Param is Null");
        }

        return null; // means permission is OK
    }
    //[Authorize]
    [HttpGet("{type}/{id}")]
    [AuthorizeFhir("rs")]
    public async Task<ActionResult<FhirResponse>> Read(string type, string id)
    {
        // string bearerToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        // string authorizeError = FhirAuth.verifyAccessToken(bearerToken, _introspectSettings);
        // if (authorizeError != "")
        // {
        //     Resource errorOperationOutcome = FhirFileImport.ImportData(authorizeError).First();
        //     return new ActionResult<FhirResponse>(new FhirResponse(HttpStatusCode.Unauthorized, errorOperationOutcome));
        // }
        // authorizeError = FhirAuth.checkPermission(bearerToken, "fhir-resource-read");
        // if (authorizeError != "")
        // {
        //     Resource errorOperationOutcome = FhirFileImport.ImportData(authorizeError).First();
        //     return new FhirResponse(HttpStatusCode.Unauthorized, errorOperationOutcome);
        // }


        // var offset = Request.GetPagingOffsetParameter();
        // var searchparams = HttpContext.Items["SearchParams"] as SearchParams;
        // if (searchparams != null)
        // {
        //     var fhirRespond = await _fhirService.SearchAsync(type, searchparams, offset).ConfigureAwait(false);
        //     if (fhirRespond.Resource is Bundle bundle)
        //     {
        //         if (bundle.Total <= 0)
        //         {
        //             string error = FhirAuth.getNotHavePermissionToAccessResource();
        //             Resource operationOutcome = FhirFileImport.ImportData(error).First();
        //             return new FhirResponse(HttpStatusCode.Unauthorized, operationOutcome);
        //         }
        //     }
        // }
        // else
        // {
        //     //Console.WriteLine("Search Param is Null");
        // }

        var permissionResult = await CheckSearchPermissionAsync(type);
        if (permissionResult != null) return permissionResult;
        ConditionalHeaderParameters parameters = new ConditionalHeaderParameters(Request);
        //Console.WriteLine("GET type/id");
        //Console.WriteLine(parameters.ToJson());
        Key key = Key.Create(type, id);
        var response = await _fhirService.ReadAsync(key, parameters).ConfigureAwait(false);
        //var json = new FhirJsonSerializer().SerializeToString(response.Resource);
        ////Console.WriteLine(json);;
        return new ActionResult<FhirResponse>(response);
    }

    [HttpGet("{type}/{id}/_history/{vid}")]
    [AuthorizeFhir("rs")]
    public async Task<FhirResponse> VRead(string type, string id, string vid)
    {
        // var offset = Request.GetPagingOffsetParameter();
        // var searchparams = HttpContext.Items["SearchParams"] as SearchParams;
        // if (searchparams != null)
        // {
        //     var fhirRespond = await _fhirService.SearchAsync(type, searchparams, offset).ConfigureAwait(false);
        //     if (fhirRespond.Resource is Bundle bundle)
        //     {
        //         if (bundle.Total <= 0)
        //         {
        //             string error = FhirAuth.getNotHavePermissionToAccessResource();
        //             Resource operationOutcome = FhirFileImport.ImportData(error).First();
        //             return new FhirResponse(HttpStatusCode.Unauthorized, operationOutcome);
        //         }
        //     }
        // }
        // else
        // {
        //     //Console.WriteLine("Search Param is Null");
        // }
        var permissionResult = await CheckSearchPermissionAsync(type);
        if (permissionResult != null) return permissionResult;
        Key key = Key.Create(type, id, vid);
        return await _fhirService.VersionReadAsync(key).ConfigureAwait(false);
    }

    [HttpPut("{type}/{id?}")]
    [AuthorizeFhir("u")]
    public async Task<ActionResult<FhirResponse>> Update(string type, Resource resource, string id = null)
    {
        // var offset = Request.GetPagingOffsetParameter();
        // var searchparams = HttpContext.Items["SearchParams"] as SearchParams;
        // if (searchparams != null)
        // {
        //     var fhirRespond = await _fhirService.SearchAsync(type, searchparams, offset).ConfigureAwait(false);
        //     if (fhirRespond.Resource is Bundle bundle)
        //     {
        //         if (bundle.Total <= 0)
        //         {
        //             string error = FhirAuth.getNotHavePermissionToAccessResource();
        //             Resource operationOutcome = FhirFileImport.ImportData(error).First();
        //             return new FhirResponse(HttpStatusCode.Unauthorized, operationOutcome);
        //         }
        //     }
        // }
        // else
        // {
        //     //Console.WriteLine("Search Param is Null");
        // }
        var permissionResult = await CheckSearchPermissionAsync(type);
        if (permissionResult != null) return permissionResult;
        string versionId = Request.GetTypedHeaders().IfMatch?.FirstOrDefault()?.Tag.Buffer;
        Key key = Key.Create(type, id, versionId);
        if (key.HasResourceId())
        {
            Request.TransferResourceIdIfRawBinary(resource, id);

            return new ActionResult<FhirResponse>(await _fhirService.UpdateAsync(key, resource).ConfigureAwait(false));
        }
        else
        {
            return new ActionResult<FhirResponse>(await _fhirService.ConditionalUpdateAsync(key, resource,
                SearchParams.FromUriParamList(Request.TupledParameters())).ConfigureAwait(false));
        }
    }
    [Authorize]
    [HttpPost("{type}")]
    [AuthorizeFhir("c")]
    public async Task<FhirResponse> Create(string type, Resource resource)
    {
        //Console.WriteLine(resource.Id);
        Key key = null;
        if (resource.Id != null)
        {
            key = Key.Create(type, resource.Id);
        }
        else
        {
            key = Key.Create(type);
        }
        if (Request.Headers.ContainsKey(FhirHttpHeaders.IfNoneExist))
        {
            //Console.WriteLine("abnnnn");
            NameValueCollection searchQueryString = HttpUtility.ParseQueryString(Request.GetTypedHeaders().IfNoneExist());
            IEnumerable<Tuple<string, string>> searchValues =
                searchQueryString.Keys.Cast<string>()
                    .Select(k => new Tuple<string, string>(k, searchQueryString[k]));

            return await _fhirService.ConditionalCreateAsync(key, resource, SearchParams.FromUriParamList(searchValues)).ConfigureAwait(false);
        }

        return await _fhirService.CreateAsync(key, resource).ConfigureAwait(false);
    }

    [HttpPatch("{type}/{id}")]
    [AuthorizeFhir("u")]
    public async Task<FhirResponse> Patch(string type, string id, Parameters patch)
    {
        // var offset = Request.GetPagingOffsetParameter();
        // var searchparams = HttpContext.Items["SearchParams"] as SearchParams;
        // if (searchparams != null)
        // {
        //     var fhirRespond = await _fhirService.SearchAsync(type, searchparams, offset).ConfigureAwait(false);
        //     if (fhirRespond.Resource is Bundle bundle)
        //     {
        //         if (bundle.Total <= 0)
        //         {
        //             string error = FhirAuth.getNotHavePermissionToAccessResource();
        //             Resource operationOutcome = FhirFileImport.ImportData(error).First();
        //             return new FhirResponse(HttpStatusCode.Unauthorized, operationOutcome);
        //         }
        //     }
        // }
        // else
        // {
        //     //Console.WriteLine("Search Param is Null");
        // }
        var permissionResult = await CheckSearchPermissionAsync(type);
        if (permissionResult != null) return permissionResult;
        // TODO: conditional PATCH support (http://www.hl7.org/fhir/R4/http.html#concurrency)
        var key = Key.Create(type, id, Request.IfMatchVersionId());
        return await _fhirService.PatchAsync(key, patch).ConfigureAwait(false);
    }

    [HttpDelete("{type}/{id}")]
    public async Task<FhirResponse> Delete(string type, string id)
    {
        Key key = Key.Create(type, id);
        FhirResponse response = await _fhirService.DeleteAsync(key).ConfigureAwait(false);
        return response;
    }

    [HttpDelete("{type}")]
    public async Task<FhirResponse> ConditionalDelete(string type)
    {
        Key key = Key.Create(type);
        return await _fhirService.ConditionalDeleteAsync(key, Request.TupledParameters()).ConfigureAwait(false);
    }

    [HttpGet("{type}/{id}/_history")]
    [AuthorizeFhir("rs")]
    public async Task<FhirResponse> History(string type, string id)
    {
        // var offset = Request.GetPagingOffsetParameter();
        // var searchparams = HttpContext.Items["SearchParams"] as SearchParams;
        // if (searchparams != null)
        // {
        //     var fhirRespond = await _fhirService.SearchAsync(type, searchparams, offset).ConfigureAwait(false);
        //     if (fhirRespond.Resource is Bundle bundle)
        //     {
        //         if (bundle.Total <= 0)
        //         {
        //             string error = FhirAuth.getNotHavePermissionToAccessResource();
        //             Resource operationOutcome = FhirFileImport.ImportData(error).First();
        //             return new FhirResponse(HttpStatusCode.Unauthorized, operationOutcome);
        //         }
        //     }
        // }
        // else
        // {
        //     //Console.WriteLine("Search Param is Null");
        // }
        var permissionResult = await CheckSearchPermissionAsync(type);
        if (permissionResult != null) return permissionResult;
        Key key = Key.Create(type, id);
        var parameters = new HistoryParameters(Request);
        return await _fhirService.HistoryAsync(key, parameters).ConfigureAwait(false);
    }

    // ============= Validate

    [HttpPost("{type}/{id}/$validate")]
    public async Task<FhirResponse> Validate(string type, string id, Resource resource)
    {
        Key key = Key.Create(type, id);
        return await _fhirService.ValidateOperationAsync(key, resource).ConfigureAwait(false);
    }

    [HttpPost("{type}/$validate")]
    public async Task<FhirResponse> Validate(string type, Resource resource)
    {
        Key key = Key.Create(type);
        return await _fhirService.ValidateOperationAsync(key, resource).ConfigureAwait(false);
    }

    // ============= Type Level Interactions
    [Authorize]
    [HttpGet("{type}")]
    [AuthorizeFhir("rs")]
    public async Task<FhirResponse> Search(string type)
    {
       
        string bearerToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
       

        var offset = Request.GetPagingOffsetParameter();
        
        var searchparams = HttpContext.Items["SearchParams"] as SearchParams ?? Request.GetSearchParams();


        return await _fhirService.SearchAsync(type, searchparams, offset).ConfigureAwait(false);
    }

    [HttpPost("{type}/_search")]
    [AuthorizeFhir("rs")]
    public async Task<FhirResponse> SearchWithOperator(string type)
    {

        var offset = Request.GetPagingOffsetParameter();
        SearchParams searchparams = HttpContext.Items["BodySearchParams"] as SearchParams ?? Request.GetSearchParamsFromBody();

        return await _fhirService.SearchAsync(type, searchparams, offset).ConfigureAwait(false);
    }

    [HttpGet("{type}/_history")]
    public async Task<FhirResponse> History(string type)
    {
        var parameters = new HistoryParameters(Request);
        return await _fhirService.HistoryAsync(type, parameters).ConfigureAwait(false);
    }

    // ============= Whole System Interactions

    [HttpGet, Route("metadata")]
    public async Task<FhirResponse> Metadata()
    {
        string json = System.IO.File.ReadAllText("Controllers/metadata.json");
        var jsonObject = JsonSerializer.Deserialize<object>(json);
        string compactJson = JsonSerializer.Serialize(jsonObject);
        Resource metadata = FhirFileImport.ImportData(compactJson).First();
        return new FhirResponse(HttpStatusCode.OK, metadata);
    }
    [HttpGet, Route(".well-known/smart-configuration")]
    public async Task<IActionResult> Smart()
    {
        string json = System.IO.File.ReadAllText("Controllers/smart.json");
        return new ContentResult
        {
            Content = json,
            ContentType = "application/json",
            StatusCode = (int)HttpStatusCode.OK
        };

    }
    [HttpOptions, Route("")]
    public async Task<FhirResponse> Options()
    {
        return await _fhirService.CapabilityStatementAsync(_settings.Version).ConfigureAwait(false);
    }

    [Authorize]
    [HttpPost, Route("")]
    [AuthorizeFhir("Bundle")]
    public async Task<FhirResponse> Transaction(Bundle bundle)
    {
        //Console.WriteLine("In FhirController line 386");
        return await _fhirService.TransactionAsync(bundle).ConfigureAwait(false);
    }

    [HttpGet, Route("_history")]
    public async Task<FhirResponse> History()
    {
        var parameters = new HistoryParameters(Request);
        return await _fhirService.HistoryAsync(parameters).ConfigureAwait(false);
    }

    [HttpGet, Route("_snapshot")]
    public async Task<FhirResponse> Snapshot()
    {
        string snapshot = Request.GetParameter(FhirParameter.SNAPSHOT_ID);
        var offset = Request.GetPagingOffsetParameter();
        return await _fhirService.GetPageAsync(snapshot, offset).ConfigureAwait(false);
    }

    // Operations

    [HttpPost, Route("${operation}")]
    public FhirResponse ServerOperation(string operation)
    {
        switch (operation.ToLower())
        {
            case "error": throw new Exception("This error is for testing purposes");
            default: return Respond.WithError(HttpStatusCode.NotFound, "Unknown operation");
        }
    }

    [HttpPost, Route("{type}/{id}/${operation}")]
    public async Task<FhirResponse> InstanceOperation(string type, string id, string operation, Parameters parameters)
    {
        Key key = Key.Create(type, id);
        switch (operation.ToLower())
        {
            case "meta": return await _fhirService.ReadMetaAsync(key).ConfigureAwait(false);
            case "meta-add": return await _fhirService.AddMetaAsync(key, parameters).ConfigureAwait(false);
            case "meta-delete":

            default: return Respond.WithError(HttpStatusCode.NotFound, "Unknown operation");
        }
    }

    [HttpPost, HttpGet, Route("{type}/{id}/$everything")]
    [AuthorizeFhir("rs")]
    public async Task<FhirResponse> Everything(string type, string id = null)
    {
        // var offset = Request.GetPagingOffsetParameter();
        // var searchparams = HttpContext.Items["SearchParams"] as SearchParams;
        // if (searchparams != null)
        // {
        //     var fhirRespond = await _fhirService.SearchAsync(type, searchparams, offset).ConfigureAwait(false);
        //     if (fhirRespond.Resource is Bundle bundle)
        //     {
        //         if (bundle.Total <= 0)
        //         {
        //             string error = FhirAuth.getNotHavePermissionToAccessResource();
        //             Resource operationOutcome = FhirFileImport.ImportData(error).First();
        //             return new FhirResponse(HttpStatusCode.Unauthorized, operationOutcome);
        //         }
        //     }
        // }
        // else
        // {
        //     //Console.WriteLine("Search Param is Null");
        // }
        var permissionResult = await CheckSearchPermissionAsync(type);
        if (permissionResult != null) return permissionResult;
        Key key = Key.Create(type, id);
        return await _fhirService.EverythingAsync(key).ConfigureAwait(false);
    }

    [HttpPost, HttpGet, Route("{type}/$everything")]
    [AuthorizeFhir("rs")]
    public async Task<FhirResponse> Everything(string type)
    {
        Key key = Key.Create(type);
        return await _fhirService.EverythingAsync(key).ConfigureAwait(false);
    }

    [HttpPost, HttpGet, Route("Composition/{id}/$document")]
    public async Task<FhirResponse> Document(string id)
    {
        Key key = Key.Create("Composition", id);
        return await _fhirService.DocumentAsync(key).ConfigureAwait(false);
    }
    [HttpPost, Route("proxy/token")]
    [Consumes("application/x-www-form-urlencoded")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> ProxyToken([FromForm] Dictionary<string, string> formData)
    {
        //Console.WriteLine("=== Incoming formData ===");
        foreach (var kvp in formData)
        {
            //Console.WriteLine($"{kvp.Key} = {kvp.Value}");
        }
        var client = new HttpClient();
        //Console.WriteLine("Line 292");
        string baseUrl = _configuration["KeyCloak:BaseUrl"];
        //Console.WriteLine(baseUrl);
        client.BaseAddress = new Uri($"{baseUrl}/");
        Request.Headers.TryGetValue("Authorization", out var authHeader);
        string authorizationHeaderValue = authHeader.ToString();
        //Console.WriteLine(authorizationHeaderValue);
        var content = new FormUrlEncodedContent(formData);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authorizationHeaderValue.Split(" ").Last());
        var response = client.PostAsync("realms/quang-fhir-server/protocol/openid-connect/token", content).Result;

        if (!response.IsSuccessStatusCode)
        {
            //Console.WriteLine("Line 298");
            var identityRespondJson = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(identityRespondJson);
            return new JsonResult(data)
            {
                StatusCode = (int?)response.StatusCode,
                ContentType = "application/json"
            };
        }

        //Console.WriteLine("Line 311");
        var json = await response.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        if (tokenData != null && tokenData.TryGetValue("access_token", out var accessTokenObj))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessTokenObj.ToString());

            var patientClaim = jwt.Claims.FirstOrDefault(c => c.Type == "patient");
            if (patientClaim != null)
            {
                tokenData["patient"] = patientClaim.Value;
            }
            var encounterClaim = jwt.Claims.FirstOrDefault(c => c.Type == "encounter");
            if (encounterClaim != null)
            {
                tokenData["encounter"] = encounterClaim.Value;
            }
        }

        return new JsonResult(tokenData)
        {
            StatusCode = 200,
            ContentType = "application/json"
        };
    }
    // [HttpGet, Route("proxy/auth")]
    // public async Task<RedirectResult> ProxyAuth()
    // {
    //     var response_type = HttpContext.Request.Query["response_type"].ToString();
    //     var client_id = HttpContext.Request.Query["client_id"].ToString();
    //     var redirect_uri = HttpContext.Request.Query["redirect_uri"].ToString();
    //     var state = HttpContext.Request.Query["state"].ToString();
    //     var aud = HttpContext.Request.Query["aud"].ToString();
    //     var code_challenge = HttpContext.Request.Query["code_challenge"].ToString();
    //     var code_challenge_method = HttpContext.Request.Query["code_challenge_method"].ToString();
    //     var scope = "launch openid fhirUser offline_access patient/Provenance.read patient/PractitionerRole.read patient/MedicationDispense.read patient/ServiceRequest.read fhirUser patient/Encounter.read patient/Condition.read patient/Patient.read patient/AllergyIntolerance.read patient/Immunization.read patient/Specimen.read openid patient/DiagnosticReport.read patient/DocumentReference.read patient/Observation.read offline_access patient/CareTeam.read patient/Practitioner.read patient/Organization.read patient/Medication.read patient/Procedure.read patient/Coverage.read patient/Location.read patient/MedicationRequest.read launch/patient patient/CarePlan.read patient/Goal.read patient/Device.read";
    //     //var scope = "launch/patient openid fhirUser offline_access patient/Patient.read patient/Condition.read patient/Observation.read";
    //     //var scope = "launch openid fhirUser offline_access user/Patient.read user/AllergyIntolerance.read user/CarePlan.read user/CareTeam.read user/Condition.read user/Coverage.read user/Device.read user/DiagnosticReport.read user/DocumentReference.read user/Encounter.read user/Goal.read user/Immunization.read user/Location.read user/Medication.read user/MedicationDispense.read user/MedicationRequest.read user/Observation.read user/Organization.read user/Practitioner.read user/PractitionerRole.read user/Procedure.read user/Provenance.read user/QuestionnaireResponse.read user/RelatedPerson.read user/ServiceRequest.read user/Specimen.read";
    //     //var scope = "launch/patient openid fhirUser offline_access patient/Medication.read patient/AllergyIntolerance.read patient/CarePlan.read patient/CareTeam.read patient/Condition.read patient/Device.read patient/DiagnosticReport.read patient/DocumentReference.read patient/Encounter.read patient/Goal.read patient/Immunization.read patient/Location.read patient/MedicationRequest.read patient/Observation.read patient/Organization.read patient/Patient.read patient/Practitioner.read patient/Procedure.read patient/Provenance.read patient/PractitionerRole.read";
    //     //var scope = "launch openid fhirUser offline_access patient/Patient.read";
    //     var client = new HttpClient();
    //     //Console.WriteLine(response_type);
    //     //Console.WriteLine(client_id);
    //     //Console.WriteLine(redirect_uri);
    //     //Console.WriteLine(state);
    //     //Console.WriteLine(aud);
    //     //Console.WriteLine(scope);
    //     //Console.WriteLine(code_challenge);
    //     //Console.WriteLine(code_challenge_method);
    //     var builder = new UriBuilder("http://192.168.56.1:8080/realms/quang-fhir-server/protocol/openid-connect/auth");
    //     var query = HttpUtility.ParseQueryString(string.Empty);
    //     query["response_type"] = response_type;
    //     query["client_id"] = client_id;
    //     query["redirect_uri"] = redirect_uri;
    //     query["state"] = state;
    //     query["aud"] = aud;
    //     query["scope"] = scope;
    //     query["code_challenge"] = code_challenge;
    //     query["code_challenge_method"] = code_challenge_method;
    //     builder.Query = query.ToString();
    //     var finalUrl = builder.ToString();
    //     return Redirect(finalUrl);
    // }

}