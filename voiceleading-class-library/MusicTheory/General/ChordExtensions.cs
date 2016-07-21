using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicTheory
{
    public static class Extensions
    {
        public static IEnumerable<NoteLetter> GetLettersOfChord(this NoteLetter? chordRoot, IEnumerable<Interval?> intervals)
        {
            if (chordRoot == null)
            {
                throw new ArgumentNullException(nameof(chordRoot));
            }

            if (intervals == null)
            {
                throw new ArgumentNullException(nameof(intervals));
            }

            if (!intervals.Any())
            {
                throw new ArgumentException("The collection is empty.", nameof(intervals));
            }

            if (intervals.Any(x => x == null))
            {
                throw new ArgumentNullException("An object in " + nameof(intervals) + " is null");
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