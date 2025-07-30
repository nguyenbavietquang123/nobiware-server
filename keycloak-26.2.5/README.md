# 1 Keycloak Configuration

To fully understand how to configure Keycloak using environment variables, refer to the official documentation:

🔗 [Keycloak Server Configuration Guide](https://www.keycloak.org/server/all-config)

---

## 2 Customizing Keycloak 

You have two options when configuring Keycloak in your setup:

- **Add more environment variables to the [Dockerfile](./Dockerfile)** to customize the behavior.
- **Use the default configuration** if it suits your local or test environment.

> ✅ Tip: Use environment variables to configure realms, clients, users, and other Keycloak settings dynamically during container startup.
