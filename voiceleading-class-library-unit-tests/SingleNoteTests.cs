using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicTheory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voiceleading;

namespace voiceleading_class_library_tests
{
    [TestClass]
    public class SIVoiceleaderSingleNoteUnitTests
    {
        [TestMethod]
        public async Task SingleNoteMovesTowardsOtherSingleNoteWithinVoiceleadingDistanceLimitCorrectly()
        {
            var config = GetStandardConfig();

            config.TargetChordRoot = NoteLetter.Fsharp;
            config.MaxFretsToStretch = 0;
            config.MaxVoiceleadingDistance = Interval.Second;
            config.TargetChordIntervalOptionalPairs = new List<IntervalOptionalPair>()
            {
                new IntervalOptionalPair()
                {
                    Interval = Interval.Root,
                    IsOptional = false
                }
            };
            config.StartChord = new Chord<MusicalNote>(new MusicalNote(NoteLetter.E, 4));

            var voiceleader = new Voiceleader(config);

            await voiceleader.CalculateVoicings();

            var results = voiceleader.VoicingSets;

            var expectedFingerings = new List<Chord<StringedMusicalNote>>()
            {
                new Chord<StringedMusicalNote>(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.E, 4), 2)),
                new Chord<StringedMusicalNote>(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.B, 3), 7)),
                new Chord<StringedMusicalNote>(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.G, 3), 11)),
                new Chord<StringedMusicalNote>(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.D, 3), 16)),
                new Chord<StringedMusicalNote>(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.A, 2), 21))
            };

            Assert.AreEqual(results.Count(), 1);
            CollectionAssert.AreEquivalent(expectedFingerings, results.First().Fingerings);
        }

        private Config GetStandardConfig()
        {
            return new Config()
            {
                StringedInstrument = new StringedInstrument(new List<MusicalNote>()
                {
                    new MusicalNote(NoteLetter.E, 4),
                    new MusicalNote(NoteLetter.B, 3),
                    new MusicalNote(NoteLetter.G, 3),
                    new MusicalNote(NoteLetter.D, 3),
                    new MusicalNote(NoteLetter.A, 2),
                    new MusicalNote(NoteLetter.E, 2)
                }, 24),
                CalculationTimeoutInMilliseconds = 10000
            };
        }
    }
}