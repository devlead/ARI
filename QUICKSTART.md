# Quick Start

## Running ARI Successfully for the First Time

### Setup Azure App Registration

1. Begin by creating an `App Registration` in Azure Entra for the report generator. This ensures that the report generator has precisely the required access, such as organization-wide read permissions or access to a limited set of subscriptions.

2. Assign the API permission `https://graph.microsoft.com/Organization.Read.All` to the created `App Registration`.

3. In my tenant, Admin consent is required for this permission.

4. Add a role that allows the `App Registration` to read an organization. You can do this under `Subscription` management in Azure and `Access Control (IAM)`. Add `Role Assignment`, find the `App Registration` and give it `Read` access.

5. Assign a secret to the `App Registration` and make a note of this secret.

With the Azure App Registration now configured, we are ready to proceed.

### Configure ARI for Execution

1. Create a dedicated folder for the generated report.

2. Set the environment variable `AZURE_TENANT_ID` to the tenant ID (found in the `App Registration` overview for your app).

3. Set the environment variable `AZURE_CLIENT_ID` to the client ID (found in the `App Registration` overview for your app).

4. Set the environment variable `AZURE_CLIENT_SECRET` to the secret noted earlier.

5. Set the environment variable `AZURE_AUTHORITY_HOST` to `https://login.microsoftonline.com/`.

## Run ARI

Assuming all the environment variables are correctly set, follow these steps:

```bash
dotnet tool install --global ARI
ari <AZURE_TENANT_ID> <FOLDER_FOR_REPORT>
```

By following these steps, you should be able to run ARI successfully for the first time. If you encounter any issues, double-check the Azure App Registration setup and ensure that the environment variables are accurately configured.