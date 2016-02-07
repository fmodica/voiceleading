using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicTheory.Voiceleading
{
    // Stringed Instrument Voicing Set
    public class SIVoicingSet
    {
        public List<Chord> Fingerings { get; set; }

        public SIVoicingSet()
        {
            Fingerings = new List<Chord>();
        }

        public SIVoicingSet(Chord chord) : this()
        {
            Fingerings.Add(chord);
        }

        public SIVoicingSet(IEnumerable<Chord> chords) : this()
        {
            Fingerings.AddRange(chords);
        }

        // Change to NumUniqueNotes
        public int NumNotes
        {
            get
            {
                // Every fingering should be made up of the same musical notes
                // but there could be duplicate musical notes on different strings:
                // E: 0
                // B: 5
                // Presumably if the above exists then the E: 0 and B: 5 exist
                // alone as well. So return the fingering with the least number 
                // of notes.
                return Fingerings.Any() ? Fingerings.OrderBy(x => x.Notes.Count()).First().Notes.Count() : 0;
            }
        }

        public MusicalNote LowestNote
        {
            get
            {
                // All fingerings should have the same notes. So just pick the
                // first fingering and grab the lowest note.
                return Fingerings.Any() ? Fingerings[0].Notes.Min() : null;
            }
        }

        public MusicalNote HighestNote
        {
            get
            {
                // All fingerings should have the same notes. So just pick the
                // first fingering and grab the highest note.
                return Fingerings.Any() ? Fingerings[0].Notes.Max() : null;
            }
        }
    }
}