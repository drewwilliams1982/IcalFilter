namespace IcalFilter.Filters
{
    using Ical.Net.CalendarComponents;

    public interface ICalendarFilter
    {
        bool IsMatch(CalendarEvent e);
    }
}