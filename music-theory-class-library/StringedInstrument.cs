using System;
using System.Collections.Generic;
using System.Linq;
using HelperExtensions;

namespace MusicTheory
{
    public class StringedInstrument
    {
        public IEnumerable<MusicalNote> Tuning { get; private set; }
        public int NumFrets { get; private set; }

        public StringedInstrument(IEnumerable<MusicalNote> tuning, int numFrets)
        {
            tuning.ValidateIsNotNullOrEmptyOrHasNullItem(nameof(tuning));
            numFrets.ValidateIsGreaterThan(0, nameof(numFrets));

            Tuning = tuning;
            NumFrets = numFrets;
        }

        public Dictionary<MusicalNote, List<StringedMusicalNote>> CreateMapFromStringToNotesOnString(IEnumerable<NoteLetter> noteLetters, Func<StringedMusicalNote, bool> filterFunc)
        {
            return noteLetters
                .Distinct()
                .SelectMany(noteLetter => GetNotesOnInstrument(noteLetter))
                .Where(note => filterFunc == null || filterFunc(note))
                .GroupBy(note => note.String)
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        private IEnumerable<StringedMusicalNote> GetNotesOnInstrument(NoteLetter noteLetter)
        {
            return Tuning
                .SelectMany(tuningNote => GetNotesOnString(noteLetter, tuningNote))
                .OrderBy(note => note.String.IntValue);
        }

        private List<StringedMusicalNote> GetNotesOnString(NoteLetter? noteLetterToFind, MusicalNote tuningNote)
        {
            var notes = new List<StringedMusicalNote>();

            for (var i = 0; i <= NumFrets; i++)
            {
                var noteIndex = (int)tuningNote.Letter + i;
                // We don't need to floor it since it's being cast to an integer.
                var numOctavesAboveString = noteIndex / 12;

                // If noteIndex is <= 11, the note on this fret is in the same octave 
                // as the string's note. After 11, the octave increments. We need to 
                // know how many times the octave has incremented, which is 
                // noteIndex / 12 floored, and use that to get noteIndex down 
                // to something between 0 and 11. 

                // Example: If our string has note F4, the letter F is at index 5. If 
                // our fret number is 22 our noteIndex is 27, which means the octave has 
                // incremented twice (once after 12, the other after 24) and we get that 
                // number by doing 27 / 12 floored. So we must reduce 27 by two octaves 
                // to get it below 12. Thus it becomes 27 - (12 * 2) = 3, which is note Eb.
                var noteLetter = (NoteLetter)(noteIndex - (numOctavesAboveString * 12));

                if (noteLetter == noteLetterToFind)
                {
                    var foundNote = new MusicalNote(noteLetter, tuningNote.Octave + numOctavesAboveString);
                    var @string = new MusicalNote(tuningNote.Letter, tuningNote.Octave);

                    notes.Add(new StringedMusicalNote(foundNote, @string, i));
                }
            }

            return notes;
        }
    }
}