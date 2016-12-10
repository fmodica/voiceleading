using HelperExtensions;
using MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Voiceleading
{
    public class VoicingSet
    {
        public List<Chord<StringedMusicalNote>> Fingerings { get; private set; }
        private Chord<MusicalNote> StartChord { get; set; }

        public VoicingSet(Chord<StringedMusicalNote> targetChordFingering, Chord<MusicalNote> startChord)
        {
            targetChordFingering.ValidateIsNotNull(nameof(targetChordFingering));
            startChord.ValidateIsNotNull(nameof(startChord));
            Fingerings = new List<Chord<StringedMusicalNote>>() { targetChordFingering };
            StartChord = startChord;
        }

        public VoicingSet(IEnumerable<Chord<StringedMusicalNote>> targetChordFingerings, Chord<MusicalNote> startChord)
        {
            targetChordFingerings.ValidateIsNotNullOrEmptyOrHasNullItem(nameof(targetChordFingerings));
            startChord.ValidateIsNotNull(nameof(startChord));
            Fingerings = new List<Chord<StringedMusicalNote>>(targetChordFingerings);
            StartChord = startChord;
        }

        public int NumUniqueNotes
        {
            get { return GetUniqueFingering(Fingerings.Select(chord => new Chord<MusicalNote>(chord.Notes))).Notes.Count; }
        }

        public MusicalNote LowestNote
        {
            get { return Fingerings.First().Notes.OrderBy(note => note.IntValue).First(); }
        }

        public MusicalNote HighestNote
        {
            get { return Fingerings.First().Notes.OrderByDescending(note => note.IntValue).First(); }
        }

        public double? AverageVoiceleadingDistance
        {
            get
            {
                var uniqueStartChord = GetUniqueFingering(new List<Chord<MusicalNote>>() { StartChord });
                var uniqueTargetChord = GetUniqueFingering(Fingerings.Select(chord => new Chord<MusicalNote>(chord.Notes)));
                var sumOfMinimumDifferencesForBothChords = GetSumOfMinimumDifferences(uniqueStartChord, uniqueTargetChord) + GetSumOfMinimumDifferences(uniqueTargetChord, uniqueStartChord);

                return sumOfMinimumDifferencesForBothChords / (uniqueStartChord.Notes.Count + uniqueTargetChord.Notes.Count);
            }
        }

        private double GetSumOfMinimumDifferences(Chord<MusicalNote> chord1, Chord<MusicalNote> chord2)
        {
            return chord1.Notes.Sum(noteFromChord1 => CalculateMinimumDifference(chord2, noteFromChord1));
        }

        private static int CalculateMinimumDifference(Chord<MusicalNote> chord, MusicalNote note)
        {
            return chord.Notes.Min(chordNote => Math.Abs(note.IntValue - chordNote.IntValue));
        }

        private Chord<MusicalNote> GetUniqueFingering(IEnumerable<Chord<MusicalNote>> chords)
        {
            return new Chord<MusicalNote>(chords.First().Notes.Distinct());
        }
    }
}