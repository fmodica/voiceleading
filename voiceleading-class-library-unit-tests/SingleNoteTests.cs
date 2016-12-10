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
        public async Task SingleNoteLeadsToSingleNote()
        {
            var config = new Config();
            config.StringedInstrument = new StringedInstrument(new List<MusicalNote>()
            {
                new MusicalNote(NoteLetter.E, 4),
                new MusicalNote(NoteLetter.B, 3),
                new MusicalNote(NoteLetter.G, 3),
                new MusicalNote(NoteLetter.D, 3),
                new MusicalNote(NoteLetter.A, 2),
                new MusicalNote(NoteLetter.E, 2)
            }, 24);
            config.CalculationTimeoutInMilliseconds = 10000;
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
            Assert.IsTrue(CollectionsAreEquivalent(expectedFingerings, results.First().Fingerings));
        }

        [TestMethod]
        public async Task SingleNoteLeadsToSingleNoteAndSplits()
        {
            var config = new Config();
            config.StringedInstrument = new StringedInstrument(new List<MusicalNote>()
            {
                new MusicalNote(NoteLetter.E, 4),
                new MusicalNote(NoteLetter.B, 3),
                new MusicalNote(NoteLetter.G, 3),
                new MusicalNote(NoteLetter.D, 3),
                new MusicalNote(NoteLetter.A, 2),
                new MusicalNote(NoteLetter.E, 2)
            }, 24);
            config.CalculationTimeoutInMilliseconds = 10000;
            config.TargetChordRoot = NoteLetter.Fsharp;
            config.MaxFretsToStretch = 4;
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
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>() {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.B, 3), 7),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.G, 3), 11)
                }),
                new Chord<StringedMusicalNote>(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.D, 3), 16)),
                new Chord<StringedMusicalNote>(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.A, 2), 21))
            };

            Assert.AreEqual(results.Count(), 1);
            Assert.IsTrue(CollectionsAreEquivalent(expectedFingerings, results.First().Fingerings));
        }

        [TestMethod]
        public async Task SingleNoteLeadsToTwoNotesAndSplitsInvolvingOpenStrings()
        {
            var config = new Config();
            config.StringedInstrument = new StringedInstrument(new List<MusicalNote>()
            {
                new MusicalNote(NoteLetter.E, 4),
                new MusicalNote(NoteLetter.B, 3),
                new MusicalNote(NoteLetter.G, 3),
                new MusicalNote(NoteLetter.D, 3),
                new MusicalNote(NoteLetter.A, 2),
                new MusicalNote(NoteLetter.E, 2)
            }, 12);
            config.CalculationTimeoutInMilliseconds = 10000;
            config.TargetChordRoot = NoteLetter.C;
            config.MaxFretsToStretch = 4;
            config.MaxVoiceleadingDistance = Interval.Second;
            config.TargetChordIntervalOptionalPairs = new List<IntervalOptionalPair>()
            {
                new IntervalOptionalPair()
                {
                    Interval = Interval.Root,
                    IsOptional = false,
                },
                new IntervalOptionalPair()
                {
                    Interval = Interval.Third,
                    IsOptional = false,
                }
            };
            config.StartChord = new Chord<MusicalNote>(new MusicalNote(NoteLetter.D, 4));

            var voiceleader = new Voiceleader(config);
            await voiceleader.CalculateVoicings();
            var results = voiceleader.VoicingSets;

            var expectedFingerings = new List<Chord<StringedMusicalNote>>()
            {
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>() {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.E, 4), 0),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.B, 3), 1)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>() {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.E, 4), 0),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.G, 3), 5)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>() {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.E, 4), 0),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.B, 3), 1),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.G, 3), 5)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>() {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.E, 4), 0),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.B, 3), 5),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.G, 3), 5)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>() {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.E, 4), 0),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.D, 3), 10)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>() {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.E, 4), 0),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.G, 3), 9),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.D, 3), 10)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>() {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.B, 3), 5),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.G, 3), 5)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>() {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.G, 3), 9),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.D, 3), 10)
                })

            };

            Assert.AreEqual(results.Count(), 1);
            Assert.IsTrue(CollectionsAreEquivalent(expectedFingerings, results.First().Fingerings));
        }

        private bool CollectionsAreEquivalent<T>(IEnumerable<T> one, IEnumerable<T> two)
        {
            return !one.Except(two).Any();
        }
    }
}