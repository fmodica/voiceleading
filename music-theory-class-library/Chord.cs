using HelperExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicTheory
{
    public class Chord<T> : IEquatable<Chord<T>> where T : MusicalNote
    {
        public List<T> Notes { get; }

        public Chord(T note)
        {
            note.ValidateIsNotNull(nameof(note));
            Notes = new List<T>() { note };
        }

        public Chord(IEnumerable<T> notes)
        {
            notes.ValidateIsNotNullOrEmptyOrHasNullItem(nameof(notes));
            Notes = new List<T>(notes);
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

        private bool _Equals(Chord<T> chord)
        {
            return !Notes.Except(chord?.Notes).Any();
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
            var str = new StringBuilder();

            Notes.Select(note => note.IntValue)
                .Distinct()
                .OrderBy(intValue => intValue)
                .ForEach(intValue => str.Append(intValue).Append(';'));

            return str.ToString();
        }
    }
}