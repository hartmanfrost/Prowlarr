using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Indexers.Definitions;

namespace NzbDrone.Core.Test.IndexerTests.TolokaTests
{
    [TestFixture]
    public class TolokaParserFixture
    {
        private static ICollection<IndexerCategory> AnimeCategories() =>
            new List<IndexerCategory> { NewznabStandardCategory.TVAnime };

        [TestCase("Kono Subarashii Sekai ni Shukufuku wo! Movie: Kurenai Densetsu (2019) BDRip 1080p")]
        [TestCase("Konosuba: Legend of Crimson [Movie] [RUS(ext), JAP] [2019, BDRip] [1080p]")]
        [TestCase("Gekijouban Kimetsu no Yaiba: Mugen Ressha-hen (2020) BDRemux 1080p")]
        [TestCase("Gekijou-ban Violet Evergarden (2020)")]
        [TestCase("劇場版 この素晴らしい世界に祝福を！紅伝説 (2019)")]
        [TestCase("Аніме Фільм: Назва (2019) BDRip 1080p")]
        public void should_tag_anime_film_as_movie(string title)
        {
            var categories = AnimeCategories();

            TolokaParser.AddAnimeMovieCategory(title, categories);

            categories.Should().Contain(NewznabStandardCategory.Movies);
            categories.Should().Contain(NewznabStandardCategory.TVAnime);
        }

        [TestCase("Spy x Family (Сезон 1) / Spy x Family (2022) WEB-DL 1080p (S1)")]
        [TestCase("Berserk (2016) WEB-DL 1080p Ukr/Jap")]
        [TestCase("Naruto: Shippuuden [TV] [154-500 of 500]")]
        [TestCase("Overlord II (2018) WEBRip 1080p (S2)")]
        public void should_not_tag_anime_series_as_movie(string title)
        {
            var categories = AnimeCategories();

            TolokaParser.AddAnimeMovieCategory(title, categories);

            categories.Should().NotContain(NewznabStandardCategory.Movies);
        }

        [Test]
        public void should_not_tag_non_anime_release()
        {
            // A non-anime category release is already classified correctly by its forum; the
            // anime-film heuristic must not touch it even if "Movie" appears in the title.
            var categories = new List<IndexerCategory> { NewznabStandardCategory.TV };

            TolokaParser.AddAnimeMovieCategory("Some Show Movie Night (2019)", categories);

            categories.Should().NotContain(NewznabStandardCategory.Movies);
        }

        [Test]
        public void should_not_duplicate_movies_category()
        {
            var categories = new List<IndexerCategory> { NewznabStandardCategory.TVAnime, NewznabStandardCategory.Movies };

            TolokaParser.AddAnimeMovieCategory("Some Anime Movie (2019)", categories);

            categories.Should().HaveCount(2);
        }
    }
}
