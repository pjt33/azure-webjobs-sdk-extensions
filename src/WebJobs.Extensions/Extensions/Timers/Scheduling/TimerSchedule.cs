// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Azure.WebJobs.Host;

namespace Microsoft.Azure.WebJobs.Extensions.Timers
{
    /// <summary>
    /// A timer scheduling strategy used with <see cref="TimerTriggerAttribute"/> for schedule
    /// based triggered jobs.
    /// </summary>
    public abstract class TimerSchedule
    {
        /// <summary>
        /// Gets the next occurrence of the schedule based on the specified
        /// base time.
        /// </summary>
        /// <param name="nowUtc">The time to compute the next schedule occurrence from.</param>
        /// <param name="tz">The timezone in which the schedule is executing.</param>
        /// <returns>The next schedule occurrence.</returns>
        public abstract DateTime GetNextOccurrence(DateTime nowUtc, TimeZoneInfo tz);

        /// <summary>
        /// Returns a collection of the next 'count' occurrences of the schedule,
        /// starting from nowUtc.
        /// </summary>
        /// <param name="count">The number of occurrences to return.</param>
        /// <param name="nowUtc">The <see cref="DateTime"/> to start from.</param>
        /// <param name="tz">The timezone in which the schedule is executing.</param>
        /// <returns>A collection of the next occurrences.</returns>
        public IEnumerable<DateTime> GetNextOccurrences(int count, DateTime nowUtc, TimeZoneInfo tz)
        {
            Debug.Assert(nowUtc.Kind == DateTimeKind.Utc);

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            var occurrences = new List<DateTime>();
            for (int i = 0; i < count; i++)
            {
                DateTime next = GetNextOccurrence(nowUtc, tz);
                occurrences.Add(next);
                nowUtc = next;
            }

            return occurrences;
        }

        internal static TimerSchedule Create(TimerTriggerAttribute attribute, INameResolver nameResolver)
        {
            TimerSchedule schedule = null;

            if (!string.IsNullOrEmpty(attribute.ScheduleExpression))
            {
                string resolvedExpression = nameResolver.ResolveWholeString(attribute.ScheduleExpression);
                if (CronSchedule.TryCreate(resolvedExpression, out CronSchedule cronSchedule))
                {
                    schedule = cronSchedule;

                    var nowUtc = DateTime.UtcNow;
                    var secondOccurrence = cronSchedule.GetNextOccurrences(2, nowUtc, TimeZoneInfo.Utc).Last();
                    if (secondOccurrence < nowUtc + TimeSpan.FromMinutes(1))
                    {
                        // if there is more than one occurrence due in the next minute,
                        // assume that this is a sub-minute constant schedule and disable
                        // persistence
                        attribute.UseMonitor = false;
                    }
                }
                else if (TimeSpan.TryParse(resolvedExpression, out TimeSpan periodTimespan))
                {
                    schedule = new ConstantSchedule(periodTimespan);

                    if (periodTimespan.TotalMinutes < 1)
                    {
                        // for very frequent constant schedules, we want to disable persistence
                        attribute.UseMonitor = false;
                    }
                }
                else
                {
                    throw new ArgumentException(string.Format("The schedule expression '{0}' was not recognized as a valid cron expression or timespan string.", resolvedExpression));
                }
            }
            else
            {
                schedule = (TimerSchedule)Activator.CreateInstance(attribute.ScheduleType);
            }

            return schedule;
        }
    }
}
