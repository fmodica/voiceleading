using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicTheory
{
    public class StringedMusicalNote : MusicalNote
    {
        public MusicalNote StringItsOn { get; set; }
        public int Fret { get; set; }
    }
}
