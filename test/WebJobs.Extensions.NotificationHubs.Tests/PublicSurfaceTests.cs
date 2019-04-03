// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Extensions.Tests.Common;
using Xunit;

namespace Microsoft.Azure.WebJobs.Extensions.NotificationHubs.Tests
{
    public class PublicSurfaceTests
    {
        [Fact]
        public void NotificationHubsPublicSurface_LimitedToSpecificTypes()
        {
            var assembly = typeof(NotificationHubAttribute).Assembly;

            var expected = new[]
            {
                "NotificationHubAttribute",
                "NotificationHubsConfiguration",
                "NotificationHubJobHostConfigurationExtensions"
            };

            TestHelpers.AssertPublicTypes(expected, assembly);
        }
    }
}
