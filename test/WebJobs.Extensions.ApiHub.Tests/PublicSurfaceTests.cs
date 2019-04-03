// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Extensions.Tests.Common;
using Xunit;

namespace Microsoft.Azure.WebJobs.Extensions.ApiHub.Tests
{
    public class PublicSurfaceTests
    {
        [Fact]
        public void ApiHubPublicSurface_LimitedToSpecificTypes()
        {
            var assembly = typeof(ApiHubFileAttribute).Assembly;

            var expected = new[]
            {
                "ApiHubFileAttribute",
                "ApiHubFileTriggerAttribute",
                "ApiHubConfiguration",
                "ApiHubJobHostConfigurationExtensions",
                "ApiHubTableAttribute",
                "ConnectionFactory"
            };

            TestHelpers.AssertPublicTypes(expected, assembly);
        }
    }
}
