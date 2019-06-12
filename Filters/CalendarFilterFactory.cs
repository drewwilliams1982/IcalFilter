namespace IcalFilter.Filters
{
    using System;
    using System.Linq;
    using Ical.Net.CalendarComponents;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class CalendarFilterFactory
    {
        /*
            Search for XXX and YYY in Summary:
            { 
                "And": { 
                    "A": { "Summary": ... }, 
                    "B": { "Summary": ... } 
                } 
            }

            { "Or": { "A": { "Summary": ... }, "B": { "Summary": ... } } }

            Search for "ZZZ" Or "XXX && YYY"
            {
                "Or" {
                    "A": {
                        "And": { 
                            "A": { "Summary": ... }, 
                            "B": { "Summary": ... } 
                        }
                    },
                    "B": { "Summary": ... }
                }
            }

        */
        public static ICalendarFilter GetFilter(string rules)
        {
            dynamic ruleset = JsonConvert.DeserializeObject(rules);
            return GetFilterRecusive(ruleset);
        }

        private static ICalendarFilter GetFilterRecusive(dynamic ruleset)
        {
            if(HasProperty(ruleset, "Or"))
            {
                var orA = ruleset.Or.A;
                var orB = ruleset.Or.B;
                return new OrCalendarFilter(GetFilterRecusive(orA), GetFilterRecusive(orB));
            }

            if(HasProperty(ruleset, "And"))
            {
                var andA = ruleset.And.A;
                var andB = ruleset.And.B;
                return new AndCalendarFilter(GetFilterRecusive(andA), GetFilterRecusive(andB));
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