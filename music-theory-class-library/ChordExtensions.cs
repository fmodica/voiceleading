using System;
using System.Collections.Generic;
using System.Linq;
using HelperExtensions;

namespace MusicTheory
{
    public static class ChordExtensions
    {
        public static IEnumerable<NoteLetter> GetLettersOfChord(this NoteLetter chordRoot, IEnumerable<Interval> intervals)
        {
            chordRoot.ValidateIsNotNull(nameof(chordRoot));
            intervals.ValidateIsNotNullOrEmpty(nameof(intervals));

            return intervals.Select(interval => GetChordNote(interval, (int) chordRoot));
        }

        private static NoteLetter GetChordNote(Interval interval, int rootValue)
        {
            var intervalIndex = rootValue + (int) interval;
            intervalIndex = intervalIndex > 11 ? intervalIndex - 12 : intervalIndex;

            return (NoteLetter) Enum.Parse(typeof(NoteLetter), intervalIndex.ToString());
        }
    }
}