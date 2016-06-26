using Instruments;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicTheory;
using MusicTheory.Voiceleading;
using System.Collections.Generic;
using System.Linq;

namespace voiceleading_class_library_tests
{
    [TestClass]
    public class SIVoiceleaderSingleNoteUnitTests
    {
        [TestMethod]
        public void SingleNoteMovesTowardsOtherSingleNoteWithinVoiceleadingDistanceLimitCorrectly()
        {
            var config = GetStandardConfig();

            config.EndChordRoot = NoteLetter.Fsharp;
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
            config.StartingChordNotes = new List<MusicalNote>()
            {
                new MusicalNote(NoteLetter.E, 4)
            };

            var voiceleader = new SIVoiceleader(config);

            voiceleader.CalculateVoicings();

            var results = voiceleader.VoicingSets;

            var expectedFingerings = new List<Chord>()
            {
                new Chord(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.E, 4), 2)),
                new Chord(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.B, 3), 7)),
                new Chord(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.G, 3), 11)),
                new Chord(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.D, 3), 16)),
                new Chord(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.A, 2), 21))
            };

            Assert.AreEqual(results.Count(), 1);
            CollectionAssert.AreEquivalent(expectedFingerings, results.First().Fingerings);
        }

        private SIVoiceleaderConfig GetStandardConfig()
        {
            return new SIVoiceleaderConfig()
            {
                StringedInstrument = new StringedInstrument(new List<MusicalNote>()
                {
                    new MusicalNote(NoteLetter.E, 4),
                    new MusicalNote(NoteLetter.B, 3),
                    new MusicalNote(NoteLetter.G, 3),
                    new MusicalNote(NoteLetter.D, 3),
                    new MusicalNote(NoteLetter.A, 2),
                    new MusicalNote(NoteLetter.E, 2)
                }, 24)
            };
        }
    }
}