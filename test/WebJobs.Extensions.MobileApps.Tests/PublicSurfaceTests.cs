// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Extensions.Tests.Common;
using Xunit;

namespace Microsoft.Azure.WebJobs.Extensions.Tests
{
    public class PublicSurfaceTests
    {
        [Fact]
        public void MobileTablesPublicSurface_LimitedToSpecificTypes()
        {
            var assembly = typeof(MobileTableAttribute).Assembly;

            var expected = new[]
            {
                "MobileTableAttribute",
                "MobileAppsConfiguration",
                "MobileAppsJobHostConfigurationExtensions"
            };

            TestHelpers.AssertPublicTypes(expected, assembly);
        }
    }
}
