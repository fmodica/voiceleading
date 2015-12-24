using System.Text;

namespace MusicTheory
{
    public class StringedMusicalNote : MusicalNote
    {
        public MusicalNote StringItsOn { get; set; }
        public int Fret { get; set; }

        public override bool Equals(object other)
        {
            if (other is StringedMusicalNote)
            {
                var otherStringedNote = (StringedMusicalNote)other;

                if (otherStringedNote.StringItsOn == null)
                {
                    return false;
                }

                return
                    IntValue == otherStringedNote.IntValue &&
                    Fret == otherStringedNote.Fret &&
                    StringItsOn.IntValue == otherStringedNote.StringItsOn.IntValue;
            }

            return false;
        }

        public override int GetHashCode()
        {
            // Meh, but good enough
            return base.GetHashCode();
        }
    }
}
