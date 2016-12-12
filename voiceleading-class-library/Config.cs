using MusicTheory;
using System.Collections.Generic;

namespace Voiceleading
{
    public class Config
    {
        public Chord<MusicalNote> StartChord { get; set; }
        public StringedInstrument StringedInstrument { get; set; }
        public NoteLetter TargetChordRoot { get; set; }
        public List<IntervalOptionalPair> TargetChordIntervalOptionalPairs { get; set; }
        public Interval MaxVoiceleadingDistance { get; set; }
        public int MaxFretsToStretch { get; set; }
        public int MinFret { get; set; }
        public int MaxFret { get; set; }
        public NoteLetter? HighestNote { get; set; }
        public NoteLetter? LowestNote { get; set; }
        public bool HighestNoteCanTravel { get; set; }
        public bool LowestNoteCanTravel { get; set; }
        public bool FilterOutOpenNotes { get; set; }
        public int CalculationTimeoutInMilliseconds { get; set; }
    }
}