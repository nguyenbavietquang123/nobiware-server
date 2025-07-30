## 🔧 Project Overview

This repository contains the complete source code of a **FHIR server integrated with Keycloak** as the Identity Provider.  
It includes custom components that implement the **SMART on FHIR authorization flow** and is designed to **pass the Inferno Test Suite**.

---

## 📄 Pre-Build Configuration

Before building the project, please carefully read the `README.md` files in the following directories and follow the configuration steps:

- [`spark/src/Spark.Web/README.md`](spark/src/Spark.Web/README.md)
- [`FE/README.md`](FE/README.md)
- [`BE/README.md`](BE/README.md)
- [`keycloak-26.2.5/README.md`](keycloak-26.2.5/README.md)
- [`keycloak-26.2.5/README.md`](keycloak-26.2.5/README.md)
- [`fhir-mongo-database/README.md`](fhir-mongo-database/README.md)
- [`keycloak-postgre-database/README.md`](keycloak-postgre-database/README.md)

---

## 🚀 Build & Run

After completing the setup, you can build and run the project using the following command:

```bash
docker-compose up --build
```

## Quick Brief:
KeyCloak is running on port: 8080
FhirServer is running on port: 6000
Frontend Auth Proxy is running on port: 3000
Backend Auth Proxy is running on port: 5000
## API Flow for testing (all the API URL is based on the provide sample data ):

## Step 1: Get access token with client credential flow: 
## 📤 HTTP Request

**POST** `http://<your-ip-address>:8080/realms/quang-fhir-server/protocol/openid-connect/token`
## 🧾 Request Headers

| Header           | Value                         |
|------------------|-------------------------------|
| Content-Type     | application/x-www-form-urlencoded |

---

## 📦 Request Body Parameters

Send the following as `x-www-form-urlencoded`:

| Parameter      | Required | Value                               |
|----------------|----------|-------------------------------------------|
| `grant_type`   | ✅       | client_credentials              |
| `client_id`    | ✅       | Portal-Client          |
| `client_secret`| ✅       | Jhhup28xLYAF2vrMiyZ4gsjBKbbPB9ly          |

If the request is success. You will receive an access token to call FHIR server API. The access token will live only 60 second. If you want to increase 
the life of access token. Please change it on Admin Console.

## Step 2: Call FHIR APIL

**GET** `http://<your-ip-address>:6000/fhir/Patient`
## Authorization: 

|                |                                 |
|----------------|-------------------------------------------|
| Auth Type   |  Bearer Token              |
| value    | The access token receive in Step 1          |

If the request have response code 200 and receive a Bundle of Patient. Your servers are running OK in local.




