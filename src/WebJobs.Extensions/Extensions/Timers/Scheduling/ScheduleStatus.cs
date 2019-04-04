// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Microsoft.Azure.WebJobs.Extensions.Timers
{
    /// <summary>
    /// Represents a timer schedule status. All times should be in UTC.
    /// </summary>
    public class ScheduleStatus
    {
        /// <summary>
        /// An alternative for default(DateTime) to use with the properties of this class which guarantees to be in UTC.
        /// </summary>
        public static readonly DateTime Never = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private DateTime lastUtc = Never;
        private DateTime nextUtc = Never;
        private DateTime lastUpdatedUtc = Never;

        /// <summary>
        /// The last recorded schedule occurrence
        /// </summary>
        public DateTime Last
        {
            get => lastUtc;
            set
            {
                Debug.Assert(value.Kind == DateTimeKind.Utc);
                lastUtc = value;
            }
        }

        /// <summary>
        /// The expected next schedule occurrence
        /// </summary>
        public DateTime Next
        {
            get => nextUtc;
            set
            {
                Debug.Assert(value.Kind == DateTimeKind.Utc);
                nextUtc = value;
            }
        }

        /// <summary>
        /// The last time this record was updated. This is used to re-calculate Next
        /// with the current Schedule after a host restart.
        /// </summary>
        public DateTime LastUpdated
        {
            get => lastUpdatedUtc;
            set
            {
                Debug.Assert(value.Kind == DateTimeKind.Utc);
                lastUpdatedUtc = value;
            }
        }
    }
}
