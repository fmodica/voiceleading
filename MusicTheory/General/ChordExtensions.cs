using System;

namespace MusicTheory
{
    public static class ChordExtensions
    {
        public static NoteLetter?[] GetNoteLettersOfChord(this NoteLetter? chordRoot, Interval[] intervals)
        {
            if (chordRoot == null)
            {
                throw new ArgumentException("The chord root cannot be null.");
            }

            if (intervals == null)
            {
                throw new ArgumentException("The array of intervals cannot be null.");
            }

            var noteLetters = new NoteLetter?[intervals.Length];
            var rootValue = (int)chordRoot;

            for (int i = 0; i < noteLetters.Length; i++)
            {
                var intervalDistance = (int)intervals[i];
                var rootPlusIntervalIndex = rootValue + intervalDistance;

                if (rootPlusIntervalIndex > 11)
                {
                    rootPlusIntervalIndex -= 12;
                }

                NoteLetter? chordNote = (NoteLetter?)Enum.Parse(typeof(NoteLetter), rootPlusIntervalIndex.ToString());

                noteLetters[i] = chordNote;
            }

            return noteLetters;
        }

        // SI is the stringed instrument, scaleNotes gives the section of the instrument, and "intervals" specifices the kinds of arpeggios 
        // you want (triads, add9's, custom, etc.)
        //public static List<List<NoteLetter>> GetArpeggiosInSISection(StringedInstrument SI, MusicalNote[] scaleNotes, Interval[] intervals)
        //{
        //    // each List<NoteLetter> is an arpeggio
        //    var allArpeggios = new List<List<NoteLetter>>();

        //    var uniqueNotesSorted = scaleNotes.Distinct();

        //    List<NoteLetter> arpeggio;
        //    // Figure out the notes that each arpeggio is made of
        //    foreach (var note in uniqueNotesSorted)
        //    {
        //        arpeggio = new List<NoteLetter>();

        //        foreach (var otherNote in uniqueNotesSorted)
        //        {
        //            foreach (var interval in intervals)
        //            {
        //                if ((note - otherNote) == (int)interval)
        //                {
        //                    arpeggio.Add(otherNote.Letter);
        //                }
        //            }
        //        }

        //        allArpeggios.Add(arpeggio);
        //    }

        //    return allArpeggios;
        //}
    }
}