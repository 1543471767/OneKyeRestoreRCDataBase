using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastrure
{
    public static class TimeHelper
    {
        //DateTime转换为时间戳
        public static long GetTimeSpan(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (long)(time - startTime).TotalSeconds;
        }
        //timeSpan转换为DateTime
        public static DateTime TimeSpanToDateTime(long span)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            time = startTime.AddSeconds(span);
            return time;
        }
        public static DateTime TimeSpanToDateTime(TimeSpan ts)
        {
            //获取开始时间 
            DateTime start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            //注意13位是整数加上4个零 
            return start.Add(ts);
        }
    }
}
