using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicTheory.Voiceleading
{
    // Stringed Instrument Voicing Set
    public class SIVoicingSet
    {
        public SIVoicingSet()
        {
            Fingerings = new List<List<StringedMusicalNote>>();
        }
        // List of Fingerings
        public List<List<StringedMusicalNote>> Fingerings { get; set; }
        public MusicalNote LowestNote
        {
            get
            {
                if (Fingerings.Count > 0)
                {
                    // Take a sample fingering and remove any of the same notes like:
                    // E: 0
                    // B: 5
                    var sample = Fingerings[0].Distinct();

                    var min = sample.Min();

                    return min;
                }
                else
                {
                    return null;
                }
            }
        }

        public int NumNotes
        {
            get
            {
                if (Fingerings.Count > 0)
                {
                    // Don't count the same notes on different strings
                    var sample = Fingerings[0].Distinct();

                    return sample.Count();
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}