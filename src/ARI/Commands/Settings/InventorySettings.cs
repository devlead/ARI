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

    [CommandOption("--run-in-parallel")]
    public bool RunInParallel { get; set; }

    [CommandOption("--markdown-name")]
    public string MarkdownName { get; set; } = "index";

    [CommandOption("--skip-frontmatter")]
    public bool SkipFrontmatter { get; set; }

    [CommandOption("--skip-frontmatter-summary")]
    public bool SkipFrontmatterSummary { get; set; }

    [CommandOption("--skip-frontmatter-modifed")]
    public bool SkipFrontmatterModified { get; set; }

    [CommandOption("--skip-frontmatter-order")]
    public bool SkipFrontmatterOrder { get; set; }
}