using System;
using HelperExtensions;

namespace MusicTheory
{
    public class StringedMusicalNote : MusicalNote, IEquatable<StringedMusicalNote>
    {
        public MusicalNote StringItsOn { get; private set; }
        public int Fret { get; private set; }

        public StringedMusicalNote(MusicalNote note, MusicalNote stringItsOn, int fret) : base(note.Letter, note.Octave)
        {
            stringItsOn.ValidateIsNotNull(nameof(stringItsOn));
            StringItsOn = stringItsOn;
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
                   StringItsOn.IntValue == other.StringItsOn.IntValue;
        }

        public override int GetHashCode()
        {
            // Meh, but good enough
            return base.GetHashCode();
        }
    }
}