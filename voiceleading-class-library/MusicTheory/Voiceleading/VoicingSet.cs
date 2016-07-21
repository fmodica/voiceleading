﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicTheory.Voiceleading
{
    // Stringed Instrument Voicing Set
    public class VoicingSet
    {
        // Should return a copy?
        public List<Chord<StringedMusicalNote>> Fingerings { get; private set; } = new List<Chord<StringedMusicalNote>>();
        private Chord<MusicalNote> StartChord { get; set; }

        public VoicingSet(Chord<StringedMusicalNote> endChordFingering, Chord<MusicalNote> startChord)
        {
            if (endChordFingering == null)
            {
                throw new ArgumentNullException(nameof(endChordFingering));
            }

            if (startChord == null)
            {
                throw new ArgumentNullException(nameof(startChord));
            }

            Fingerings.Add(endChordFingering);
            StartChord = startChord;
        }

        public VoicingSet(IEnumerable<Chord<StringedMusicalNote>> endChordFingerings, Chord<MusicalNote> startChord)
        {
            if (endChordFingerings == null)
            {
                throw new ArgumentNullException(nameof(endChordFingerings));
            }

            if (endChordFingerings.Any(x => x == null))
            {
                throw new ArgumentNullException("An object in " + nameof(endChordFingerings) + " is null.");
            }
         
            if (startChord == null)
            {
                throw new ArgumentNullException(nameof(startChord));
            }

            foreach (var chord in endChordFingerings)
            {
                Fingerings.Add(chord);
            }

            StartChord = startChord;
        }

        public int NumUniqueNotes
        {
            get
            {
                return GetUniqueFingering(Fingerings.Cast<Chord<MusicalNote>>()).Notes.Count;
            }
        }

        public MusicalNote LowestNote
        {
            get
            {
                return Fingerings[0].Notes.OrderBy(x => x.IntValue).First();
            }
        }

        public MusicalNote HighestNote
        {
            get
            {
                return Fingerings[0].Notes.OrderByDescending(x => x.IntValue).First();
            }
        }

        public double? AverageVoiceleadingDistance
        {
            get
            {
                var uniqueStartChord = GetUniqueFingering(new List<Chord<MusicalNote>>() { StartChord });
                var uniqueEndChord = GetUniqueFingering(Fingerings.Cast<Chord<MusicalNote>>());
                var sumOfMinimumDifferencesForBothChords = GetSumOfMinimumDifferences(uniqueStartChord, uniqueEndChord) + GetSumOfMinimumDifferences(uniqueEndChord, uniqueStartChord);

                return sumOfMinimumDifferencesForBothChords / (uniqueStartChord.Notes.Count + uniqueEndChord.Notes.Count);
            }
        }

        private double GetSumOfMinimumDifferences(Chord<MusicalNote> chord1, Chord<MusicalNote> chord2)
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

        private Chord<MusicalNote> GetUniqueFingering(IEnumerable<Chord<MusicalNote>> chords)
        {
            var map = new Dictionary<int, MusicalNote>();

            foreach (var note in chords.First().Notes)
            {
                if (!map.ContainsKey(note.IntValue))
                {
                    map[note.IntValue] = note;
                }
            }

            return new Chord<MusicalNote>(map.Values);
        }
    }
}