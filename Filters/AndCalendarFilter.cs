namespace IcalFilter.Filters
{
    using System.Collections.Generic;
    using System.Linq;
    using Ical.Net.CalendarComponents;

    public class AndCalendarFilter : ICalendarFilter
    {
        private IEnumerable<ICalendarFilter> _filters;

        public AndCalendarFilter(IEnumerable<ICalendarFilter> filters)
        {
            this._filters = filters;
        }

        public bool IsMatch(CalendarEvent e)
        {
            return this._filters.ToList().All(f => f.IsMatch(e));
        }
    }
}