// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Extensions.Tests.Common;
using Xunit;

namespace Microsoft.Azure.WebJobs.Extensions.Http.Tests
{
    public class PublicSurfaceTests
    {
        [Fact]
        public void HttpPublicSurface_LimitedToSpecificTypes()
        {
            var assembly = typeof(HttpTriggerAttribute).Assembly;

            var expected = new[]
            {
                "HttpExtensionConfiguration",
                "HttpExtensionConstants",
                "HttpJobHostConfigurationExtensions",
                "AuthorizationLevel",
                "HttpRouteFactory",
                "HttpRequestManager",
                "HttpTriggerAttribute"
            };

            TestHelpers.AssertPublicTypes(expected, assembly);
        }
    }
}
