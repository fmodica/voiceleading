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
        public int NumNotes
        {
            get
            {
                if (Fingerings.Any())
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

        public MusicalNote LowestNote()
        {
            if (Fingerings.Any())
            {
                return Fingerings[0].GetLowest();
            }
            else
            {
                return null;
            }
        }
    }
}