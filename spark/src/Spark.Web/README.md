# 🌐 Spark.Web

The `Spark.Web` project is where you can modify the **FHIR server REST API**.  
All FHIR API endpoints are defined in [`./Controllers/FhirController`](./Controllers/FhirController.cs).

---

## ⚙️ Environment Configuration

The FHIR server is integrated with **Keycloak** for authentication and authorization.  
You can either update the environment settings to match your Keycloak setup or leave them as default for local development.

> **Important:**  
> If you're running the app locally via Docker, replace all instances of `localhost` with your **host machine's IP address**.

---

## 🛠 Replace with Your Own Keycloak Info

Update the following fields in `appsettings.json` or `appsettings.Development.json`:

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

## 📘 Example Setup (from Office Wiki Deployment Tutorial)

If you're following the deployment tutorial, set the environment variables as below:

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
