## This repository contains the complete source code of a FHIR server integrated with Keycloak as the Identity Server. It includes custom components that implement the SMART on FHIR authorization flow and is designed to pass the Inferno Test Suite.

## Before building the project, please carefully read the README.md files in the following directories and follow the configuration steps:
+ `spark/src/Spark.Web/README.md`
+ `FE/README.md`
+ `BE/README.md`
+ `keycloak-26.2.5/README.md`

## Then you can build and run the source by this command:
```bash
docker-compose up --build
```