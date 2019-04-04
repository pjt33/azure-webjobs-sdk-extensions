// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Extensions.Tests.Common;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Xunit;

namespace Microsoft.Azure.WebJobs.Extensions.Tests.Timers
{
    public class TimerInfoTests : IClassFixture<CultureFixture.EnUs>
    {
        private static readonly TimeZoneInfo _timezonePacific = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

        [Fact]
        public void ScheduleStatus_ReturnsExpectedValue()
        {
            TimerSchedule schedule = new ConstantSchedule(TimeSpan.FromDays(1));
            TimerInfo timerInfo = new TimerInfo(_timezonePacific, schedule, null);
            Assert.Null(timerInfo.ScheduleStatus);

            ScheduleStatus scheduleStatus = new ScheduleStatus();
            timerInfo = new TimerInfo(_timezonePacific, schedule, scheduleStatus);
            Assert.Same(scheduleStatus, timerInfo.ScheduleStatus);
        }

        [Fact]
        public void FormatNextOccurrences_ReturnsExpectedString()
        {
            DateTime now = new DateTime(2015, 9, 16, 10, 30, 00, DateTimeKind.Utc);
            TimerInfo timerInfo = new TimerInfo(_timezonePacific, new CronSchedule("0 0 * * * *"), null);
            string result = timerInfo.FormatNextOccurrences(10, now);

            string expected =
                "The next 10 occurrences of the schedule will be:\r\n" +
                "9/16/2015 11:00:00 AM\r\n" +
                "9/16/2015 12:00:00 PM\r\n" +
                "9/16/2015 1:00:00 PM\r\n" +
                "9/16/2015 2:00:00 PM\r\n" +
                "9/16/2015 3:00:00 PM\r\n" +
                "9/16/2015 4:00:00 PM\r\n" +
                "9/16/2015 5:00:00 PM\r\n" +
                "9/16/2015 6:00:00 PM\r\n" +
                "9/16/2015 7:00:00 PM\r\n" +
                "9/16/2015 8:00:00 PM\r\n";
            Assert.Equal(expected, result);
        }
    }
}
