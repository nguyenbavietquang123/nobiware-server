# 🌐 Spark.Web

  
> The `Spark.Web` project is where you can modify the **FHIR server REST API**.  
> 
> All FHIR API endpoints are defined in [spark/src/Spark.Web/Controllers/FhirController](spark/src/Spark.Web/Controllers/FhirController).  
> If you want to modify existing endpoints or add new ones, that is the file to edit.

---

> Available API Examples:

- `GET /fhir/Patient/{id}` — Retrieve a patient by ID  
- `POST /fhir/Observation` — Create a new observation  
- `PUT /fhir/Encounter/{id}` — Update an encounter  
- `DELETE /fhir/Condition/{id}` — Delete a condition  

---

> You can customize the controller logic directly in `FhirController.cs` to adjust how resources are handled.

---

## 1. Environment Configuration
Update the following fields in [`spark/src/Spark.Web/appsettings.json`](./appsettings.json):

> **Important:**  
> If you're running the app locally via Docker, replace all instances of `localhost` with your **host machine's IP address (ethernet IP)**.

The FHIR server is integrated with **Keycloak** for authentication and authorization. These environment variables is for this integration.  



---

## 2. Using Your Own Keycloak Info
> **Important:**
> If you have your own Keycloak server Domain and want to use it instead of the sample server please read it, otherwise ignore this section.  
Update the following fields in [`spark/src/Spark.Web/appsettings.json`](./appsettings.json):

| Setting              | Description                                      | Replace With                                                      |
|----------------------|--------------------------------------------------|-------------------------------------------------------------------|
| `BaseUrl`            | Base URL of your Keycloak server                | e.g. `https://auth.yourdomain.com`                                |
| `AuthorizationUrl`   | URL for login redirection                       | Replace `quang-fhir-server` with your Keycloak realm              |
| `MetadataAddress`    | OpenID configuration discovery URL              | Match your Keycloak realm                                         |
| `ValidIssuer`        | Must match `iss` field in the access token      | Usually the realm URL                                             |
| `Audience`           | Expected audience in token                      | Typically `account`, unless customized                            |
| `ClientId`           | Your Keycloak client ID                         | Your registered client ID                                         |
| `ClientSecret`       | Secret for the confidential client              | Your client secret                                                |
| `IntrospectEndpoint` | Path to the token introspection endpoint        | Update with your realm’s path                                     |

---

## 3 Example Setup (from Office Wiki Deployment Tutorial)

If you're following the deployment tutorial, set the environment variables as below, otherwise ignore this section:

| Setting              | Example Value                                                                 |
|----------------------|-------------------------------------------------------------------------------|
| `BaseUrl`            | `https://my-id.server.com`                                                    |
| `AuthorizationUrl`   | `quang-fhir-server`                                                           |
| `MetadataAddress`    | `https://my-id.server.com/realms/quang-fhir-server/.well-known/openid-configuration` |
| `ValidIssuer`        | `https://my-id.server.com/realms/quang-fhir-server`                           |
| `Audience`           | `account`                                                                     |
| `ClientId`           | `confidential_client`                                                         |
| `ClientSecret`       | `wRaUrcMeYXFRTm99lWFFd1cRoFh4ppkI`                                            |
| `IntrospectEndpoint` | `realms/quang-fhir-server/protocol/openid-connect/token/introspect`          |
