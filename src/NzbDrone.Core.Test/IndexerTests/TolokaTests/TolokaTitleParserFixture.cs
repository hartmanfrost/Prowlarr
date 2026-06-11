using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Indexers.Definitions;

namespace NzbDrone.Core.Test.IndexerTests.TolokaTests
{
    [TestFixture]
    public class TolokaTitleParserFixture
    {
        private static readonly ICollection<IndexerCategory> TvCategories = new List<IndexerCategory> { NewznabStandardCategory.TVAnime };

        private readonly TolokaTitleParser _titleParser = new();

        [Test]
        public void should_relocate_stranded_season_token_after_stripping_apostrophe_title()
        {
            // "Сім'я шпигуна" leaves a stranded "' (S1) / " prefix after Cyrillic stripping
            _titleParser.Parse("Сім'я шпигуна (Сезон 1) / Spy x Family (2022) WEB-DL 1080p H.265 Ukr/Jap | sub Ukr", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Spy x Family (2022) WEB-DL 1080p H.265 Ukr/Jap | sub Ukr (S1)");
        }

        [Test]
        public void should_relocate_stranded_season_token_after_stripping_title_with_digits()
        {
            // "Володар 2" leaves a stranded "2 (S2) / " prefix after Cyrillic stripping
            _titleParser.Parse("Володар 2 (Сезон 2) / Overlord II (2018) WEBRip 1080p H.265 Ukr/Jap | Sub Ukr", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Overlord II (2018) WEBRip 1080p H.265 Ukr/Jap | Sub Ukr (S2)");
        }

        [Test]
        public void should_strip_plain_cyrillic_title_without_relocation()
        {
            _titleParser.Parse("Берсерк / Berserk (2016) WEB-DL 1080p Ukr/Jap", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Berserk (2016) WEB-DL 1080p Ukr/Jap");
        }
    }
}
