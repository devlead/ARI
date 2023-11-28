namespace ARI.Models.Tenant;

public class AzureResourceTags : SortedDictionary<string, string>
{
    public static AzureResourceTags Empty { get; } = new();


    public AzureResourceTags() :base(StringComparer.OrdinalIgnoreCase)
    {
    }
}

