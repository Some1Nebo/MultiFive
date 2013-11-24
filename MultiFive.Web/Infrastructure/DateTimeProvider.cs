using System;

namespace MultiFive.Web.Infrastructure
{
    public class DateTimeProvider: IDateTimeProvider
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public DateTime Today
        {
            get { return DateTime.Today; }
        }
    }
}