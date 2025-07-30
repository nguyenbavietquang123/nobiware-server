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
