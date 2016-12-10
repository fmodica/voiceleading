using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicTheory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voiceleading;

namespace voiceleading_class_library_tests
{
    [TestClass]
    public class ConfigUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task ThrowsExceptionWhenTimeoutIsExceeded()
        {
            // Perform an expensive calculation
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
                CalculationTimeoutInMilliseconds = 1,
                TargetChordRoot = NoteLetter.A,
                MinFret = 0,
                MaxFret = 24,
                MaxFretsToStretch = 24,
                MaxVoiceleadingDistance = Interval.Third,
                TargetChordIntervalOptionalPairs = new List<IntervalOptionalPair>()
                {
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.Root,
                        IsOptional = true,
                    },
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.FlatSecond,
                        IsOptional = true,
                    },
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.Second,
                        IsOptional = true,
                    },
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.FlatThird,
                        IsOptional = true,
                    },
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.Third,
                        IsOptional = true
                    },
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.Fourth,
                        IsOptional = true
                    },
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.FlatFifth,
                        IsOptional = true
                    },
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.Fifth,
                        IsOptional = true
                    },
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.FlatSixth,
                        IsOptional = true
                    },
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.Sixth,
                        IsOptional = true
                    },
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.FlatSeventh,
                        IsOptional = true
                    },
                    new IntervalOptionalPair()
                    {
                        Interval = Interval.Seventh,
                        IsOptional = true
                    }
                },
                StartChord = new Chord<MusicalNote>(new List<MusicalNote>()
                {
                    new MusicalNote(NoteLetter.C, 3),
                    new MusicalNote(NoteLetter.E, 3),
                    new MusicalNote(NoteLetter.G, 3),
                    new MusicalNote(NoteLetter.B, 3)
                })
            };

            var voiceleader = new Voiceleader(config);
            await voiceleader.CalculateVoicings();
        }
    }
}