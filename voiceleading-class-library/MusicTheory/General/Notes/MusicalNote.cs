using System;

namespace MusicTheory
{
    public class MusicalNote
    {
        public NoteLetter Letter { get; private set; }
        public int Octave { get; private set; }

        public MusicalNote(NoteLetter letter, int octave)
        {
            Letter = letter;
            Octave = octave;
        }

        public int IntValue
        {
            get
            {
                return (int)Letter + (12 * Octave);
            }
        }

        public override int GetHashCode()
        {
            return IntValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is MusicalNote)
            {
                return IntValue == ((MusicalNote)obj).IntValue;
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
    }
}