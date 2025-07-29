# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy entire src folder
COPY src ./src

# Set working dir to Spark.Web inside copied src
WORKDIR /src/src/Spark.Web

RUN dotnet restore
RUN dotnet publish Spark.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Spark.Web.dll"]