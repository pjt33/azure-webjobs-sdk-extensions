// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Xunit;

namespace Microsoft.Azure.WebJobs.Extensions.Tests.Timers.Scheduling
{
    public class CronScheduleTests
    {
        private static readonly TimeZoneInfo _timezonePacific = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

        [Fact]
        public void GetNextOccurrence_NowEqualToNext_ReturnsCorrectValue()
        {
            CronSchedule schedule = new CronSchedule("0 * * * * *");

            var now = schedule.GetNextOccurrence(DateTime.UtcNow, _timezonePacific);
            var next = schedule.GetNextOccurrence(now, _timezonePacific);

            Assert.True(next > now);
        }

        [Fact]
        public void GetNextOccurrence_ThreeDaySchedule_MultipleScheduleIterations()
        {
            // 11:59AM on Mondays, Tuesdays, Wednesdays, Thursdays and Fridays
            CronSchedule schedule = new CronSchedule("0 59 11 * * 1-5");

            DateTime nowUtc = TimeZoneInfo.ConvertTimeToUtc(new DateTime(2015, 5, 23, 9, 0, 0), _timezonePacific);

            TimeSpan expectedTime = new TimeSpan(11, 59, 0);
            for (int i = 1; i <= 5; i++)
            {
                DateTime nextOccurrenceUtc = schedule.GetNextOccurrence(nowUtc, _timezonePacific);
                DateTime nextOccurrence = TimeZoneInfo.ConvertTimeFromUtc(nextOccurrenceUtc, _timezonePacific);

                Assert.Equal((DayOfWeek)i, nextOccurrence.DayOfWeek);
                Assert.Equal(expectedTime, nextOccurrence.TimeOfDay);
                nowUtc = nextOccurrenceUtc + TimeSpan.FromSeconds(1);
            }
        }

        [Fact]
        public void ToString_ReturnsExpectedValue()
        {
            CronSchedule schedule = new CronSchedule("0 59 11 * * 1-5");
            Assert.Equal("Cron: '0 59 11 * * 1-5'", schedule.ToString());
        }
    }
}
