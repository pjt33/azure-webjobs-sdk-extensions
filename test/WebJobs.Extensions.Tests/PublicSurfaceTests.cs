// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Extensions.Tests.Common;
using Xunit;

namespace Microsoft.Azure.WebJobs.Extensions.Tests
{
    public class PublicSurfaceTests
    {
        [Fact]
        public void ExtensionsPublicSurface_LimitedToSpecificTypes()
        {
            var assembly = typeof(TimerTriggerAttribute).Assembly;

            var expected = new[]
            {
                "ConstantSchedule",
                "CoreJobHostConfigurationExtensions",
                "CronSchedule",
                "DailySchedule",
                "ErrorTriggerAttribute",
                "ExecutionContext",
                "FileAttribute",
                "FileProcessor",
                "FileProcessorFactoryContext",
                "FilesConfiguration",
                "FilesJobHostConfigurationExtensions",
                "FileSystemScheduleMonitor",
                "StorageScheduleMonitor",
                "FileTriggerAttribute",
                "IFileProcessorFactory",
                "JobHostConfigurationExtensions",
                "ScheduleMonitor",
                "ScheduleStatus",
                "SlidingWindowTraceFilter",
                "StreamValueBinder",
                "TimerInfo",
                "TimerJobHostConfigurationExtensions",
                "TimerSchedule",
                "TimersConfiguration",
                "TimerTriggerAttribute",
                "TraceFilter",
                "TraceMonitor",
                "ValueBinder",
                "WeeklySchedule"
            };

            TestHelpers.AssertPublicTypes(expected, assembly);
        }
    }
}
