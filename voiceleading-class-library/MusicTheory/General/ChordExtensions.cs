using System;

namespace MusicTheory
{
    public static class ChordExtensions
    {
        public static NoteLetter?[] GetLettersOfChord(this NoteLetter? chordRoot, Interval[] intervals)
        {
            if (chordRoot == null)
            {
                throw new ArgumentException(nameof(chordRoot) + " cannot be null.");
            }

            if (intervals == null)
            {
                throw new ArgumentException(nameof(intervals) + " cannot be null.");
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
    }
}