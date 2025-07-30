### Keycloak database setup
This is where you import the existing Keycloak Database by default there are sample data for testing in [keycloak-dump](./keycloak-dump):

### Using an Existing Keycloak Identity Server Database

If you already have a **Keycloak** database, you can reuse it by exporting your data and replacing the contents of the `keycloak-dump` directory with your own export (If you use the provided sample data ignore this section).

> ⚠️ **Important:**  
> After exporting your data, make sure to set the **database owner to `postgres`** to avoid permission conflicts during container startup.

This setup allows the Keycloak server to start with your **existing users, realms, and configurations** intact.
