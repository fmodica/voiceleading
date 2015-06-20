using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instruments;

namespace MusicTheory.Voiceleading
{
    public class SIVoiceleaderConfig
    {
        public List<MusicalNote> StartingChordNotes { get; set; }
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
    }
}
