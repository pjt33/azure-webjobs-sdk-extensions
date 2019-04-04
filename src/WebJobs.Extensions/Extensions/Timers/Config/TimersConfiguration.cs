// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Extensions.Timers
{
    /// <summary>
    /// Configuration object for <see cref="TimerTriggerAttribute"/> decorated job functions.
    /// </summary>
    public class TimersConfiguration
    {
        /// <summary>
        /// Gets or sets the schedule monitor used to persist
        /// schedule occurrences and monitor execution.
        /// </summary>
        public ScheduleMonitor ScheduleMonitor { get; set; }

        /// <summary>
        /// Gets or sets the timezone used to interpret schedule definitions.
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; }
    }
}
