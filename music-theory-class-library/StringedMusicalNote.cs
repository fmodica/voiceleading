using System;
using HelperExtensions;

namespace MusicTheory
{
    public class StringedMusicalNote : MusicalNote, IEquatable<StringedMusicalNote>
    {
        public MusicalNote String { get; private set; }
        public int Fret { get; private set; }

        public StringedMusicalNote(MusicalNote note, MusicalNote @string, int fret) : base(note.Letter, note.Octave)
        {
            @string.ValidateIsNotNull(nameof(@string));
            String = @string;
            Fret = fret;
        }

        public override bool Equals(object other)
        {
            if (other is StringedMusicalNote)
            {
                return _Equals(other as StringedMusicalNote);
            }

            return false;
        }

        public bool Equals(StringedMusicalNote other)
        {
            return _Equals(other);
        }

        private bool _Equals(StringedMusicalNote other)
        {
            if (other == null)
            {
                return false;
            }

            return IntValue == other.IntValue &&
                   Fret == other.Fret &&
                   String.IntValue == other.String.IntValue;
        }

        public override int GetHashCode()
        {
            // Meh, but good enough
            return base.GetHashCode();
        }
    }
}