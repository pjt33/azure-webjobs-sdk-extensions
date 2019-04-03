// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Extensions.Tests.Common;
using Xunit;

namespace Microsoft.Azure.WebJobs.Extensions.SendGrid.Tests
{
    public class PublicSurfaceTests
    {
        [Fact]
        public void SendGridPublicSurface_LimitedToSpecificTypes()
        {
            var assembly = typeof(SendGridAttribute).Assembly;

            var expected = new[]
            {
                "SendGridAttribute",
                "SendGridConfiguration",
                "SendGridJobHostConfigurationExtensions"
            };

            TestHelpers.AssertPublicTypes(expected, assembly);
        }
    }
}
