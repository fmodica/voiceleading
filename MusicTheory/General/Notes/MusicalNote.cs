using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicTheory
{
    public class MusicalNote : IComparable<MusicalNote>
    {
        public NoteLetter Letter { get; set; }
        public int Octave { get; set; }

        public int IntValue
        {
            get
            {
                return ToInt();
            }
        }

        // Some useful overloads
        public override int GetHashCode()
        {
            return this.ToInt();
        }

        public override bool Equals(object obj)
        {
            if (obj is MusicalNote)
            {
                return (this.ToInt() == ((MusicalNote)obj).ToInt());
            }
            else
            {
                return false;
            }
        }

        public static int operator +(MusicalNote one, MusicalNote two)
        {
            return one.ToInt() + two.ToInt();
        }

        public static int operator -(MusicalNote one, MusicalNote two)
        {
            return Math.Abs(one.ToInt() - two.ToInt());
        }

        private int ToInt()
        {
            return (int)this.Letter + (12 * this.Octave);
        }

        int IComparable<MusicalNote>.CompareTo(MusicalNote other)
        {
            return this.ToInt().CompareTo(other.ToInt());
        }
    }
}