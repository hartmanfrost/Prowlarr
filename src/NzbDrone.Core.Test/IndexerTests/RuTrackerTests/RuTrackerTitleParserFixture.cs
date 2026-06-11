using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Indexers.Definitions;

namespace NzbDrone.Core.Test.IndexerTests.RuTrackerTests
{
    [TestFixture]
    public class RuTrackerTitleParserFixture
    {
        private static readonly ICollection<IndexerCategory> TvCategories = new List<IndexerCategory> { NewznabStandardCategory.TVHD };

        private readonly RuTrackerTitleParser _titleParser = new();

        [Test]
        public void should_add_rus_to_title_when_missing()
        {
            _titleParser.Parse("The Irishman (2019) WEB-DL 1080p", TvCategories, stripCyrillicLetters: false, addRussianToTitle: true)
                .Should().Be("The Irishman (2019) WEB-DL 1080p RUS");
        }

        [Test]
        public void should_not_duplicate_rus_in_title()
        {
            _titleParser.Parse("The Irishman (2019) WEB-DL 1080p RUS", TvCategories, stripCyrillicLetters: false, addRussianToTitle: true)
                .Should().Be("The Irishman (2019) WEB-DL 1080p RUS");
        }
    }
}
