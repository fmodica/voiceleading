using System.Collections.Generic;
using MusicTheory;
using System.Linq;
using System;

namespace Instruments
{
    public class StringedInstrument
    {
        public IEnumerable<MusicalNote> Tuning { get; private set; }
        public int NumFrets { get; private set; }

        public StringedInstrument(IEnumerable<MusicalNote> tuning, int numFrets)
        {
            if (tuning == null || !tuning.Any())
            {
                throw new ArgumentException(nameof(tuning) + " must have at least one note.");
            }

            if (numFrets <= 0)
            {
                throw new ArgumentException(nameof(numFrets) + " must be greater than zero.");
            }

            Tuning = tuning;
            NumFrets = numFrets;
        }

        public IEnumerable<StringedMusicalNote> GetNotesOnInstrument(NoteLetter? noteLetter)
        {
            var notes = new List<StringedMusicalNote>();

            foreach (var tuningNote in Tuning)
            {
                notes.AddRange(GetNotesOnString(noteLetter, tuningNote));
            }

            return notes.OrderBy(x => x.StringItsOn.IntValue);
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
                    var stringItsOn = new MusicalNote(tuningNote.Letter, tuningNote.Octave);

                    notes.Add(new StringedMusicalNote(foundNote, stringItsOn, i));
                }
            }

            return notes;
        }
    }
}