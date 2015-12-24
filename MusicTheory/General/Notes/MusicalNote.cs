using System;

namespace MusicTheory
{
    public class MusicalNote : IComparable<MusicalNote>
    {
        public NoteLetter Letter { get; set; }
        public int Octave { get; set; }

        public int IntValue
        {
            get { return (int) Letter + (12*Octave); }
        }

        public override int GetHashCode()
        {
            return IntValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is MusicalNote)
            {
                return IntValue == ((MusicalNote) obj).IntValue;
            }

            return false;
        }

        public static int operator +(MusicalNote one, MusicalNote two)
        {
            return one.IntValue + two.IntValue;
        }

        public static int operator -(MusicalNote one, MusicalNote two)
        {
            return one.IntValue - two.IntValue;
        }

        // For .Min() and .Max()
        int IComparable<MusicalNote>.CompareTo(MusicalNote other)
        {
            return IntValue.CompareTo(other.IntValue);
        }
    }
}