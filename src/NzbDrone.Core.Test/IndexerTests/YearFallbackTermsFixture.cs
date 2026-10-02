using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Indexers.Definitions;
using NzbDrone.Core.IndexerSearch.Definitions;

namespace NzbDrone.Core.Test.IndexerTests
{
    [TestFixture]
    public class YearFallbackTermsFixture
    {
        [TestCase("The Gentlemen 2020", "The Gentlemen 2019", "The Gentlemen 2021")]
        [TestCase("1917 2019", "1917 2018", "1917 2020")]
        [TestCase("2012 2009", "2012 2008", "2012 2010")]
        [TestCase("Blade Runner 2049 2017", "Blade Runner 2049 2016", "Blade Runner 2049 2018")]
        [TestCase("  The Gentlemen 2020  ", "The Gentlemen 2019", "The Gentlemen 2021")]
        public void should_return_adjacent_years(string term, string previous, string next)
        {
            YearFallbackTerms.Variants(term).Should().Equal(previous, next);
        }

        [TestCase("1917")]
        [TestCase("The Gentlemen")]
        [TestCase("The Gentlemen (2019)")]
        [TestCase("Title 1899")]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void should_not_return_fallback(string term)
        {
            YearFallbackTerms.Variants(term).Should().BeEmpty();
        }

        [Test]
        public void should_skip_years_out_of_range()
        {
            YearFallbackTerms.Variants("Title 1900").Should().Equal("Title 1901");
            YearFallbackTerms.Variants($"Title {DateTime.UtcNow.Year + 1}").Should().Equal($"Title {DateTime.UtcNow.Year}");
        }
    }

    [TestFixture]
    public class YearFallbackRequestGeneratorFixture
    {
        private static string Term(IndexerPageableRequestChain chain, int tier)
        {
            var url = chain.GetTier(tier).Single().First().HttpRequest.Url.FullUri;
            return Uri.UnescapeDataString(url).Replace("+", " ");
        }

        private static RuTrackerRequestGenerator RuGenerator() => new RuTrackerRequestGenerator(new RuTrackerSettings { BaseUrl = "https://rutracker.org/" }, new IndexerCapabilities());

        private static TolokaRequestGenerator TolGenerator() => new TolokaRequestGenerator(new TolokaSettings { BaseUrl = "https://toloka.to/" }, new IndexerCapabilities());

        [Test]
        public void rutracker_movie_should_add_fallback_tiers()
        {
            var chain = RuGenerator().GetSearchRequests(new MovieSearchCriteria { SearchTerm = "The Gentlemen 2020" });

            chain.Tiers.Should().Be(3);
            Term(chain, 0).Should().Contain("The%Gentlemen%2020");
            Term(chain, 1).Should().Contain("The%Gentlemen%2019");
            Term(chain, 2).Should().Contain("The%Gentlemen%2021");
        }

        [Test]
        public void rutracker_movie_without_year_should_have_single_tier()
        {
            RuGenerator().GetSearchRequests(new MovieSearchCriteria { SearchTerm = "1917" }).Tiers.Should().Be(1);
        }

        [Test]
        public void rutracker_tv_should_not_add_fallback()
        {
            RuGenerator().GetSearchRequests(new TvSearchCriteria { SearchTerm = "Show 2019" }).Tiers.Should().Be(1);
        }

        [Test]
        public void toloka_movie_should_add_fallback_tiers()
        {
            var chain = TolGenerator().GetSearchRequests(new MovieSearchCriteria { SearchTerm = "The Gentlemen 2020" });

            chain.Tiers.Should().Be(3);
            Term(chain, 0).Should().Contain("The Gentlemen 2020");
            Term(chain, 1).Should().Contain("The Gentlemen 2019");
            Term(chain, 2).Should().Contain("The Gentlemen 2021");
        }

        [Test]
        public void toloka_tv_should_not_add_fallback()
        {
            TolGenerator().GetSearchRequests(new TvSearchCriteria { SearchTerm = "Show 2019" }).Tiers.Should().Be(1);
        }
    }
}
