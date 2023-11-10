# ARI

Azure Resource Inventory .NET Tool - Inventories and documents Azure Tenant resources to a set of markdown files for specified tenant to a specified folder.

## Obtain

```bash
dotnet tool install -g ari
```

## Commands

Use `-h` / `--help` to get the current list of available commands and options.

```bash
ari --help
ari command --help
```

### inventory

The **inventory** command inventories tenants and outputs it's result as markdown files to specified output path.

```bash
ari inventory <tenantId> <outputpath> [options]
```

## Quick Start

### Running ARI Successfully for the First Time

By default it'll try authenticate using the [DefaultAzureCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet) which tries to authorize in the following order based on your environment.

1. [EnvironmentCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.environmentcredential?view=azure-dotnet)
1. [WorkloadIdentityCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.workloadidentitycredential?view=azure-dotnet)
1. [ManagedIdentityCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.managedidentitycredential?view=azure-dotnet)
1. [SharedTokenCacheCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.sharedtokencachecredential?view=azure-dotnet)
1. [VisualStudioCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.visualstudiocredential?view=azure-dotnet)
1. [VisualStudioCodeCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.visualstudiocodecredential?view=azure-dotnet)
1. [AzureCliCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azureclicredential?view=azure-dotnet)
1. [AzurePowerShellCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azurepowershellcredential?view=azure-dotnet)
1. [AzureDeveloperCliCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azuredeveloperclicredential?view=azure-dotnet)
1. [InteractiveBrowserCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.interactivebrowsercredential?view=azure-dotnet)

#### Setup Azure App Registration

The recommended way is using a service principle with only the access required for it to document, you can do this by creating an app registration.

1. Begin by creating an `App Registration` in Azure Entra for the report generator. This ensures that the report generator has precisely the required access, such as organization-wide read permissions or access to a limited set of subscriptions.
1. Assign the API permission `https://graph.microsoft.com/Organization.Read.All` to the created `App Registration`.
1. In my tenant, Admin consent is required for this permission.
1. Add a role that allows the `App Registration` to read an organization. You can do this under `Subscription` management in Azure and `Access Control (IAM)`. Add `Role Assignment`, find the `App Registration` and give it `Read` access.
1. Assign a secret to the `App Registration` and make a note of this secret.

With the Azure App Registration now configured, we are ready to proceed.

#### Configure ARI for Execution

1. Create a dedicated folder for the generated report.
1. Set the environment variable `AZURE_TENANT_ID` to the tenant ID (found in the `App Registration` overview for your app).
1. Set the environment variable `AZURE_CLIENT_ID` to the client ID (found in the `App Registration` overview for your app).
1. Set the environment variable `AZURE_CLIENT_SECRET` to the secret noted earlier.
1. Set the environment variable `AZURE_AUTHORITY_HOST` to `https://login.microsoftonline.com/`.

### Run ARI

Assuming all the environment variables are correctly set, follow these steps:

```bash
dotnet tool install --global ARI
ari <AZURE_TENANT_ID> <FOLDER_FOR_REPORT>
```

By following these steps, you should be able to run ARI successfully for the first time. If you encounter any issues, double-check the Azure App Registration setup and ensure that the environment variables are accurately configured.
