using System.Collections.Generic;

namespace MusicTheory.Voiceleading
{
    public class SIVoicingSetGrouper
    {
        private Dictionary<string, SIVoicingSet> MapFromVoicingStringRepresentationToVoicingSet { get; set; }

        public SIVoicingSetGrouper()
        {
            MapFromVoicingStringRepresentationToVoicingSet = new Dictionary<string, SIVoicingSet>();
        }

        public SIVoicingSetGrouper(Chord chord) : this()
        {
            AddChord(chord);
        }

        public SIVoicingSetGrouper(IEnumerable<Chord> chords) : this()
        {
            foreach (var chord in chords)
            {
                AddChord(chord);
            }
        }

        public void AddChord(Chord chord)
        {
            var key = chord.ToUniqueMusicalNoteString();

            if (MapFromVoicingStringRepresentationToVoicingSet.ContainsKey(key))
            {
                MapFromVoicingStringRepresentationToVoicingSet[key].Fingerings.Add(chord);
            }
            else
            {
                MapFromVoicingStringRepresentationToVoicingSet[key] = new SIVoicingSet(chord);
            }
        }

        public IEnumerable<SIVoicingSet> GetVoicingSets()
        {
            return MapFromVoicingStringRepresentationToVoicingSet.Values;
        }
    }
}
