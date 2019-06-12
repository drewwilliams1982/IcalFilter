namespace IcalFilter.Filters
{
    using System;
    using System.Linq;
    using Ical.Net.CalendarComponents;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class SummaryCalendarFilter : ICalendarFilter
    {
        private string text;
        private string matchFlavour;

        public SummaryCalendarFilter(string text, string matchFlavour)
        {
            this.text = text;
            this.matchFlavour = matchFlavour;
        }
        public bool IsMatch(CalendarEvent e)
        {
            bool result;
            if(this.matchFlavour.ToLower() == "contains")
                result = e.Summary.Contains(text);
            else if(this.matchFlavour.ToLower() == "equals")
                result = e.Summary == text;
            else if(this.matchFlavour.ToLower() == "starts")
                result = e.Summary.StartsWith(text);
            else if(this.matchFlavour.ToLower() == "ends")
                result = e.Summary.EndsWith(text);
            else
                result = false;

            return result;
        }
    }
}