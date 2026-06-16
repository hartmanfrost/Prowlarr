using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Indexers.Definitions;

namespace NzbDrone.Core.Test.IndexerTests.RuTrackerTests
{
    [TestFixture]
    public class RuTrackerParserFixture
    {
        private static ICollection<IndexerCategory> AnimeCategories() =>
            new List<IndexerCategory> { NewznabStandardCategory.TVAnime };

        [TestCase("Kono Subarashii Sekai ni Shukufuku wo!: Kurenai Densetsu / Konosuba: Legend of Crimson [Movie] [RUS(ext), JAP] [2019, BDRip] [1080p]")]
        [TestCase("Gekijouban Kimetsu no Yaiba: Mugen Ressha-hen [Movie] [RUS] [2020, BDRemux 1080p]")]
        [TestCase("劇場版 この素晴らしい世界に祝福を！紅伝説 [2019, BDRip] [1080p]")]
        [TestCase("Меч Бессмертного / Mugen no Juunin: Immortal [Фильм] [RUS] [2019]")]
        public void should_tag_anime_film_as_movie(string title)
        {
            var categories = AnimeCategories();

            RuTrackerParser.AddAnimeMovieCategory(title, categories);

            categories.Should().Contain(NewznabStandardCategory.Movies);
            categories.Should().Contain(NewznabStandardCategory.TVAnime);
        }

        [TestCase("Наруто / Naruto [ТВ-2] [500 из 500] [RUS(int), JAP] [2007, BDRip 720p]")]
        [TestCase("Берсерк / Berserk [01-25 из 25] [RUS, JAP] [2016, WEB-DL 1080p]")]
        [TestCase("Ван-Пис / One Piece [1000+ из 1000+] [RUS] [1999, HDTV 720p]")]
        public void should_not_tag_anime_series_as_movie(string title)
        {
            var categories = AnimeCategories();

            RuTrackerParser.AddAnimeMovieCategory(title, categories);

            categories.Should().NotContain(NewznabStandardCategory.Movies);
        }

        [Test]
        public void should_not_tag_non_anime_release()
        {
            // A non-anime category release is already classified correctly by its forum; the
            // anime-film heuristic must not touch it even if a movie marker appears in the title.
            var categories = new List<IndexerCategory> { NewznabStandardCategory.TV };

            RuTrackerParser.AddAnimeMovieCategory("Some Drama / Драма [Movie] [2019, BDRemux 1080p]", categories);

            categories.Should().NotContain(NewznabStandardCategory.Movies);
        }
    }
}
