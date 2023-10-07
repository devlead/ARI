using Markdig.Extensions.AutoIdentifiers;

namespace BRI.TestWeb;

public class DefaultAutoIdentifierExtension : AutoIdentifierExtension
{
    public DefaultAutoIdentifierExtension()
        : base(AutoIdentifierOptions.Default)
    {

    }
}
