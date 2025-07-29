### 💾 Using an Existing Keycloak Identity Server Database

If you already have a Spark FHIR database, you can export its data and replace the contents of the `keycloak-dump` file with your exported data. 

**Important** After exported your own data, make sure to change the database owner to `postgres` to avoid permission conflicts during container startup.


This allows the server to start with your existing resources.