# EasyShiftHQ FullStack

This repository contains the EasyShiftHQ application built with the [ABP Framework](https://abp.io) on .NET 9.0.  It follows a layered Domain Driven Design architecture and provides a ready‑to‑use web application, database migrator and supporting projects.

## How the project works

Source code lives under the [`easyshifthq/`](easyshifthq) folder and is composed of the following layers:

- **Domain & Domain.Shared** – business objects and domain logic.
- **Application & Application.Contracts** – application services and DTOs.
- **EntityFrameworkCore** – data access with Entity Framework Core and PostgreSQL.
- **HttpApi & HttpApi.Client** – REST API endpoints and typed client proxies.
- **DbMigrator** – console app that applies EF Core migrations and seeds initial data.
- **Web** – ASP.NET Core MVC application that hosts the UI.

The solution uses PostgreSQL by default and can be extended with additional modules provided by ABP.

## Running locally

### Prerequisites

- [.NET SDK 9.0+](https://dotnet.microsoft.com/download)
- [Node.js 18 or 20](https://nodejs.org/en)
- A PostgreSQL server
- (Optional) [ABP CLI](https://abp.io/docs/latest/cli) for convenient tooling

If the .NET SDK is not already installed on your machine, you can install the exact
version pinned in [`global.json`](global.json) using Microsoft's install script:

```bash
curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version 9.0.100
```
The script installs the SDK to `~/.dotnet` and updates `PATH` for the current
session.

### Steps

1. Install client side packages:
   ```bash
   cd easyshifthq
   abp install-libs
   ```
2. Configure the database connection string in `src/easyshifthq.DbMigrator/appsettings.json` and (if needed) `src/easyshifthq.Web/appsettings.json`.
3. Create the database and seed initial data:
   ```bash
   dotnet run --project src/easyshifthq.DbMigrator
   ```
4. Run the web application:
   ```bash
   dotnet run --project src/easyshifthq.Web
   ```
   The site will be available at <https://localhost:44387> in development.
5. (Optional) Run the test suite:
   ```bash
   dotnet test easyshifthq.sln
   ```

## Deployment to Kubernetes

The repository contains Dockerfiles for both the Web application and the DbMigrator.  The general process for deploying to any Kubernetes provider (such as [DigitalOcean Kubernetes](https://www.digitalocean.com/products/kubernetes)) is:

1. **Build and push container images**
   ```bash
   cd easyshifthq
   dotnet publish src/easyshifthq.Web -c Release
   docker build -f src/easyshifthq.Web/Dockerfile -t <registry>/easyshifthq-web:latest .
   dotnet publish src/easyshifthq.DbMigrator -c Release
   docker build -f src/easyshifthq.DbMigrator/Dockerfile -t <registry>/easyshifthq-migrator:latest .
   docker push <registry>/easyshifthq-web:latest
   docker push <registry>/easyshifthq-migrator:latest
   ```
   Replace `<registry>` with your container registry, e.g. `registry.digitalocean.com/myrepo`.

2. **Run the database migrator** (once per environment) as a Kubernetes job:
   ```yaml
   apiVersion: batch/v1
   kind: Job
   metadata:
     name: easyshifthq-migrator
   spec:
     template:
       spec:
         restartPolicy: OnFailure
         containers:
           - name: migrator
             image: <registry>/easyshifthq-migrator:latest
             env:
               - name: ConnectionStrings__Default
                 value: "Host=mydb;Port=5432;Database=easyshifthq;User ID=...;Password=..."
   ```
   Apply with `kubectl apply -f migrator-job.yaml`.

3. **Deploy the web application**
   ```yaml
   apiVersion: apps/v1
   kind: Deployment
   metadata:
     name: easyshifthq-web
   spec:
     replicas: 1
     selector:
       matchLabels:
         app: easyshifthq-web
     template:
       metadata:
         labels:
           app: easyshifthq-web
       spec:
         containers:
           - name: web
             image: <registry>/easyshifthq-web:latest
             env:
               - name: ConnectionStrings__Default
                 value: "Host=mydb;Port=5432;Database=easyshifthq;User ID=...;Password=..."
             ports:
               - containerPort: 80
   ---
   apiVersion: v1
   kind: Service
   metadata:
     name: easyshifthq-web
   spec:
     type: LoadBalancer
     selector:
       app: easyshifthq-web
     ports:
       - port: 80
         targetPort: 80
   ```
   Apply with `kubectl apply -f web-deployment.yaml`.  On DigitalOcean this will provision a load balancer automatically.

These manifests are intentionally minimal; adjust replicas, resource limits, ingress and secrets for production use.  Any Kubernetes platform that supports standard deployments can run the application using the same approach.

## Additional resources

- [ABP Framework documentation](https://abp.io/docs/latest)
- [DigitalOcean Kubernetes documentation](https://docs.digitalocean.com/products/kubernetes/)

