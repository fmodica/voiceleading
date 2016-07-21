using System.Collections.Generic;
using Instruments;

namespace MusicTheory.Voiceleading
{
    public class Config
    {
        public Chord<MusicalNote> StartChord { get; set; }
        public StringedInstrument StringedInstrument { get; set; }
        public NoteLetter? EndChordRoot { get; set; }
        public List<IntervalOptionalPair> TargetChordIntervalOptionalPairs { get; set; }
        public Interval? MaxVoiceleadingDistance { get; set; }
        public int? MaxFretsToStretch { get; set; }
        public int? FretToStayAtOrBelow { get; set; }
        public int? FretToStayAtOrAbove { get; set; }
        public NoteLetter? HighestNote { get; set; }
        public bool HighestNoteCanTravel { get; set; }
        public bool LowestNoteCanTravel { get; set; }
        public bool FilterOutOpenNotes { get; set; }
        public int CalculationTimeoutInMilliseconds { get; set; }
    }
}