using System.Collections.Generic;
using MusicTheory;

namespace Instruments
{
    public class StringedInstrument
    {
        public List<MusicalNote> Tuning { get; set; }

        public int NumFrets { get; set; }

        public StringedInstrument()
        {
            Tuning = new List<MusicalNote>();
        }

        public List<StringedMusicalNote> GetNotesByNoteLetter(NoteLetter? chordNoteLetter)
        {
            var stringedNotes = new List<StringedMusicalNote>();

            foreach (MusicalNote tuningNote in this.Tuning)
            {
                stringedNotes.AddRange(GetNotesOnString(chordNoteLetter, tuningNote));
            }

            return stringedNotes;
        }

        private List<StringedMusicalNote> GetNotesOnString(NoteLetter? chordNoteLetter, MusicalNote stringNote)
        {
            var notes = new List<StringedMusicalNote>();

            for (int i = 0; i <= NumFrets; i++)
            {
                int noteIndex = (int) stringNote.Letter + i;
                // Don't need to floor it since it's being cast to an integer.
                int numOctavesAboveString = noteIndex/12;

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
                NoteLetter noteLetter = (NoteLetter) (noteIndex - (numOctavesAboveString*12));

                if (noteLetter == chordNoteLetter)
                {
                    notes.Add(new StringedMusicalNote()
                    {
                        Letter = noteLetter,
                        Octave = stringNote.Octave + numOctavesAboveString,
                        Fret = i,
                        StringItsOn = stringNote
                    });
                }
            }

            return notes;
        }
    }
}
