using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicTheory
{
    public class Chord<T> : IEquatable<Chord<T>> where T : MusicalNote
    {
        public List<T> Notes { get; private set; } = new List<T>();

        public Chord(T note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            Notes.Add(note);
        }

        public Chord(IEnumerable<T> notes)
        {
            if (notes == null)
            {
                throw new ArgumentNullException(nameof(notes));
            }

            if (!notes.Any())
            {
                throw new ArgumentException("The collection is empty.", nameof(notes));
            }

            if (notes.Any(x => x == null))
            {
                throw new ArgumentNullException("An object in " + nameof(notes) + " is null");
            }

            Notes.AddRange(notes);
        }

        public bool Equals(Chord<T> other)
        {
            return _Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (obj is Chord<T>)
            {
                return _Equals(obj as Chord<T>);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public string ToSortedPitchString()
        {
            return ToString();
        }

        public override string ToString()
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

        private bool _Equals(Chord<T> chord)
        {
            return !Notes.Except(chord?.Notes).Any();
        }
    }
}