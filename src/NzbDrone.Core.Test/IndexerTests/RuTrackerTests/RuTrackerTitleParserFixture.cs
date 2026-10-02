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

        private static readonly ICollection<IndexerCategory> AnimeCategories = new List<IndexerCategory> { NewznabStandardCategory.TVAnime };

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

        [TestCase("Демоны старшей школы (ТВ-3) / High School DxD Born / Highschool DxD Born / Старшая школа ДхД рождение: Демоны против падших [TV+Special] [12+6 из 12+6] [RUS(ext), JAP+Sub] [2015, комедия, фэнтези, этти, гарем, BDRip] [1080p]", "High School DxD Born (S3) / Highschool DxD Born / : [TV+SP] [RUS(ext), JAP] [2015, BDRip] [1080p]")]
        [TestCase("Демоны старшей школы (ТВ-2) / High School DxD New / Highschool DxD New / Старшая школа ДхД по-новому: Демоны против падших [TV] [12 из 12] [RUS(ext), JAP+Sub] [2013, комедия, фэнтези, этти, гарем, BDRip] [1080p]", "High School DxD New (S2) / Highschool DxD New / -: [TV] [RUS(ext), JAP] [2013, BDRip] [1080p]")]
        [TestCase("Демоны старшей школы (ТВ-4) / High School DxD Hero / Highschool DxD Hero / Старшая школа ДхД герой: Демоны против падших [TV] [13 из 13] [RUS(int), JAP+Sub] [2018, комедия, фэнтези, этти, HDTVRip] [HWP]", "High School DxD Hero (S4) / Highschool DxD Hero / : [TV] [RUS(int), JAP] [2018, HDTV] [HWP]")]
        [TestCase("Демоны старшей школы (ТВ-1) / High School DxD / Highschool DxD / Старшая школа ДхД: Демоны против падших [TV+Special+OVA] [12+6+1 из 12+6+1] [RUS(ext), JAP+Sub] [2012, приключения, мистика, этти, BDRip] [1080p]", "High School DxD (S1) / Highschool DxD / : [TV+SP+OVA] [RUS(ext), JAP] [2012, BDRip] [1080p]")]
        [TestCase("Демоны старшей школы (ТВ-3) / High School DxD Born / Highschool DxD Born / Старшая школа ДхД рождение: Демоны против падших [TV+Special] [12+3 из 12+6] [JAP+Sub] & [12+0 из 12+6] [RUS(ext)] [2015, приключения, комедия, этти, BDRip] [1080p]", "High School DxD Born (S3) / Highschool DxD Born / : [TV+SP] [JAP] & [RUS(ext)] [2015, BDRip] [1080p]")]
        [TestCase("Фрирен, провожающая в последний путь (ТВ-2) / Sousou no Frieren 2nd Season / Frieren: Beyond Journey's End Season 2 [TV] [08 из 10] [RUS(ext), JAP+Sub] [2026, приключения, фэнтези, WEB-DL] [1080p]", "Sousou no Frieren 2nd Season (S02E01-08) / Frieren: Beyond Journey's End Season 2 [TV] [RUS(ext), JAP] [2026, WEB-DL] [1080p]")]
        [TestCase("Фрирен, провожающая в последний путь (ТВ-2) / Sousou no Frieren 2nd Season [TV] [01 из XX] [RUS(int), JAP+Sub] [2026, приключения, фэнтези, WEBRip] [1080p]", "Sousou no Frieren 2nd Season (S02E01) [TV] [RUS(int), JAP] [2026, WEBRip] [1080p]")]
        [TestCase("Фрирен, провожающая в последний путь (ТВ-2) / Sousou no Frieren 2nd Season [TV] [05-08 из 10] [RUS(ext), JAP+Sub] [2026, приключения, фэнтези, WEB-DL] [1080p]", "Sousou no Frieren 2nd Season (S02E05-08) [TV] [RUS(ext), JAP] [2026, WEB-DL] [1080p]")]
        [TestCase("Наруто / Naruto [ТВ-2] [500 из 500] [RUS(int), JAP] [2007, BDRip 720p]", "Naruto [S2] [RUS(int), JAP] [2007, BDRip 720p]")]
        public void should_normalize_anime_season_for_sonarr(string title, string expected)
        {
            _titleParser.Parse(title, AnimeCategories, stripCyrillicLetters: true, addRussianToTitle: true)
                .Should().Be(expected);
        }

        [TestCase("Демоны старшей школы / High School DxD [Янагисава Тэцуя][TV][1 сезон][12 из 12 / 6 из 6][2012, приключения, мистика, этти, комедия, HDRip][Субтитры]", "High School DxD [TV][1 ][12 12 / 6 6][2012, HDRip] RUS")]
        [TestCase("Демоны старшей школы (OBA) / High School DxD OAD / Highschool DxD OAD / Старшая школа ДхД: Демоны против падших [OVA] [4 из 4] [RUS(ext), JAP+Sub] [2012-2015, приключения, мистика, этти, комедия, демоны, BDRemux] [1080p]", "(OBA) / High School DxD OAD / Highschool DxD OAD / : [OVA] E4 of 4 [RUS(ext), JAP] [2012-2015, BDRemux] [1080p]")]
        public void should_keep_episode_count_for_anime_without_season_token(string title, string expected)
        {
            _titleParser.Parse(title, AnimeCategories, stripCyrillicLetters: true, addRussianToTitle: true)
                .Should().Be(expected);
        }
    }
}
