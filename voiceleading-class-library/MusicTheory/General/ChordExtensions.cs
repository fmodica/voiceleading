using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicTheory
{
    public static class ChordExtensions
    {
        public static IEnumerable<NoteLetter> GetLettersOfChord(this NoteLetter? chordRoot, IEnumerable<Interval> intervals)
        {
            if (chordRoot == null)
            {
                throw new ArgumentException(nameof(chordRoot) + " cannot be null.");
            }

            if (intervals == null)
            {
                throw new ArgumentException(nameof(intervals) + " cannot be null.");
            }

            var noteLetters = new List<NoteLetter>();
            var rootValue = (int)chordRoot;

            foreach (var interval in intervals)
            {
                var intervalDistance = (int)interval;
                var rootPlusIntervalIndex = rootValue + intervalDistance;

                if (rootPlusIntervalIndex > 11)
                {
                    rootPlusIntervalIndex -= 12;
                }

                NoteLetter? chordNote = (NoteLetter?)Enum.Parse(typeof(NoteLetter), rootPlusIntervalIndex.ToString());

                noteLetters.Add((NoteLetter)chordNote);
            }

            return noteLetters;
        }
    }
}