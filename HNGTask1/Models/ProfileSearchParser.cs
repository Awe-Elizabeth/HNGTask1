namespace HNGTask1.Models
{ 
    public static class ProfileSearchParser
    {
        private static readonly Dictionary<string, string> CountryMap = new()
        {
                { "nigeria", "NG" },
                { "kenya", "KE" },
                { "angola", "AO" },
                { "tanzania", "TZ" },
                { "rwanda", "RW" },
                { "australia", "AU" },
                { "senegal", "SN" },
                { "united kingdom", "GB" },
                { "south africa", "ZA" },
                { "india", "IN" },
                { "mali", "ML" },
                { "somalia", "SO" },
                { "united states", "US" },
                { "uganda", "UG" },
                { "malawi", "MW" },
                { "Mozambique", "MZ" }
        };

        public static ProfileSearchFilter Parse(string query)
        {
            var filter = new ProfileSearchFilter();

            if (string.IsNullOrWhiteSpace(query))
                return filter;

            var q = query.ToLower();

            /* GENDER */
            if (q.Contains("male"))
                filter.Gender = "male";

            if (q.Contains("female"))
                filter.Gender = "female";
            if (q.Contains("male and female"))
                filter.Gender = null;

            /* AGE GROUPS */
            if (q.Contains("child"))
                filter.AgeGroup = "child";

            if (q.Contains("teen"))
                filter.AgeGroup = "teenager";

            if (q.Contains("adult"))
                filter.AgeGroup = "adult";

            if (q.Contains("senior"))
                filter.AgeGroup = "senior";

            /* CUSTOM PHRASES */
            if (q.Contains("young"))
            {
                filter.MinAge = 16;
                filter.MaxAge = 24;
            }
            if (q.Contains("old"))
            {
                filter.MinAge = 24;
            }

            /* AGE CONDITIONS */
            var aboveMatch = System.Text.RegularExpressions.Regex.Match(q, @"above\s+(\d+)");
            if (aboveMatch.Success)
            {
                filter.MinAge = int.Parse(aboveMatch.Groups[1].Value);
            }

            var belowMatch = System.Text.RegularExpressions.Regex.Match(q, @"below\s+(\d+)");
            if (belowMatch.Success)
            {
                filter.MaxAge = int.Parse(belowMatch.Groups[1].Value);
            }

            /* COUNTRY */
            foreach (var country in CountryMap)
            {
                if (q.Contains(country.Key))
                {
                    filter.CountryId = country.Value;
                    break;
                }
            }

            return filter;
        }
    }
}

