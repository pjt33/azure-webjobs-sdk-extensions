﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Azure.WebJobs.Extensions.Timers;

namespace Microsoft.Azure.WebJobs
{
    /// <summary>
    /// Provides access to timer schedule information for jobs triggered 
    /// by <see cref="TimerTriggerAttribute"/>
    /// </summary>
    public class TimerInfo
    {
        /// <summary>
        /// Constructs a new instance
        /// </summary>
        /// <param name="tz">The timezone in which the timer operates.</param>
        /// <param name="schedule">The timer trigger schedule.</param>
        /// <param name="status">The current schedule status.</param>
        /// <param name="isPastDue">True if the schedule is past due, false otherwise.</param>
        public TimerInfo(TimeZoneInfo tz, TimerSchedule schedule, ScheduleStatus status, bool isPastDue = false)
        {
            TimeZone = tz;
            Schedule = schedule;
            ScheduleStatus = status;
            IsPastDue = isPastDue;
        }

        /// <summary>
        /// Gets the timezone in which this timer operates.
        /// </summary>
        public TimeZoneInfo TimeZone { get; private set; }

        /// <summary>
        /// Gets the schedule for the timer trigger.
        /// </summary>
        public TimerSchedule Schedule { get; private set; }

        /// <summary>
        /// Gets or sets the current schedule status for this timer.
        /// If schedule monitoring is not enabled for this timer (see <see cref="TimerTriggerAttribute.UseMonitor"/>)
        /// this property will return null.
        /// </summary>
        public ScheduleStatus ScheduleStatus { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this timer invocation
        /// is due to a missed schedule occurrence.
        /// </summary>
        public bool IsPastDue { get; private set; }

        /// <summary>
        /// Formats the next 'count' occurrences of the schedule into an
        /// easily loggable string.
        /// </summary>
        /// <param name="count">The number of occurrences to format.</param>
        /// <param name="nowUtc">The <see cref="DateTime"/> to start from.</param>
        /// <returns>A formatted string with the next occurrences.</returns>
        public string FormatNextOccurrences(int count, DateTime nowUtc)
        {
            return FormatNextOccurrences(Schedule, count, nowUtc, TimeZone);
        }

        internal static string FormatNextOccurrences(TimerSchedule schedule, int count, DateTime nowUtc, TimeZoneInfo tz)
        {
            Debug.Assert(nowUtc.Kind == DateTimeKind.Utc);

            if (schedule == null)
            {
                throw new ArgumentNullException("schedule");
            }

            IEnumerable<DateTime> nextOccurrences = schedule.GetNextOccurrences(count, nowUtc, tz);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("The next {0} occurrences of the schedule will be:", count));
            foreach (var occurrence in nextOccurrences)
            {
                builder.AppendLine(occurrence.ToString());
            }

            return builder.ToString();
        }
    }
}
