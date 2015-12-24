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
                    new MusicalNote()
                    {
                        Letter = NoteLetter.E,
                        Octave = 4
                    }
                };
            config.StringedInstrument.NumFrets = 24;

            var voiceleader = new SIVoiceleader(config);

            voiceleader.CalculateVoicings();

            var results = voiceleader.VoicingSets;

            var expectedFingerings = new List<Chord>()
            {
                new Chord(new StringedMusicalNote()
                {
                    Fret = 2,
                    Letter = NoteLetter.Fsharp,
                    Octave = 4,
                    StringItsOn = new MusicalNote()
                    {
                        Letter = NoteLetter.E,
                        Octave = 4
                    }
                }),
                new Chord (new StringedMusicalNote()
                {
                    Fret = 7,
                    Letter = NoteLetter.Fsharp,
                    Octave = 4,
                    StringItsOn = new MusicalNote()
                    {
                        Letter = NoteLetter.B,
                        Octave = 3
                    }
                }), new Chord(new StringedMusicalNote()
                {
                    Fret = 11,
                    Letter = NoteLetter.Fsharp,
                    Octave = 4,
                    StringItsOn = new MusicalNote()
                    {
                        Letter = NoteLetter.G,
                        Octave = 3
                    }
                }), new Chord(new StringedMusicalNote()
                {
                    Fret = 16,
                    Letter = NoteLetter.Fsharp,
                    Octave = 4,
                    StringItsOn = new MusicalNote()
                    {
                        Letter = NoteLetter.D,
                        Octave = 3
                    }
                }), new Chord(new StringedMusicalNote()
                {
                    Fret = 21,
                    Letter = NoteLetter.Fsharp,
                    Octave = 4,
                    StringItsOn = new MusicalNote()
                    {
                        Letter = NoteLetter.A,
                        Octave = 2
                    }
                })
            };

            Assert.AreEqual(results.Count(), 1);
            CollectionAssert.AreEquivalent(expectedFingerings, results.First().Fingerings);
        }

        private SIVoiceleaderConfig GetStandardConfig()
        {
            return new SIVoiceleaderConfig()
            {
                StringedInstrument = new StringedInstrument()
                {
                    Tuning = new List<MusicalNote>()
                    {
                        new MusicalNote()
                        {
                            Letter = NoteLetter.E,
                            Octave = 4
                        },
                        new MusicalNote()
                        {
                            Letter = NoteLetter.B,
                            Octave = 3
                        },
                        new MusicalNote()
                        {
                            Letter = NoteLetter.G,
                            Octave = 3
                        },
                        new MusicalNote()
                        {
                            Letter = NoteLetter.D,
                            Octave = 3
                        },
                        new MusicalNote()
                        {
                            Letter = NoteLetter.A,
                            Octave = 2
                        },
                        new MusicalNote()
                        {
                            Letter = NoteLetter.E,
                            Octave = 2
                        }
                    }
                }
            };
        }
    }
}