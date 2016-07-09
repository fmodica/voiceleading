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
        private Chord StartChord { get; set; }

        public SIVoicingSet(Chord chord, Chord startChord)
        {
            Validate(startChord);
            Validate(chord);

            StartChord = startChord;
            Fingerings.Add(chord);
        }

        public SIVoicingSet(IEnumerable<Chord> chords, Chord startChord)
        {
            Validate(startChord);
            StartChord = startChord;

            foreach (var chord in chords)
            {
                Validate(chord);
                Fingerings.Add(chord);
            }
        }

        private static void Validate(Chord chord)
        {
            if (chord == null || !chord.Notes.Any())
            {
                throw new ArgumentException(nameof(chord) + " was null or empty.");
            }
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
                return GetUniqueFingering(Fingerings).Notes.Count;
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

        public double? AverageVoiceleadingDistance
        {
            get
            {
                var uniqueStartChord = GetUniqueFingering(new List<Chord>() { StartChord });
                var uniqueEndChord = GetUniqueFingering(Fingerings);
                var sumOfMinimumDifferencesForBothChords = GetSumOfMinimumDifferences(uniqueStartChord, uniqueEndChord) + GetSumOfMinimumDifferences(uniqueEndChord, uniqueStartChord);

                return sumOfMinimumDifferencesForBothChords / (uniqueStartChord.Notes.Count + uniqueEndChord.Notes.Count);
            }
        }

        private double GetSumOfMinimumDifferences(Chord chord1, Chord chord2)
        {
            var sumOfMinDistances = 0.0;

            foreach (var note1 in chord1.Notes)
            {
                double? minDistance = null;

                foreach (var note2 in chord2.Notes)
                {
                    var difference = Math.Abs(note1.IntValue - note2.IntValue);

                    if (minDistance == null || (difference < minDistance))
                    {
                        minDistance = difference;
                    }
                }

                sumOfMinDistances += minDistance.Value;
            }

            return sumOfMinDistances;
        }

        private Chord GetUniqueFingering(IEnumerable<Chord> fingerings)
        {
            var chordsWithUniqueNotes = new List<Chord>();

            foreach (var chord in fingerings)
            {
                var map = new Dictionary<int, MusicalNote>();

                foreach (var note in chord.Notes)
                {
                    if (!map.ContainsKey(note.IntValue))
                    {
                        map[note.IntValue] = new MusicalNote(note.Letter, note.Octave);
                    }
                }

                chordsWithUniqueNotes.Add(new Chord(map.Values));
            }

            return chordsWithUniqueNotes.First();
        }
    }
}