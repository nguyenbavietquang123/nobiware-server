FROM quay.io/keycloak/keycloak:latest AS builder

# Enable health and metrics support
ENV KC_HEALTH_ENABLED=true
ENV KC_METRICS_ENABLED=true

# Configure a database vendor
#ENV KC_DB=postgres
# Add the provider JAR file to the providers directory
COPY --chown=keycloak:keycloak providers/keycloak-sha256-provider-1.0-SNAPSHOT.jar /opt/keycloak/providers/myprovider.jar
WORKDIR /opt/keycloak
# for demonstration purposes only, please make sure to use proper certificates in production instead
RUN keytool -genkeypair -storepass password -storetype PKCS12 -keyalg RSA -keysize 2048 -dname "CN=server" -alias server -ext "SAN:c=DNS:localhost,IP:127.0.0.1" -keystore conf/server.keystore
RUN /opt/keycloak/bin/kc.sh build

FROM quay.io/keycloak/keycloak:26.2.5
COPY --from=builder /opt/keycloak/ /opt/keycloak/

# change these values to point to a running postgres instance

ENV KC_DB=postgres
ENV KC_DB_URL=jdbc:postgresql://keycloakdb:5432/keycloak
ENV KC_DB_USERNAME=keycloak
ENV KC_DB_PASSWORD=password
ENV KC_HOSTNAME_PORT=8080
#ENV KC_HOSTNAME=id-dev.nobiware.com
ENV KC_PROXY_HEADERS=xforwarded
ENV KC_PROXY=edge
ENV KC_HOSTNAME_STRICT=false
ENV KC_HOSTNAME_STRICT_HTTPS=false

# ENV KC_DB_PASSWORD=nguyenbavietquang123
# ENV KC_HOSTNAME=keycloakserverquang1.blackhill-47d65bd5.eastasia.azurecontainerapps.io
ENTRYPOINT ["/opt/keycloak/bin/kc.sh"]
CMD ["start-dev"]