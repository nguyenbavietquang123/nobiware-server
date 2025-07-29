# Spark.Web

Spark.Web is where you can modify FHIR server Rest API. You can see all the API of FHIR server in ./Controller/FHIR Controller.

# The explaination of some environment in appsetting.json and appsetting.development.
Your FHIR server connects to Keycloak for authentication and authorization. You can either update the following settings to match your own Keycloak domain and realm or leave it by default to run it on local.
**Important**: If you run the app on local via Docker, please replace all the `localhost` with your IP address.
### 🛠 How to Replace with Your Own Keycloak Info:

| Setting             | Description                                      | How to Replace                                                    |
|---------------------|--------------------------------------------------|-------------------------------------------------------------------|
| `BaseUrl`           | The base URL of your Keycloak server             | Replace with your domain, e.g. `https://auth.yourdomain.com`      |
| `AuthorizationUrl`  | URL used for redirect-based login                | Replace `quang-fhir server` with your own realm name              |
| `MetadataAddress`   | OpenID configuration discovery URL               | Update to match your Keycloak realm                               |
| `ValidIssuer`       | Must match the `iss` field in the access token   | Usually the same as your realm URL                                |
| `Audience`          | Expected audience in token (usually `account`)   | Keep as `account` unless your Keycloak client uses something else |
| `ClientId`          | The client ID registered in Keycloak             | Replace with your own client ID                                   |
| `ClientSecret`      | Secret for your confidential client              | Replace with your actual secret                                   |
| `IntrospectEndpoint`| Token introspection endpoint (relative path)     | Replace the realm with your own                                   |


## If you are following the deployment tutorial on Office Wiki:
You should update the environment variables like that:

| Setting             | Description                                      | How to Replace                                                    |
|---------------------|--------------------------------------------------|-------------------------------------------------------------------|
| `BaseUrl`           | The base URL of your Keycloak server             | `https://my-id.server.com`                                        |
| `AuthorizationUrl`  | URL used for redirect-based login                | `quang-fhir-server`                                               |
| `MetadataAddress`   | OpenID configuration discovery URL               | `https://my-id.server.com/realms/quang-fhir-server/.well-known/openid-configuration`                                                                                                                        |
| `ValidIssuer`       | Must match the `iss` field in the access token   | `https://my-id.server.com/realms/quang-fhir-server`               |
| `Audience`          | Expected audience in token (usually `account`)   | Keep as `account` unless your Keycloak client uses something else |
| `ClientId`          | The client ID registered in Keycloak             | `confidential_client`                                             |
| `ClientSecret`      | Secret for your confidential client              | `wRaUrcMeYXFRTm99lWFFd1cRoFh4ppkI`                                |
| `IntrospectEndpoint`| Token introspection endpoint (relative path)     | `realms/quang-fhir-server/protocol/openid-connect/token/introspect`                                                                                                                                  |