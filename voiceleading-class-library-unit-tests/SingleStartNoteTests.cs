using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicTheory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voiceleading;

namespace voiceleading_class_library_tests
{
    [TestClass]
    public class SingleStartNoteTests
    {
        [TestMethod]
        public async Task SingleNote_LeadsToSingleNote()
        {
            var config = new Config
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
                CalculationTimeoutInMilliseconds = 10000000,
                TargetChordRoot = NoteLetter.Fsharp,
                MinFret = 0,
                MaxFret = 24,
                MaxFretsToStretch = 0,
                MaxVoiceleadingDistance = Interval.Second,
                TargetChordIntervalOptionalPairs = new List<IntervalOptionalPair>()
                {
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.Root,
                        IsOptional = false
                    }
                },
                StartChord = new Chord<MusicalNote>(new MusicalNote(NoteLetter.E, 4))
            };

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

            Assert.AreEqual(1, results.Count());
            Assert.IsTrue(CollectionsAreEquivalent(expectedFingerings, results.First().Fingerings));
        }

        [TestMethod]
        public async Task SingleNote_LeadsToSingleNote_WhichCanExistOnMultipleStringsSimultaneously()
        {
            var config = new Config
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
                CalculationTimeoutInMilliseconds = 10000,
                TargetChordRoot = NoteLetter.Fsharp,
                MinFret = 0,
                MaxFret = 24,
                MaxFretsToStretch = 4,
                MaxVoiceleadingDistance = Interval.Second,
                TargetChordIntervalOptionalPairs = new List<IntervalOptionalPair>()
                {
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.Root,
                        IsOptional = false
                    }
                },
                StartChord = new Chord<MusicalNote>(new MusicalNote(NoteLetter.E, 4))
            };

            var voiceleader = new Voiceleader(config);
            await voiceleader.CalculateVoicings();
            var results = voiceleader.VoicingSets;

            var expectedFingerings = new List<Chord<StringedMusicalNote>>()
            {
                new Chord<StringedMusicalNote>(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.E, 4), 2)),
                new Chord<StringedMusicalNote>(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.B, 3), 7)),
                new Chord<StringedMusicalNote>(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.G, 3), 11)),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>()
                {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.B, 3), 7),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.G, 3), 11)
                }),
                new Chord<StringedMusicalNote>(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.D, 3), 16)),
                new Chord<StringedMusicalNote>(new StringedMusicalNote(new MusicalNote(NoteLetter.Fsharp, 4), new MusicalNote(NoteLetter.A, 2), 21))
            };

            Assert.AreEqual(1, results.Count());
            Assert.IsTrue(CollectionsAreEquivalent(expectedFingerings, results.First().Fingerings));
        }

        [TestMethod]
        public async Task SingleNote_SplitsIntoTwoNotes()
        {
            var config = new Config
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
                CalculationTimeoutInMilliseconds = 10000,
                TargetChordRoot = NoteLetter.C,
                MinFret = 0,
                MaxFret = 12,
                MaxFretsToStretch = 4,
                MaxVoiceleadingDistance = Interval.Second,
                FilterOutOpenNotes = true,
                TargetChordIntervalOptionalPairs = new List<IntervalOptionalPair>()
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
                },
                StartChord = new Chord<MusicalNote>(new MusicalNote(NoteLetter.D, 4))
            };

            var voiceleader = new Voiceleader(config);
            await voiceleader.CalculateVoicings();
            var results = voiceleader.VoicingSets;

            var expectedFingerings = new List<Chord<StringedMusicalNote>>()
            {
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>()
                {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.B, 3), 5),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.G, 3), 5)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>()
                {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.G, 3), 9),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.D, 3), 10)
                })
            };

            Assert.AreEqual(1, results.Count());
            Assert.IsTrue(CollectionsAreEquivalent(expectedFingerings, results.First().Fingerings));
        }

        [TestMethod]
        public async Task SingleNote_SplitsIntoTwoNotes_WhereEachNoteCanExistOnMultipleStringsSimultaneously()
        {
            var config = new Config
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
                CalculationTimeoutInMilliseconds = 10000,
                TargetChordRoot = NoteLetter.C,
                MinFret = 0,
                MaxFret = 12,
                MaxFretsToStretch = 4,
                MaxVoiceleadingDistance = Interval.Second,
                TargetChordIntervalOptionalPairs = new List<IntervalOptionalPair>()
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
                },
                StartChord = new Chord<MusicalNote>(new MusicalNote(NoteLetter.D, 4))
            };

            var voiceleader = new Voiceleader(config);
            await voiceleader.CalculateVoicings();
            var results = voiceleader.VoicingSets;

            var expectedFingerings = new List<Chord<StringedMusicalNote>>()
            {
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>()
                {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.E, 4), 0),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.B, 3), 1)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>()
                {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.E, 4), 0),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.G, 3), 5)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>()
                {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.E, 4), 0),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.B, 3), 1),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.G, 3), 5)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>()
                {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.E, 4), 0),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.B, 3), 5),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.G, 3), 5)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>()
                {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.E, 4), 0),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.D, 3), 10)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>()
                {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.E, 4), 0),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.G, 3), 9),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.D, 3), 10)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>()
                {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.B, 3), 5),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.G, 3), 5)
                }),
                new Chord<StringedMusicalNote>(new List<StringedMusicalNote>()
                {
                    new StringedMusicalNote(new MusicalNote(NoteLetter.E, 4), new MusicalNote(NoteLetter.G, 3), 9),
                    new StringedMusicalNote(new MusicalNote(NoteLetter.C, 4), new MusicalNote(NoteLetter.D, 3), 10)
                })
            };

            Assert.AreEqual(1, results.Count());
            Assert.IsTrue(CollectionsAreEquivalent(expectedFingerings, results.First().Fingerings));
        }

        private static bool CollectionsAreEquivalent<T>(IEnumerable<T> one, IEnumerable<T> two)
        {
            return !one.Except(two).Any();
        }
    }
}