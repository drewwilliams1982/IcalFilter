namespace IcalFilter.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ical.Net.CalendarComponents;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class CalendarFilterFactory
    {
        /*
            Search for "ZZZ" Or "XXX && YYY"
            
            {
                "Or": [
                    { "Summary": "ZZZ", "Match": "contains" },
                    {
                        "And": [
                            { "Summary": "XXX", "Match": "contains" },
                            { "Summary": "YYY", "Match": "contains" }
                        ]
                    }
                ]
            }

        */
        public static ICalendarFilter GetFilter(string rules)
        {
            dynamic ruleset = JsonConvert.DeserializeObject(rules);
            return GetFilterRecusive(ruleset);
        }

        private static List<ICalendarFilter> GetFilterArray(dynamic rules)
        {
            var retval = new List<ICalendarFilter>();
            var lst = new List<dynamic>(rules);
            foreach(var l in lst)
            {
                retval.Add(GetFilterRecusive(l));
            }

            return retval;
        }

        private static ICalendarFilter GetFilterRecusive(dynamic ruleset)
        {
            if(HasProperty(ruleset, "Or"))
            {
                return new OrCalendarFilter(GetFilterArray(ruleset.Or));
            }

            if(HasProperty(ruleset, "And"))
            {
                return new AndCalendarFilter(GetFilterArray(ruleset.And));
            }

            if(HasProperty(ruleset, "Summary"))
            {
                var match = "contains";
                if(HasProperty(ruleset, "Match"))
                {
                    match = (string)ruleset.Match;
                }
                return new SummaryCalendarFilter((string)ruleset.Summary, match);
            }

            throw new Exception("Invalid rule");
        }


        private static bool HasProperty(JObject obj, string op)
        {
            return obj.Properties().Any(p => p.Name == op);
        }
    }
}