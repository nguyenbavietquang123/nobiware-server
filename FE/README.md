# 🔐 Frontend Auth Proxy for Inferno Test Suite

This project provides the **frontend component** of the Auth Proxy, enabling support for the **SMART on FHIR authorization flow** required by the [Inferno Test Suite](https://inferno.healthit.gov/).

---

## ⚙️ Environment Configuration

Before running the project, Go to `src/ScopeConsent/ScopeConsent.js` file and update the following variables:

proxyAuthBaseUrl=http://<your-ip-address>:5000
authBaseUrl="http://<your-ip-address>:8080"
realm="quang-fhir-server"

## 🛠️ Explanation

| Variable                  | Description                                                   |
| --------------------------| ------------------------------------------------------------- |
| `authBaseUrl`             | Base URL of your Keycloak server                              |
| `realm`                   | Name of the Keycloak realm used for authentication            |
| `proxyAuthBaseUrl`        | The base URL of your backend Auth Proxy                       |


## 🔁 Customization
You can change these values based on your own Keycloak setup and domain:
    If you're using your own infrastructure, update the values accordingly.
    If you're using the provided sample data and running everything locally, you can leave the default values.

## If you are following the deployment tutorial on Office Wiki:
You should update the variables like that:

proxyAuthBaseUrl="https://my-id.server.com"
authBaseUrl="https://my-id.server.com"
realm="quang-fhir-server"