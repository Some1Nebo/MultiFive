using System;

namespace MultiFive.Web.Infrastructure
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime Today { get; }
    }
}