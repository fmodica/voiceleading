using System.Collections.Generic;

namespace MusicTheory.Voiceleading
{
    public class SIVoicingSetGrouper
    {
        private Dictionary<string, SIVoicingSet> MapFromVoicingStringRepresentationToVoicingSet { get; set; } = new Dictionary<string, SIVoicingSet>();
        // Should return a copy?
        public IEnumerable<SIVoicingSet> VoicingSets
        {
            get
            {
                return MapFromVoicingStringRepresentationToVoicingSet.Values;
            }
        }

        public SIVoicingSetGrouper(Chord chord)
        {
            AddChord(chord);
        }

        public SIVoicingSetGrouper(IEnumerable<Chord> chords)
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
    }
}