using System;
using HelperExtensions;

namespace MusicTheory
{
    public class MusicalNote : IEquatable<MusicalNote>
    {
        public NoteLetter Letter { get; private set; }
        public int Octave { get; private set; }

        public MusicalNote(NoteLetter letter, int octave)
        {
            Letter = letter;
            Octave = octave;
        }

        public int IntValue => (int)Letter + (12 * Octave);

        public bool Equals(MusicalNote other)
        {
            return _Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (obj is MusicalNote)
            {
                return _Equals(obj as MusicalNote);
            }

            return false;
        }

        private bool _Equals(MusicalNote other)
        {
            return IntValue == other?.IntValue;
        }

        public override int GetHashCode()
        {
            return IntValue;
        }

        public override string ToString()
        {
            return IntValue.ToString();
        }
    }
}