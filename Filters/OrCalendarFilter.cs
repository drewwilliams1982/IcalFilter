namespace IcalFilter.Filters
{
    using System.Collections.Generic;
    using System.Linq;
    using Ical.Net.CalendarComponents;

    public class OrCalendarFilter : ICalendarFilter
    {
        private IEnumerable<ICalendarFilter> _filters;

        public OrCalendarFilter(IEnumerable<ICalendarFilter> filters)
        {
            this._filters = filters;
        }

        public bool IsMatch(CalendarEvent e)
        {
            return this._filters.ToList().Any(f => f.IsMatch(e));
        }
    }
}