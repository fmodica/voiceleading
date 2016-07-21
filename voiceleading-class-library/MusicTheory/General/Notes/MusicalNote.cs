using System;

namespace MusicTheory
{
    public class MusicalNote : IEquatable<MusicalNote>
    {
        public NoteLetter Letter { get; private set; }
        public int Octave { get; private set; }

        public MusicalNote(NoteLetter? letter, int? octave)
        {
            if (letter == null)
            {
                throw new ArgumentNullException(nameof(letter));
            }

            if (octave == null)
            {
                throw new ArgumentNullException(nameof(octave));
            }

            Letter = letter.Value;
            Octave = octave.Value;
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

        public override string ToString()
        {
            return IntValue.ToString();
        }

        private bool _Equals(MusicalNote other)
        {
            if (other == null)
            {
                return false;
            }

            return IntValue == other.IntValue;
        }
    }
}