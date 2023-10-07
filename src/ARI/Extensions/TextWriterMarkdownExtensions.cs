namespace ARI.Extensions;

public static  class TextWriterMarkdownExtensions
{
    public const int NameColumnWidth = 35;
    public const int TypeColumnWidth = 55;
    public const int DescriptionColumnWidth = 95;

    public static async Task AddFrontmatter(
        this TextWriter writer,
        DateTimeOffset lastUpdateTime,
        string summary,
        int order)
    {
        await writer.WriteLineAsync(
            FormattableString.Invariant(
                    $$"""
                    ---
                    summary: {{summary}}
                    modifiedby: ARI
                    modified: {{lastUpdateTime:yyyy-MM-dd HH:mm}}
                    order: {{order}}
                    ---
                    """
                )
            );
    }

    public static async Task AddTenantOverview(
        this TextWriter writer,
        AzureTenant tenant)
    {
        await writer.WriteLineAsync(
            FormattableString.Invariant(
                    $$"""
                    # Overview
                   
                    |                                     |                                                                                                 |
                    |-------------------------------------|-------------------------------------------------------------------------------------------------|
                    | **Name**                            | {{tenant.DisplayName.CodeLine(),-DescriptionColumnWidth}} |
                    | **Id**                              | {{tenant.Id.CodeLine(),-DescriptionColumnWidth}} |
                    | **Tenant Id**                       | {{tenant.TenantId.CodeLine(),-DescriptionColumnWidth}} |
                    | **Country code**                    | {{tenant.CountryCode.CodeLine(),-DescriptionColumnWidth}} |
                    | **Default domain**                  | {{tenant.DefaultDomain.CodeLine(),-DescriptionColumnWidth}} |
                    | **Domains**                         | {{string.Join(Environment.NewLine, tenant.Domains).CodeLine(),-DescriptionColumnWidth}} |
                    """
                )
            );
    }

    public static async Task AddSubscriptionOverview(
        this TextWriter writer,
        Subscription subscription)
    {
        await writer.WriteLineAsync(
            FormattableString.Invariant(
                    $$"""
                    # Overview
                   
                    |                                     |                                                                                                 |
                    |-------------------------------------|-------------------------------------------------------------------------------------------------|
                    | **Name**                            | {{subscription.DisplayName.CodeLine(),-DescriptionColumnWidth}} |
                    | **Id**                              | {{subscription.Id.CodeLine(),-DescriptionColumnWidth}} |
                    | **Subscription Id**                 | {{subscription.SubscriptionId.CodeLine(),-DescriptionColumnWidth}} |
                    | **State**                           | {{subscription.State.CodeLine(),-DescriptionColumnWidth}} |
                    """
                )
            );
    }
}
