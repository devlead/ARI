using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARI.Tests.Unit.Services.ARM;

[TestFixture]
public class ResourceGroupServiceTests
{
    [TestCase(Constants.Tenant.Subscription1.Id)]
    [TestCase(Constants.Tenant.Subscription2.Id)]
    public async Task GetSubscriptions(string subscriptionId)
    {
        // Given
        var resourceGroupService = ARIServiceProviderFixture
                                    .GetRequiredService<ResourceGroupService>();

        // When
        var result = await resourceGroupService.GetResourceGroups(Constants.Tenant.Id, subscriptionId);

        // Then
        await Verify(result);
    }
}
