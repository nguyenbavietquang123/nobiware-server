### Using an Existing Spark FHIR Database

If you already have a **Spark FHIR** database, you can integrate it into this setup by following these steps:

1. **Export** your existing database data.
2. **Replace** the contents of the `dump/spark/` directory with your exported data.

> ✅ This enables the FHIR server to start with your **pre-existing resources**, preserving your current data and configurations.
