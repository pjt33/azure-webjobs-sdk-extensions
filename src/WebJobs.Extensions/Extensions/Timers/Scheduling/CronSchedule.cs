// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NCrontab;

namespace Microsoft.Azure.WebJobs.Extensions.Timers
{
    /// <summary>
    /// A scheduling strategy based on crontab expressions. <a href="http://en.wikipedia.org/wiki/Cron#CRON_expression"/>
    /// for details. E.g. "59 11 * * 1-5" is an expression representing every Monday-Friday at 11:59 AM.
    /// </summary>
    public class CronSchedule : TimerSchedule
    {
        private readonly CrontabSchedule _cronSchedule;

        /// <summary>
        /// Constructs a new instance based on the specified crontab expression
        /// </summary>
        /// <param name="cronTabExpression">The crontab expression defining the schedule</param>
        public CronSchedule(string cronTabExpression)
        {
            CrontabSchedule.ParseOptions options = new CrontabSchedule.ParseOptions()
            {
                IncludingSeconds = true
            };
            _cronSchedule = CrontabSchedule.Parse(cronTabExpression, options);
        }

        /// <summary>
        /// Constructs a new instance based on the specified crontab schedule
        /// </summary>
        /// <param name="schedule">The crontab schedule to use</param>
        public CronSchedule(CrontabSchedule schedule)
        {
            _cronSchedule = schedule;
        }

        /// <inheritdoc/>
        public override DateTime GetNextOccurrence(DateTime nowUtc, TimeZoneInfo tz)
        {
            Debug.Assert(nowUtc.Kind == DateTimeKind.Utc);

            if (tz == null)
            {
                throw new ArgumentNullException(nameof(tz));
            }

            // NCrontab operates in a generic timezone with no DST.
            // Naïve approach:
            //   1. Convert nowUtc to local time in the timezone
            //   2. Get next occurrence from NCrontab.
            //   3. Convert back to UTC.
            //
            // The problem is that this may fail due to DST changes.
            //
            // The local time produced in step 1 clearly exists, but it may be ambiguous.
            // The problem here is that the next time which matches the pattern might be lexicographically earlier.
            // E.g. if the job should execute every 10 minutes and the minute after 02:59 is 02:00 then we jump
            // "backwards".
            //
            // The local time produced in step 2 may be invalid or ambiguous.
            // In all the common use cases, skipping forward an hour from an invalid time is almost certainly correct.
            // If it's a frequent job (hourly or more frequent) then this will almost certainly be the next valid time to match.
            // It it's a rare job (six-hourly or less frequent) then we want to pick a suitable time to execute.
            // The main uncertainty is the gap in between: e.g. with a two-hourly job would we prefer a gap of one hour or three?
            // If the time is ambiguous, the use cases aren't as homogenous.
            // If it's a frequent job, we want to pick the earliest possibility.
            // If it's a rare job, we want to pick one or the other but not both.

            var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, tz);

            if (tz.IsAmbiguousTime(nowLocal))
            {
                // Force it backwards far enough to guarantee that we get a non-ambiguous time.
                var adjustmentRule = tz.GetAdjustmentRules()
                    .First(rule => rule.DateStart <= nowUtc && rule.DateEnd >= nowUtc);
                var deltaAbsolute = adjustmentRule.DaylightDelta < TimeSpan.Zero
                    ? -adjustmentRule.DaylightDelta
                    : adjustmentRule.DaylightDelta;
                nowLocal -= deltaAbsolute;

                Debug.Assert(!tz.IsAmbiguousTime(nowLocal));
            }

            DateTime? nextUtc = null;
            foreach (var candidateNextLocal in _cronSchedule.GetNextOccurrences(nowLocal, DateTime.MaxValue))
            {
                IEnumerable<DateTime> candidatesNextUtc;
                if (tz.IsInvalidTime(candidateNextLocal))
                {
                    candidatesNextUtc = new DateTime[]
                    {
                        TimeZoneInfo.ConvertTimeToUtc(candidateNextLocal + TimeSpan.FromHours(1), tz)
                    };
                }
                else if (tz.IsAmbiguousTime(candidateNextLocal))
                {
                    // Is the job frequent or rare? This is really quite hacky.
                    var delta = _cronSchedule.GetNextOccurrence(candidateNextLocal) - candidateNextLocal;
                    candidatesNextUtc = tz.GetAmbiguousTimeOffsets(candidateNextLocal)
                        .Select(offset => new DateTimeOffset(candidateNextLocal, offset).UtcDateTime)
                        .Take(delta.TotalHours < 4 ? 2 : 1);
                }
                else
                {
                    candidatesNextUtc = new DateTime[] { TimeZoneInfo.ConvertTimeToUtc(candidateNextLocal, tz) };
                }

                foreach (var candidateNextUtc in candidatesNextUtc)
                {
                    Debug.Assert(candidateNextUtc.Kind == DateTimeKind.Utc);
                    if (candidateNextUtc > nowUtc && candidateNextUtc < nextUtc.GetValueOrDefault(DateTime.MaxValue))
                    {
                        nextUtc = candidateNextUtc;
                    }
                }

                if (nextUtc.HasValue && !tz.IsAmbiguousTime(candidateNextLocal))
                {
                    break;
                }
            }

            return nextUtc.Value;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Cron: '{0}'", _cronSchedule.ToString());
        }

        internal static bool TryCreate(string cronExpression, out CronSchedule cronSchedule)
        {
            cronSchedule = null;
            CrontabSchedule.ParseOptions options = new CrontabSchedule.ParseOptions()
            {
                IncludingSeconds = true
            };
            CrontabSchedule crontabSchedule = CrontabSchedule.TryParse(cronExpression, options);
            if (crontabSchedule != null)
            {
                cronSchedule = new CronSchedule(crontabSchedule);
                return true;
            }
            return false;
        }
    }
}
