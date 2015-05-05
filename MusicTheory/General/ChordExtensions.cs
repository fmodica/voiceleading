using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MusicTheory;
using Instruments;

namespace MusicTheory
{
	/// <summary>
	/// A helper class for operations related to general music theory.
	/// </summary>
	public static class ChordExtensions
	{
		public static NoteLetter[] GetNoteLettersOfChord(this NoteLetter chordRoot, Interval[] intervals)
		{
            // chordRoot can never be null (will default to C)

            if (intervals == null)
            {
                throw new ArgumentException("The array of intervals cannot be null.");
            }

			var noteLetters = new NoteLetter[intervals.Length];

			int rootValue = (int)chordRoot;

			for (int i = 0; i < noteLetters.Length; i++)
			{
				int intervalDistance = (int)intervals[i];

				int rootPlusIntervalIndex = rootValue + intervalDistance;

				if (rootPlusIntervalIndex > 11)
					rootPlusIntervalIndex -= 12;

				NoteLetter chordNote = (NoteLetter)Enum.Parse(typeof(NoteLetter), rootPlusIntervalIndex.ToString());

				noteLetters[i] = chordNote;
			}

			return noteLetters;
		}

        // SI is the stringed instrument, scaleNotes gives the section of the instrument, and "intervals" specifices the kinds of arpeggios 
        // you want (triads, add9's, custom, etc.)
        public static List<List<NoteLetter>> GetArpeggiosInSISection(StringedInstrument SI, MusicalNote[] scaleNotes, Interval[] intervals)
        {
            // each List<NoteLetter> is an arpeggio
            var allArpeggios = new List<List<NoteLetter>>();

            var uniqueNotesSorted = scaleNotes.Distinct();

            List<NoteLetter> arpeggio;
            // Figure out the notes that each arpeggio is made of
            foreach (var note in uniqueNotesSorted)
            {
                arpeggio = new List<NoteLetter>();

                foreach (var otherNote in uniqueNotesSorted)
                {
                    foreach (var interval in intervals)
                    {
                        if ((note - otherNote) == (int)interval)
                        {
                            arpeggio.Add(otherNote.Letter);
                        }
                    }
                }

                allArpeggios.Add(arpeggio);
            }

            return allArpeggios;
        }

	    public static MusicalNote GetLowest(this IEnumerable<MusicalNote> notes)
	    {
            if (notes.Any())
            {
                // Remove any of the same notes like:
                // E: 0
                // B: 5
                return notes.Distinct().Min();
            }
            else
            {
                return null;
            }
	    }

	    public static MusicalNote GetHighest(this IEnumerable<MusicalNote> notes)
	    {
            if (notes.Any())
            {
                // Remove any of the same notes like:
                // E: 0
                // B: 5
                return notes.Distinct().Max();
            }
            else
            {
                return null;
            }
	    } 
	}
}