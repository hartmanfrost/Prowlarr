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

        [Test]
        public void should_normalize_plural_seasons_with_single_number()
        {
            // "Seasons 8" (plural with a single number) breaks Sonarr's parser: it fails
            // language detection and falls back to the series default. Normalize to "S8".
            _titleParser.Parse("Futurama (Seasons 8) (2023) WEB-DL 1080p 2xUkr/Eng | Sub Eng", TvCategories, stripCyrillicLetters: false)
                .Should().Be("Futurama (S8) (2023) WEB-DL 1080p 2xUkr/Eng | Sub Eng");
        }

        [Test]
        public void should_normalize_number_first_season_notation()
        {
            // Cyrillic trackers often write the season number first ("7 season" / "7 сезон").
            _titleParser.Parse("The Simpsons (7 season) Ukr/Eng", TvCategories, stripCyrillicLetters: false)
                .Should().Be("The Simpsons (S7) Ukr/Eng");
        }

        [Test]
        public void should_normalize_cyrillic_p_resolution()
        {
            // FanVoxUA writes "1080р" with a Cyrillic "р" that the Cyrillic stripping would otherwise eat,
            // leaving a bare "1080" that Sonarr reads as season 10.
            _titleParser.Parse("Раґна Крімзон (S1) / Ragna Crimson (S1) (2023) WEB-DL 1080р H.265 [FanVoxUA]", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Ragna Crimson (S1) (2023) WEB-DL 1080p H.265 [FanVoxUA] (S1)");
        }

        [TestCase("Mahou Shoujo ni Akogarete (S1) (2024) WEB-DL 1080 Jap | sub Ukr [Okashi]", "Mahou Shoujo ni Akogarete (S1) (2024) WEB-DL 1080p Jap | sub Ukr [Okashi]")]
        [TestCase("Valkyrie Drive: Mermaid (S1) (2015) HDTV 720 [FanVoxUA]", "Valkyrie Drive: Mermaid (S1) (2015) HDTV 720p [FanVoxUA]")]
        [TestCase("Sekai Saikyou no Majo (S1) (2026) WEBRip Ai Rem 1080 Ukr/Jap", "Sekai Saikyou no Majo (S1) (2026) WEBRip Ai Rem 1080p Ukr/Jap")]
        [TestCase("Mob Psycho 100 (S1) (2016) BDRip 1080p Ukr/Jap", "Mob Psycho 100 (S1) (2016) BDRip 1080p Ukr/Jap")]
        [TestCase("Sample (S1E720) (2016) BDRip 1080p", "Sample (S1E720) (2016) BDRip 1080p")]
        public void should_add_missing_p_to_bare_resolution_after_source(string title, string expected)
        {
            _titleParser.Parse(title, TvCategories, stripCyrillicLetters: true).Should().Be(expected);
        }

        [Test]
        public void should_drop_cyrillic_segment_with_latin_numeral_crumbs()
        {
            _titleParser.Parse("Сталева тривога! IV Невидима Перемога / Full Metal Panic! IV Invisible Victory (2018) WebRip 1080p Jap | Sub Ukr [AckiyBolt]", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Full Metal Panic! IV Invisible Victory (2018) WebRip 1080p Jap | Sub Ukr [AckiyBolt]");
        }

        [Test]
        public void should_drop_cyrillic_segment_with_latin_abbreviation_crumbs()
        {
            _titleParser.Parse("Сталева тривога! (1, 2, S3) + OVA / Full Metal Panic! + OVA (2002-2006) BDRip 1080p Ukr/Jap [wrongfunction]", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Full Metal Panic! + OVA (2002-2006) BDRip 1080p Ukr/Jap [wrongfunction]");
        }

        [Test]
        public void should_keep_season_token_of_dropped_cyrillic_segment()
        {
            _titleParser.Parse("Сутінкові мисливці OVA (Сезон 1) / Shadowhunters (2016) BDRemux 1080p Ukr/Eng", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Shadowhunters (2016) BDRemux 1080p Ukr/Eng (S1)");
        }

        [Test]
        public void should_keep_latin_leading_segment()
        {
            _titleParser.Parse("Gakuen Mokushiroku / Highschool of the Dead (2010) BDRip 1080p Ukr/Jap", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Gakuen Mokushiroku / Highschool of the Dead (2010) BDRip 1080p Ukr/Jap");
        }

        [Test]
        public void should_convert_progress_in_brackets_to_episode_range_when_season_is_named()
        {
            _titleParser.Parse("Ізекай (S2) / Isekai Nonbiri Nouka (S2) [5/12] (2026) WEB-DL 1080p H.265 [FanVoxUA]", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Isekai Nonbiri Nouka (S2) (S02E01-05) (2026) WEB-DL 1080p H.265 [FanVoxUA] (S2)");
        }

        [Test]
        public void should_drop_complete_progress_in_brackets_when_season_is_named()
        {
            _titleParser.Parse("Isekai Nonbiri Nouka (S2) [12/12] (2026) WEB-DL 1080p H.265", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Isekai Nonbiri Nouka (S2) (2026) WEB-DL 1080p H.265");
        }

        [Test]
        public void should_leave_progress_in_brackets_without_a_season()
        {
            // The season is unknown ("IV" is part of the name), an assumed one could grab the wrong season.
            _titleParser.Parse("Full Metal Panic! IV Invisible Victory [5/12] (2018) WebRip 1080p Jap", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Full Metal Panic! IV Invisible Victory [5/12] (2018) WebRip 1080p Jap");
        }

        [TestCase("Мистецтво Меча Онлайн (S1, 15 з 24) / Sword Art Online (S1) (2012) WEB-DL 1080p Ukr/Jpn", "Sword Art Online (S1) (2012) WEB-DL 1080p Ukr/Jpn (S01E01-15)")]
        [TestCase("Мистецтво Меча Онлайн 2 (S2, 12 з 12) / Sword Art Online Alternative II (S2) (2024) WEB-DL 1080p", "Sword Art Online Alternative II (S2) (2024) WEB-DL 1080p (S2)")]
        [TestCase("Що, якщо рольова гра (S1, 03 з 12-серій) | Kyuukyoku Shinka shita Full Dive RPG (2021) Ukr [mamze]", "Kyuukyoku Shinka shita Full Dive RPG (2021) Ukr [mamze] (S01E01-03)")]
        public void should_convert_season_progress_to_episode_range(string title, string expected)
        {
            _titleParser.Parse(title, TvCategories, stripCyrillicLetters: true).Should().Be(expected);
        }

        [Test]
        public void should_write_partial_episode_range_with_two_digits_and_no_total()
        {
            _titleParser.Parse("Найсильніша відьма (S1E1-3 of XX) / Sekai Saikyou no Majo (S1) (2026) WEBRip 1080p Ukr/Jap", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Sekai Saikyou no Majo (S1) (2026) WEBRip 1080p Ukr/Jap (S01E01-03)");
        }

        [Test]
        public void should_write_complete_season_without_episode_token()
        {
            _titleParser.Parse("Назва (Сезон 1, серії 1-12 з 12) / Some Anime (2022) WEB-DL 1080p", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Some Anime (2022) WEB-DL 1080p (S1)");
        }

        [Test]
        public void should_fix_look_alike_cyrillic_letters_inside_latin_words()
        {
            // Cyrillic "а" in the group name and "х" in the dub count would otherwise be stripped ("Wrden", "2Ukr").
            _titleParser.Parse("Фермерське життя (S1) / Isekai Nonbiri Nouka (S1) (2023) BDRemux 1080p 2хUkr/Jap | Sub Eng [Wаrden]", TvCategories, stripCyrillicLetters: true)
                .Should().Be("Isekai Nonbiri Nouka (S1) (2023) BDRemux 1080p 2xUkr/Jap | Sub Eng [Warden] (S1)");
        }

        [Test]
        public void should_fix_look_alike_latin_letter_inside_cyrillic_word()
        {
            // "Cерії" starts with a Latin "C"; fixed so the episode range is recognised.
            _titleParser.Parse("Мисливець (Cерії 1-7 з 62) / Hunter x Hunter (1999) DVDRip 1080p", TvCategories, stripCyrillicLetters: true)
                .Should().Contain("(E1-7 of 62)");
        }
    }
}
