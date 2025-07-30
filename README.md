## 1. Project Overview

This repository contains the complete source code of a **FHIR server integrated with Keycloak** as the Identity Provider.  
It includes custom components that implement the **SMART on FHIR authorization flow** and is designed to **pass the Inferno Test Suite**.

---

## 2. Pre-Build Configuration

Before building the project, please carefully read the `README.md` files in the following directories and follow the configuration steps:
- [`fhir-mongo-database/README.md`](fhir-mongo-database/README.md) -- Setup FHIR Server database
- [`keycloak-postgre-database/README.md`](keycloak-postgre-database/README.md) -- Setup Keycloak database
- [`spark/src/Spark.Web/README.md`](spark/src/Spark.Web/README.md) -- Setup Spark FHIR server
- [`keycloak-26.2.5/README.md`](keycloak-26.2.5/README.md) -- Setup Keycloak Server
- [`FE/README.md`](FE/README.md) -- (Optional setup for passing Inferno Test Suite)
- [`BE/README.md`](BE/README.md) -- (Optional setup for passing Inferno Test Suite)



---

## 3. Build & Run

After completing the setup, you can build and run the project using the following command:

```bash
docker-compose up --build
```

## 4. Quick Brief:
- KeyCloak is running on port: 8080
- FhirServer is running on port: 6000
- Frontend Auth Proxy is running on port: 3000
- Backend Auth Proxy is running on port: 5000
## 5.API Flow for testing 
Note: All the API URL is based on the provide sample data:

## 5.1  Get access token with client credential flow: 
## 5.1.2 HTTP Request

**POST** `http://<your-ip-address>:8080/realms/quang-fhir-server/protocol/openid-connect/token`
## 5.1.3 Request Headers

| Header           | Value                         |
|------------------|-------------------------------|
| Content-Type     | application/x-www-form-urlencoded |

---

## 5.1.4 Request Body Parameters

Send the following as `x-www-form-urlencoded`:

| Parameter      | Required | Value                               |
|----------------|----------|-------------------------------------------|
| `grant_type`   | ✅       | client_credentials              |
| `client_id`    | ✅       | Portal-Client          |
| `client_secret`| ✅       | Jhhup28xLYAF2vrMiyZ4gsjBKbbPB9ly          |

If the request is success. You will receive an access token to call FHIR server API. The access token will live only 60 second. If you want to increase 
the life of access token. Please change it on Admin Console.

## 5.2: Call FHIR API
## 5.2.1 HTTP Request
**GET** `http://<your-ip-address>:6000/fhir/Patient`
## 5.2.2 Authorization: 

|                |                                 |
|----------------|-------------------------------------------|
| Auth Type   |  Bearer Token              |
| value    | The access token receive in Step 1          |

If the request have response code 200 and receive a Bundle of Patient. Your servers are running OK in local.




