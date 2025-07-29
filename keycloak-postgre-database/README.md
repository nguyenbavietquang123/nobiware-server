### Using an Existing Keycloak Identity Server Database

If you already have a **Keycloak** database, you can reuse it by exporting your data and replacing the contents of the `keycloak-dump` directory with your own export.

> ⚠️ **Important:**  
> After exporting your data, make sure to set the **database owner to `postgres`** to avoid permission conflicts during container startup.

This setup allows the Keycloak server to start with your **existing users, realms, and configurations** intact.
