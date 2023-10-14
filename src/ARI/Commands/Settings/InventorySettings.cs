using System.ComponentModel;

namespace ARI.Commands.Settings;

public class InventorySettings : CommandSettings
{
    [CommandArgument(0, "<tenantId>")]
    [ValidateString]
    [Description("Azure Tenant Id")]
    public string TenantId { get; set; } = string.Empty;

    [CommandArgument(1, "<outputpath>")]
    [ValidatePath]
    [Description("Output path")]
    public DirectoryPath OutputPath { get; set; } = System.Environment.CurrentDirectory;

    [CommandOption("--skip-tenant-overview")]
    public bool SkipTenantOverview { get; set; }
}