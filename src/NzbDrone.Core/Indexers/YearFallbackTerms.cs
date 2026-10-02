using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NzbDrone.Core.Indexers
{
    /// <summary>
    /// Radarr queries text-only trackers as "&lt;Title&gt; &lt;TMDB year&gt;", but the tracker topic is often tagged
    /// with a neighbouring year (festival premiere vs. release). Radarr accepts a release whose year is the movie
    /// Year or SecondaryYear, and the latter is normally one year off, so adjacent years are worth a retry.
    /// </summary>
    internal static class YearFallbackTerms
    {
        private static readonly Regex TrailingYearRegex = new Regex(@"^(?<title>.*\S)\s+(?<year>(?:19|20)\d{2})$", RegexOptions.Compiled);

        public static IEnumerable<string> Variants(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                yield break;
            }

            var match = TrailingYearRegex.Match(term.Trim());

            if (!match.Success)
            {
                yield break;
            }

            var title = match.Groups["title"].Value;
            var year = int.Parse(match.Groups["year"].Value);
            var maxYear = DateTime.UtcNow.Year + 1;

            foreach (var candidate in new[] { year - 1, year + 1 })
            {
                if (candidate is >= 1900 and <= 2099 && candidate <= maxYear)
                {
                    yield return $"{title} {candidate}";
                }
            }
        }
    }
}
