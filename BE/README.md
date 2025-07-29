# 🔐 Backend Auth Proxy for Inferno Test Suite

This project serves as the **backend of the Auth Proxy**, designed to support the **standard SMART on FHIR authorization flow** required by the [Inferno Test Suite](https://inferno.healthit.gov/).

---

## ⚙️ Environment Configuration

Before running the project, create a `.env` file in the root directory and define the following environment variables:

+ KEYCLOAK_BASE_URL=http://<your-ip-address>:8080
+ REALM=quang-fhir-server
+ PROXY_REDIRECT_URL=http://<your-ip-address>:5000

## 🛠️ Explanation

| Variable             | Description                                                   |
| -------------------- | ------------------------------------------------------------- |
| `KEYCLOAK_BASE_URL`  | Base URL of your Keycloak server                              |
| `REALM`              | Name of the Keycloak realm used for authentication            |
| `PROXY_REDIRECT_URL` | The redirect URL of this proxy server (usually your frontend) |


## 🔁 Customization
You can change these values based on your own Keycloak setup and domain:
* If you're using your own infrastructure, update the values accordingly.
* If you're using the provided sample data and running everything locally, you can leave the default values.

## Deployment Setup (from Office Wiki):
If you are following the deployment tutorial on Office Wiki, use the values below:

+ KEYCLOAK_BASE_URL=https://my-id.server.com
+ REALM=quang-fhir-server
+ PROXY_REDIRECT_URL=https://my-id.server.com