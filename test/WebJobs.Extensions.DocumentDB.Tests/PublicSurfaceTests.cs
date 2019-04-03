// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Extensions.Tests.Common;
using Xunit;

namespace Microsoft.Azure.WebJobs.Extensions.Tests
{
    public class PublicSurfaceTests
    {
        [Fact]
        public void DocumentDBPublicSurface_LimitedToSpecificTypes()
        {
            var assembly = typeof(DocumentDBAttribute).Assembly;

            var expected = new[]
            {
                "DocumentDBAttribute",
                "DocumentDBConfiguration",
                "DocumentDBJobHostConfigurationExtensions",
                "CosmosDBTriggerAttribute"
            };

            TestHelpers.AssertPublicTypes(expected, assembly);
        }
    }
}
