using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicTheory.Voiceleading
{
    // Stringed Instrument Voicing Set
    public class SIVoicingSet
    {
        // Should return a copy?
        public List<Chord> Fingerings { get; private set; } = new List<Chord>();

        public SIVoicingSet(Chord chord)
        {
            Fingerings.Add(chord);
        }

        public SIVoicingSet(IEnumerable<Chord> chords)
        {
            Fingerings.AddRange(chords);
        }

        public int NumUniqueNotes
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
                return Fingerings[0].Notes.OrderBy(x => x.IntValue).First();
            }
        }

        public MusicalNote HighestNote
        {
            get
            {
                // All fingerings should have the same notes. So just pick the
                // first fingering and grab the highest note.
                return Fingerings[0].Notes.OrderByDescending(x => x.IntValue).First();
            }
        }
    }
}