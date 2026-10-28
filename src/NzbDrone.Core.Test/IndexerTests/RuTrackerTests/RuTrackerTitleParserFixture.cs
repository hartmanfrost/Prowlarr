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

        private static readonly ICollection<IndexerCategory> AnimeMovieCategories = new List<IndexerCategory> { NewznabStandardCategory.TVAnime, NewznabStandardCategory.Movies };

        private static readonly ICollection<IndexerCategory> MovieCategories = new List<IndexerCategory> { NewznabStandardCategory.MoviesHD };

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

        [TestCase("Демоны старшей школы (ТВ-3) / High School DxD Born / Highschool DxD Born / Старшая школа ДхД рождение: Демоны против падших [TV+Special] [12+6 из 12+6] [RUS(ext), JAP+Sub] [2015, комедия, фэнтези, этти, гарем, BDRip] [1080p]", "High School DxD Born (S3) / Highschool DxD Born [TV+SP] [RUS(ext), JAP] [2015, BDRip] [1080p]")]
        [TestCase("Демоны старшей школы (ТВ-2) / High School DxD New / Highschool DxD New / Старшая школа ДхД по-новому: Демоны против падших [TV] [12 из 12] [RUS(ext), JAP+Sub] [2013, комедия, фэнтези, этти, гарем, BDRip] [1080p]", "High School DxD New (S2) / Highschool DxD New [TV] [RUS(ext), JAP] [2013, BDRip] [1080p]")]
        [TestCase("Демоны старшей школы (ТВ-4) / High School DxD Hero / Highschool DxD Hero / Старшая школа ДхД герой: Демоны против падших [TV] [13 из 13] [RUS(int), JAP+Sub] [2018, комедия, фэнтези, этти, HDTVRip] [HWP]", "High School DxD Hero (S4) / Highschool DxD Hero [TV] [RUS(int), JAP] [2018, HDTV] [HWP]")]
        [TestCase("Демоны старшей школы (ТВ-1) / High School DxD / Highschool DxD / Старшая школа ДхД: Демоны против падших [TV+Special+OVA] [12+6+1 из 12+6+1] [RUS(ext), JAP+Sub] [2012, приключения, мистика, этти, BDRip] [1080p]", "High School DxD (S1) / Highschool DxD [TV+SP+OVA] [RUS(ext), JAP] [2012, BDRip] [1080p]")]
        [TestCase("Демоны старшей школы (ТВ-3) / High School DxD Born / Highschool DxD Born / Старшая школа ДхД рождение: Демоны против падших [TV+Special] [12+3 из 12+6] [JAP+Sub] & [12+0 из 12+6] [RUS(ext)] [2015, приключения, комедия, этти, BDRip] [1080p]", "High School DxD Born (S3) / Highschool DxD Born [TV+SP] [JAP] & [RUS(ext)] [2015, BDRip] [1080p]")]
        [TestCase("Фрирен, провожающая в последний путь (ТВ-2) / Sousou no Frieren 2nd Season / Frieren: Beyond Journey's End Season 2 [TV] [08 из 10] [RUS(ext), JAP+Sub] [2026, приключения, фэнтези, WEB-DL] [1080p]", "Sousou no Frieren 2nd Season (S02E01-08) / Frieren: Beyond Journey's End Season 2 [TV] [RUS(ext), JAP] [2026, WEB-DL] [1080p]")]
        [TestCase("Фрирен, провожающая в последний путь (ТВ-2) / Sousou no Frieren 2nd Season [TV] [01 из XX] [RUS(int), JAP+Sub] [2026, приключения, фэнтези, WEBRip] [1080p]", "Sousou no Frieren 2nd Season (S02E01) [TV] [RUS(int), JAP] [2026, WEBRip] [1080p]")]
        [TestCase("Фрирен, провожающая в последний путь (ТВ-2) / Sousou no Frieren 2nd Season [TV] [05-08 из 10] [RUS(ext), JAP+Sub] [2026, приключения, фэнтези, WEB-DL] [1080p]", "Sousou no Frieren 2nd Season (S02E05-08) [TV] [RUS(ext), JAP] [2026, WEB-DL] [1080p]")]
        [TestCase("Сэкирэй (ТВ-2): Честная битва / Sekirei: Pure Engagement [TV+Special] [13+1 из 13+1] [RUS(ext), JAP+Sub] [2010, приключения, комедия, этти, BDRip] [1080p]", "Sekirei: Pure Engagement (S2) [TV+SP] [RUS(ext), JAP] [2010, BDRip] [1080p]")]
        [TestCase("Сэкирэй [ТВ-2]: Честная битва / Sekirei: Pure Engagement [TV+Special] [13+1 из 13+1] [RUS(ext),JAP+Sub] [2010, приключения, комедия, этти, BDRip] [720p]", "Sekirei: Pure Engagement (S2) [TV+SP] [RUS(ext),JAP] [2010, BDRip] [720p]")]
        [TestCase("Сэкирэй (ТВ-2) / Sekirei: Pure Engagement [TV+Special] [13+1 из 13+1] [RUS(int),JAP] [2010, приключения, комедия, этти, BDRip]", "Sekirei: Pure Engagement (S2) [TV+SP] [RUS(int),JAP] [2010, BDRip]")]
        [TestCase("Наруто / Naruto [ТВ-2] [500 из 500] [RUS(int), JAP] [2007, BDRip 720p]", "Naruto [S2] [RUS(int), JAP] [2007, BDRip 720p]")]
        public void should_normalize_anime_season_for_sonarr(string title, string expected)
        {
            _titleParser.Parse(title, AnimeCategories, stripCyrillicLetters: true, addRussianToTitle: true)
                .Should().Be(expected);
        }

        [TestCase("Стальная тревога! / Fullmetal Panic! / Full Metal Panic! / FMP [TV] [24 из 24] [RUS(int), JAP] [2002, приключения, комедия, романтика, меха, BDRip] [1080p]", "Fullmetal Panic! (S1) / Full Metal Panic! / FMP [TV] [RUS(int), JAP] [2002, BDRip] [1080p]")]
        [TestCase("Спидграфер / Speed Grapher [TV] [24 из 24] [RUS(int),JAP]", "Speed Grapher (S1) [TV] [RUS(int),JAP]")]
        [TestCase("Сильнейшая ведьма в мире стартовала / Sekai Saikyou no Majo, Hajimemashita / The World's Strongest Witch [TV] [1-3 из 12] [JAP] & [1 из 12] [RUS(ext)] [2026, комедия, фэнтези, WEB-DL] [1080p]", "Sekai Saikyou no Majo, Hajimemashita (S01E01-03) / The World's Strongest Witch [TV] [JAP] & [RUS(ext)] [2026, WEB-DL] [1080p]")]
        [TestCase("Некий ITEM тёмной стороны / Toaru Anbu no ITEM / A Certain Dark ITEM [TV] [1-2 из 12] [JAP] [2026, экшен, фантастика, приключения, WEB-DL] [1080p]", "Toaru Anbu no ITEM (S01E01-02) / A Certain Dark ITEM [TV] [JAP] [2026, WEB-DL] [1080p]")]
        [TestCase("Одолженная мной магия будет принудительно изъята посредством возобновляемого кредита / Kashita Maryoku wa \"Revo-barai\" de Kyousei Choushuu [TV] [01-03 из ХХ] [RUS(int), JAP] [2026, Экшен, Фэнтези, WEB-DL] [1080p]", "Kashita Maryoku wa \"Revo-barai\" de Kyousei Choushuu (S01E01-03) [TV] [RUS(int), JAP] [2026, WEB-DL] [1080p]")]
        [TestCase("Школа Мертвецов / Gakuen Mokushiroku: High School Of the Dead / Highschool of the Dead [TV+Special] [12+1 из 12+1] [RUS(int), JAP] [2010, BDRip]", "Gakuen Mokushiroku: High School Of the Dead (S1) / Highschool of the Dead [TV+SP] [RUS(int), JAP] [2010, BDRip]")]
        public void should_treat_anime_tv_topic_without_season_token_as_first_season(string title, string expected)
        {
            _titleParser.Parse(title, AnimeCategories, stripCyrillicLetters: true, addRussianToTitle: true)
                .Should().Be(expected);
        }

        // "Invisible Victory" is season 4 but Sonarr reads "Full Metal Panic! Invisible Victory (S1)" as season 1 of the
        // series, so a title with a sequel marker keeps the old episode count instead of getting a guessed season.
        [TestCase("Стальная тревога! IV / Fullmetal Panic! Invisible Victory / Full Metal Panic! Invisible Victory [TV] [12 из 12] [JAP] [2018, приключения, BDRip] [1080p]", "Fullmetal Panic! Invisible Victory / Full Metal Panic! Invisible Victory [TV] E12 of 12 [JAP] [2018, BDRip] [1080p]")]
        [TestCase("Стальная тревога! Новый рейд / Fullmetal Panic! The Second Raid / Full Metal Panic! TSR The Second Raid [TV] [13 из 13] [RUS(int), JAP] [2005, приключения, BDRip] [720p]", "Fullmetal Panic! The Second Raid / Full Metal Panic! TSR The Second Raid [TV] E13 of 13 [RUS(int), JAP] [2005, BDRip] [720p]")]
        [TestCase("Мастера меча онлайн: Алисизация / Sword Art Online: Alicization [TV] [24 из 24] [RUS(int), JAP] [2018, экшен, BDRip]", "Sword Art Online: Alicization (S1) [TV] [RUS(int), JAP] [2018, BDRip]")]
        [TestCase("Нечто / Something Part 2 [TV] [12 из 12] [RUS(int), JAP] [2021, BDRip]", "Something Part 2 [TV] E12 of 12 [RUS(int), JAP] [2021, BDRip]")]
        [TestCase("Нечто / Something 2nd Season Part 2 [TV] [12 из 12] [RUS(int), JAP] [2021, BDRip]", "Something 2nd Season Part 2 [TV] E12 of 12 [RUS(int), JAP] [2021, BDRip]")]
        [TestCase("Школа Мертвецов [TV] [12 из 12] [RUS(int), JAP] [2010, боевик, ужасы, этти, BDRip] [720p] [HD]", "[TV] E12 of 12 [RUS(int), JAP] [2010, BDRip] [720p] [HD]")]
        public void should_not_guess_first_season_for_sequels_or_untitled_topics(string title, string expected)
        {
            _titleParser.Parse(title, AnimeCategories, stripCyrillicLetters: true, addRussianToTitle: true)
                .Should().Be(expected);
        }

        // A count-only topic ("[14 из 14]") has no season token, so it used to come out as "E14 of 14", which Sonarr reads as the
        // single episode 14. The season is taken from a number the latin names state outright.
        [TestCase("Мастера Меча Онлайн: Алисизация - Война Андерворлда 2 / Sword Art Online: Alicization - War of Underworld 2nd Season [TV] [11 из 11] [RUS(int), JAP] [2020, экшен, BDRip]", "Sword Art Online: Alicization - War of Underworld 2nd Season (S2) [TV] [RUS(int), JAP] [2020, BDRip]")]
        [TestCase("Реинкарнация безработного III / Mushoku Tensei III: Isekai Ittara Honki Dasu / Mushoku Tensei: Jobless Reincarnation Season 3 [TV] [14 из 14] [RUS(int), JAP] [2026, WEB-DL] [1080p]", "Mushoku Tensei III: Isekai Ittara Honki Dasu (S3) / Mushoku Tensei: Jobless Reincarnation Season 3 [TV] [RUS(int), JAP] [2026, WEB-DL] [1080p]")]
        [TestCase("Реинкарнация безработного III / Mushoku Tensei III: Isekai Ittara Honki Dasu [TV] [01-11 из 14] [RUS(int), JAP] [2026, WEB-DL] [1080p]", "Mushoku Tensei III: Isekai Ittara Honki Dasu (S03E01-11) [TV] [RUS(int), JAP] [2026, WEB-DL] [1080p]")]
        [TestCase("Реинкарнация безработного II / Mushoku Tensei II: Isekai Ittara Honki Dasu [TV+Special] [12+1 из 12+1] [RUS(int), JAP] [2023, BDRip] [1080p]", "Mushoku Tensei II: Isekai Ittara Honki Dasu (S2) [TV+SP] [RUS(int), JAP] [2023, BDRip] [1080p]")]
        [TestCase("Жизнь в альтернативном мире с нуля 2 / Re:Zero kara Hajimeru Isekai Seikatsu 2 / Re:Zero - Starting Life in Another World 2nd Season [TV] [13 из 13] [RUS(ext), JAP] [2020, BDRip] [1080p]", "Re:Zero kara Hajimeru Isekai Seikatsu 2 (S2) / Re:Zero - Starting Life in Another World 2nd Season [TV] [RUS(ext), JAP] [2020, BDRip] [1080p]")]
        public void should_take_season_of_count_only_topic_from_latin_name(string title, string expected)
        {
            _titleParser.Parse(title, AnimeCategories, stripCyrillicLetters: true, addRussianToTitle: true)
                .Should().Be(expected);
        }

        [TestCase("Некий ITEM тёмной стороны / Toaru Anbu no ITEM / A Certain Dark ITEM [TV] [JAP] [2026, экшен, WEB-DL] [1080p]", "Toaru Anbu no ITEM / A Certain Dark ITEM [TV] [JAP] [2026, WEB-DL] [1080p]")]
        [TestCase("Стальная тревога! IV / Fullmetal Panic! Invisible Victory [TV] [JAP] [2018, BDRip]", "Fullmetal Panic! Invisible Victory [TV] [JAP] [2018, BDRip]")]
        [TestCase("Полное погружение: Что, если лучшая RPG с полным погружением будет хуже реальности? / Kyuukyoku Shinka shita Full Dive RPG ga Genjitsu yori mo Kusoge Dattara [TV] [RUS(int), JAP] [2021, WEBRip] [1080p]", "Kyuukyoku Shinka shita Full Dive RPG ga Genjitsu yori mo Kusoge Dattara [TV] [RUS(int), JAP] [2021, WEBRip] [1080p]")]
        [TestCase("Восхищаясь девочками-волшебницами / Mahou Shoujo ni Akogarete / Gushing over Magical Girls / Становясь волшебницей [TV] [RUS(ext), JAP] [2024, этти, BDRip] [1080p]", "Mahou Shoujo ni Akogarete / Gushing over Magical Girls [TV] [RUS(ext), JAP] [2024, BDRip] [1080p]")]
        [TestCase("Охотник х Охотник (ТВ-2) / Hunter x Hunter / Хантер х Хантер [TV] [JAP] [2011, приключения, сёнэн, BDRip] [1080p]", "Hunter x Hunter (S2) [TV] [JAP] [2011, BDRip] [1080p]")]
        [TestCase("Мастера меча онлайн: Порядковый ранг / Gekijouban Sword Art Online: Ordinal Scale / Искусство меча онлайн: Порядковый ранг / SAO: OS [TV] [RUS(int), JAP] [2017, BDRip]", "Gekijouban Sword Art Online: Ordinal Scale / SAO: OS [TV] [RUS(int), JAP] [2017, BDRip]")]
        public void should_drop_mostly_cyrillic_title_segments(string title, string expected)
        {
            _titleParser.Parse(title, AnimeCategories, stripCyrillicLetters: true, addRussianToTitle: true)
                .Should().Be(expected);
        }

        [Test]
        public void should_keep_latin_title_next_to_cyrillic_names_in_parentheses()
        {
            _titleParser.Parse("Южный Парк / South Park (Мэтт Стоун, Трэй Паркер) / S27E1-5 of 5 [2025, США, комедия, WEB-DL 1080p] + Original", TvCategories, stripCyrillicLetters: true)
                .Should().Be("South Park / S27E1-5 of 5 [2025, WEB-DL 1080p] + Original");
        }

        [TestCase("Охотник х Охотник / Hunter x Hunter [TV] [01 из ХХ] [JAP]", "Охотник х Охотник (S01E01) / Hunter x Hunter [TV] [JAP]")]
        [TestCase("Охотник х Охотник (ТВ-2) / Hunter x Hunter [TV] [05-08 из ХХ] [JAP]", "Охотник х Охотник (S02E05-08) / Hunter x Hunter [TV] [JAP]")]
        public void should_only_read_cyrillic_x_as_unknown_episode_count_after_iz(string title, string expected)
        {
            // the "х" in "Охотник х Охотник" is a letter of the title, the "ХХ" after "из" an unknown episode count
            _titleParser.Parse(title, AnimeCategories, stripCyrillicLetters: false)
                .Should().Be(expected);
        }

        [TestCase("Фермерская жизнь в ином мире (ТВ-1) / Isekai Nonbiri Nouka / Farming Life in Another World [TV] [JAP] [2023, BDRemux] [1080p]", "Isekai Nonbiri Nouka (S1) / Farming Life in Another World [TV] [JAP] [2023, BDRemux] [1080p]")]
        [TestCase("Фермерская жизнь в ином мире (ТВ-1) / Isekai Nonbiri Nouka / Farming Life in Another World [TV] [JAP+Sub] [2023, BDRip] [1080p]", "Isekai Nonbiri Nouka (S1) / Farming Life in Another World [TV] [JAP] [2023, BDRip] [1080p]")]
        [TestCase("Фермерская жизнь в ином мире (ТВ-1) / Isekai Nonbiri Nouka / Farming Life in Another World [TV] [RUS(ext), JAP] [2023, BDRip] [1080p]", "Isekai Nonbiri Nouka (S1) / Farming Life in Another World [TV] [RUS(ext), JAP] [2023, BDRip] [1080p]")]
        [TestCase("Фермерская жизнь в ином мире (ТВ-1) / Isekai Nonbiri Nouka / Farming Life in Another World [TV] [2023, BDRip] [1080p]", "Isekai Nonbiri Nouka (S1) / Farming Life in Another World [TV] [2023, BDRip] [1080p] RUS")]
        public void should_not_add_rus_to_anime_with_explicit_audio_without_it(string title, string expected)
        {
            _titleParser.Parse(title, AnimeCategories, stripCyrillicLetters: true, addRussianToTitle: true)
                .Should().Be(expected);
        }

        [Test]
        public void should_still_add_rus_to_non_anime_with_japanese_audio_tag()
        {
            _titleParser.Parse("The Irishman [JAP] (2019) WEB-DL 1080p", TvCategories, stripCyrillicLetters: false, addRussianToTitle: true)
                .Should().Be("The Irishman [JAP] (2019) WEB-DL 1080p RUS");
        }

        [TestCase("Мастера меча онлайн: Порядковый ранг / Gekijouban Sword Art Online: Ordinal Scale / Искусство меча онлайн: Порядковый ранг / SAO: OS [Movie] [RUS(int), JAP] [2017, приключения, драма, фэнтези, BDRip] [720p]", "Gekijouban Sword Art Online: Ordinal Scale / SAO: OS (2017) [RUS(int), JAP] [2017, BDRip] [720p]")]
        [TestCase("Евангелион: Смерть и перерождение / Shinseiki Evangelion Gekijouban: Shi to Shinsei / Evangelion: Death & Rebirth [Movie] [RUS(int), JAP] [1997, драма, фантастика, меха, BDRip] [1080p]", "Shinseiki Evangelion Gekijouban: Shi to Shinsei / Evangelion: Death & Rebirth (1997) [RUS(int), JAP] [1997, BDRip] [1080p]")]
        [TestCase("Евангелион: Смерть и перерождение / Shinseiki Evangelion Gekijouban: Shi to Shinsei [Movie] [RUS(int), JAP] [1997, драма, фантастика, меха, BDRip] [1080p]", "Shinseiki Evangelion Gekijouban: Shi to Shinsei (1997) [RUS(int), JAP] [1997, BDRip] [1080p]")]
        [TestCase("Мастера меча онлайн: Прогрессив - Скерцо глубокой ночи / Gekijouban Sword Art Online: Progressive - Kuraki Yuuyami no Scherzo / SAO: Progressive - Scherzo of Deep Night [Movie] [JAP] [2022, экшен, BDRip] [1080p]", "Gekijouban Sword Art Online: Progressive - Kuraki Yuuyami no Scherzo / SAO: Progressive - Scherzo of Deep Night (2022) [JAP] [2022, BDRip] [1080p]")]
        [TestCase("Охотник х Охотник: Пилотная серия / Hunter x Hunter Pilot [Movie][RUS(ext), JAP][1998, приключения, комедия, VHSRip]", "Hunter x Hunter Pilot (1998) [RUS(ext), JAP][1998, VHSRip]")]
        public void should_put_year_next_to_anime_movie_title(string title, string expected)
        {
            _titleParser.Parse(title, AnimeMovieCategories, stripCyrillicLetters: true, addRussianToTitle: true)
                .Should().Be(expected);
        }

        [Test]
        public void should_keep_movie_tag_of_anime_without_year()
        {
            _titleParser.Parse("Фильм / Film Name [Movie] [RUS(int), JAP] [BDRip]", AnimeMovieCategories, stripCyrillicLetters: true, addRussianToTitle: true)
                .Should().Be("Film Name [Movie] [RUS(int), JAP] [BDRip]");
        }

        // Radarr reads "Uncut" in front of "[1999, ...]" as an edition and loses it from the title; with the year alone in its
        // bracket "South Park: Bigger Longer & Uncut" matches the library movie "South Park: Bigger, Longer & Uncut".
        [TestCase("Южный Парк: Большой, длинный, необрезанный / South Park: Bigger Longer & Uncut [1999, США, мультфильм, комедия, BDRip] [1080p] Dub + Original", "South Park: Bigger Longer & Uncut [1999] [BDRip] [1080p] Dub + Original RUS")]
        [TestCase("Южный Парк: Большой, длинный, необрезанный / South Park: Bigger Longer & Uncut [1999, США, мультфильм, комедия, BDRip]", "South Park: Bigger Longer & Uncut [1999] [BDRip] RUS")]
        [TestCase("Ирландец / The Irishman (Мартин Скорсезе / Martin Scorsese) [2019, США, криминал, драма, WEB-DL 1080p] Dub + Original Eng", "The Irishman [2019, WEB-DL 1080p] Dub + Original Eng RUS")]
        [TestCase("Фильм / Some Movie Extended Cut [2019, США, драма, BDRip 1080p]", "Some Movie Extended Cut [2019, BDRip 1080p] RUS")]
        public void should_keep_edition_word_that_ends_movie_title(string title, string expected)
        {
            _titleParser.Parse(title, MovieCategories, stripCyrillicLetters: true, addRussianToTitle: true)
                .Should().Be(expected);
        }

        [TestCase("Демоны старшей школы / High School DxD [Янагисава Тэцуя][TV][1 сезон][12 из 12 / 6 из 6][2012, приключения, мистика, этти, комедия, HDRip][Субтитры]", "High School DxD [TV][1 ][12 12 / 6 6][2012, HDRip] RUS")]
        [TestCase("Демоны старшей школы (OBA) / High School DxD OAD / Highschool DxD OAD / Старшая школа ДхД: Демоны против падших [OVA] [4 из 4] [RUS(ext), JAP+Sub] [2012-2015, приключения, мистика, этти, комедия, демоны, BDRemux] [1080p]", "High School DxD OAD / Highschool DxD OAD [OVA] E4 of 4 [RUS(ext), JAP] [2012-2015, BDRemux] [1080p]")]
        public void should_keep_episode_count_for_anime_without_season_token(string title, string expected)
        {
            _titleParser.Parse(title, AnimeCategories, stripCyrillicLetters: true, addRussianToTitle: true)
                .Should().Be(expected);
        }
    }
}
