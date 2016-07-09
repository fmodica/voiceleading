using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicTheory
{
    public class Chord
    {
        public List<MusicalNote> Notes { get; private set; } = new List<MusicalNote>();

        public Chord(MusicalNote note)
        {
            Notes.Add(note);
        }

        public Chord(IEnumerable<MusicalNote> notes)
        {
            Notes.AddRange(notes);
        }

        public override bool Equals(object obj)
        {
            if (obj is Chord)
            {
                return !Notes.Except(((Chord)obj).Notes).Any();
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToUniqueMusicalNoteString().GetHashCode();
        }

        public string ToUniqueMusicalNoteString()
        {
            if (Notes == null || !Notes.Any())
            {
                return string.Empty;
            }

            var str = new StringBuilder();

            foreach (var intValue in Notes.Select(x => x.IntValue).Distinct().OrderBy(x => x))
            {
                str.Append(intValue).Append(';');
            }

            return str.ToString();
        }
    }
}