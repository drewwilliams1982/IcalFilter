namespace IcalFilter.Filters
{
    using System;
    using System.Linq;
    using Ical.Net.CalendarComponents;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class AndCalendarFilter : ICalendarFilter
    {
        private ICalendarFilter a;
        private ICalendarFilter b;

        public AndCalendarFilter(ICalendarFilter a, ICalendarFilter b)
        {
            this.a = a;
            this.b = b;
        }

        public bool IsMatch(CalendarEvent e)
        {
            return a.IsMatch(e) && b.IsMatch(e);
        }
    }
}