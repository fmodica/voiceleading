using System;
using System.Collections.Generic;
using System.Linq;
using Instruments;

namespace MusicTheory.Voiceleading
{
    public class SIVoiceleader
    {
        // Constructor parameters
        private StringedInstrument StringedInstrument { get; set; }
        private List<MusicalNote> StartingChordNotes { get; set; }
        private List<IntervalOptionalPair> TargetChordIntervalOptionalPairs { get; set; }
        private NoteLetter? TargetChordRoot { get; set; }
        private Interval? MaxVoiceleadingDistance { get; set; }
        private int? MaxFretsToStretch { get; set; }
        private int? FretToStayAtOrBelow { get; set; }
        private int? FretToStayAtOrAbove { get; set; }
        private NoteLetter? HighestRequiredNoteLetter { get; set; }
        private bool HighestNoteCanTravel { get; set; }
        private bool LowestNoteCanTravel { get; set; }
        private bool FilterOutOpenNotes { get; set; }

        // To be calculated
        private List<Chord> ChordMovements { get; set; } = new List<Chord>();
        public IEnumerable<SIVoicingSet> VoicingSets { get; private set; } = new List<SIVoicingSet>();
        public bool VoicingsLimitReached { get; private set; }
        public int MaxVoicings
        {
            get { return 20000; }
        }

        private HashSet<NoteLetter?> RequiredNotes { get; set; } = new HashSet<NoteLetter?>();
        private List<List<StringedMusicalNote>> ValidTargetNotesOnEachString { get; set; } = new List<List<StringedMusicalNote>>();
        // These letters will be in the same order as TargetChordIntervalOptionalPairs.
        private List<NoteLetter> TargetChordLetters { get; set; } = new List<NoteLetter>();
        private MusicalNote HighestNoteInStartingChord { get; set; }
        private MusicalNote SecondHighestNoteInStartingChord { get; set; }
        private MusicalNote LowestNoteInStartingChord { get; set; }
        private MusicalNote SecondLowestNoteInStartingChord { get; set; }

        public SIVoiceleader(SIVoiceleaderConfig config)
        {
            SIVoiceleaderConfigValidator.Validate(config);

            StringedInstrument = config.StringedInstrument;
            StartingChordNotes = config.StartingChordNotes;
            TargetChordIntervalOptionalPairs = config.TargetChordIntervalOptionalPairs;
            TargetChordRoot = config.EndChordRoot;
            MaxVoiceleadingDistance = config.MaxVoiceleadingDistance;
            MaxFretsToStretch = config.MaxFretsToStretch;
            FretToStayAtOrBelow = config.FretToStayAtOrBelow ?? StringedInstrument.NumFrets;
            FretToStayAtOrAbove = config.FretToStayAtOrAbove ?? 0;
            HighestRequiredNoteLetter = config.HighestNote;
            HighestNoteCanTravel = config.HighestNoteCanTravel;
            LowestNoteCanTravel = config.LowestNoteCanTravel;
            FilterOutOpenNotes = config.FilterOutOpenNotes;

            var distinctOrderedStartNotes = StartingChordNotes.Distinct().OrderByDescending(x => x.IntValue).ToList();

            HighestNoteInStartingChord = distinctOrderedStartNotes[0];
            SecondHighestNoteInStartingChord = distinctOrderedStartNotes.Count > 1 ? distinctOrderedStartNotes[1] : null;
            LowestNoteInStartingChord = distinctOrderedStartNotes[distinctOrderedStartNotes.Count - 1];
            SecondLowestNoteInStartingChord = distinctOrderedStartNotes.Count > 1 ? distinctOrderedStartNotes[distinctOrderedStartNotes.Count - 2] : null;
        }

        public void CalculateVoicings()
        {
            CalculateTargetChordLetters();
            CalculateRequiredNoteLetters();
            CalculateValidTargetNotesOnEachString();
            VoicingsLimitReached = !CalculateVoicingsRecursive(0, new List<StringedMusicalNote>(), new HashSet<int>());
            OrganizeVoicingsByPitchSet();
        }

        private void CalculateTargetChordLetters()
        {
            TargetChordLetters = TargetChordRoot.GetLettersOfChord(TargetChordIntervalOptionalPairs.Select(o => o.Interval)).ToList();
        }

        private void CalculateRequiredNoteLetters()
        {
            // This assumes TargetChordLetters and TargetChordIntervalOptionalPairs are in the same order.
            for (var i = 0; i < TargetChordLetters.Count; i++)
            {
                if (!TargetChordIntervalOptionalPairs[i].IsOptional)
                {
                    RequiredNotes.Add(TargetChordLetters[i]);
                }
            }
        }

        // Target notes will be eliminated based on constraints (e.g. if small leaps cannot occur to the note
        // from any start-chord note, if fret location is too high or low on the neck, etc.). Ultimately we end 
        // up with ValidTargetNotesOnEachString, which holds each target-note possibility for each string which we 
        // will later loop over, finding every combination.

        // Example (6-string guitar, standard tuning, major-second voiceleading max, target chord = F maj):
        //
        // Start-Chord     Target-Chord matches (ValidTargetNotesOnEachString)
        //     5                  [1, 5]
        //     6                  [6, 10]
        //     X                  [10, 14]
        //     X                  [15, 19]
        //     5                  [3, 20, 24]
        //     X                  [8]
        private void CalculateValidTargetNotesOnEachString()
        {
            var mapFromStringToTargetNotes = new Dictionary<MusicalNote, HashSet<StringedMusicalNote>>();

            foreach (var targetChordLetter in TargetChordLetters)
            {
                var validTargetNotesForThisNoteLetter = FilterNotesAgainstVoiceleadingOptions(StringedInstrument.GetNotesOnInstrument(targetChordLetter));

                MergeIntoMapFromStringToTargetNotes(mapFromStringToTargetNotes, validTargetNotesForThisNoteLetter);
            }

            ValidTargetNotesOnEachString = mapFromStringToTargetNotes.Values.Select(x => x.ToList()).OrderBy(x => x.Count).ToList();
        }

        private static void MergeIntoMapFromStringToTargetNotes(Dictionary<MusicalNote, HashSet<StringedMusicalNote>> mapFromStringToTargetNotes, IEnumerable<StringedMusicalNote> validTargetNotesForOneNoteLetter)
        {
            foreach (var note in validTargetNotesForOneNoteLetter)
            {
                if (!mapFromStringToTargetNotes.ContainsKey(note.StringItsOn))
                {
                    // Add a null to represent the possibility of a note not being played on that string
                    mapFromStringToTargetNotes[note.StringItsOn] = new HashSet<StringedMusicalNote>() { null, note };
                }
                else
                {
                    mapFromStringToTargetNotes[note.StringItsOn].Add(note);
                }
            }
        }

        private IEnumerable<StringedMusicalNote> FilterNotesAgainstVoiceleadingOptions(IEnumerable<StringedMusicalNote> notes)
        {
            var validNotes = new List<StringedMusicalNote>();

            foreach (var note in notes)
            {
                if (note.Fret > FretToStayAtOrBelow)
                    continue;

                if (note.Fret < FretToStayAtOrAbove)
                {
                    if (note.Fret != 0 || (note.Fret == 0 && FilterOutOpenNotes))
                        continue;
                }

                foreach (var startNote in StartingChordNotes)
                {
                    if (!HasGoodVoiceleading(startNote, note))
                        continue;

                    validNotes.Add(note);
                }
            }

            return validNotes;
        }

        private bool HasGoodVoiceleading(MusicalNote startNote, MusicalNote endNote)
        {
            // If the highest note must be a certain letter, then we assume the highest
            // note in the start chord is a melody and can jump. It can go as high as it wants,
            // but not lower than the second highest - voice leading limit.
            if ((HighestNoteCanTravel || HighestRequiredNoteLetter != null) && startNote.Equals(HighestNoteInStartingChord))
            {
                var lowerLimit = SecondHighestNoteInStartingChord == null
                    ? StringedInstrument.Tuning.Min(x => x.IntValue)
                    : (SecondHighestNoteInStartingChord.IntValue - (int)MaxVoiceleadingDistance);

                return endNote.IntValue >= lowerLimit;
            }

            if (LowestNoteCanTravel && startNote.Equals(LowestNoteInStartingChord))
            {
                var higherLimit = SecondLowestNoteInStartingChord == null
                    ? StringedInstrument.Tuning.Max(x => x.IntValue)
                    : (SecondLowestNoteInStartingChord.IntValue - (int)MaxVoiceleadingDistance);

                return endNote.IntValue <= higherLimit;
            }

            return Math.Abs(endNote - startNote) <= (int)MaxVoiceleadingDistance;
        }

        // return false if the voicings limit has been reached
        private bool CalculateVoicingsRecursive(int currentStringIndex, IEnumerable<StringedMusicalNote> chordInProgress, HashSet<int> requiredNoteLettersObtained)
        {
            // For each string on the instrument, we have a list of available notes, including
            // a null which represents not playing anything on that string. 
            //
            // ValidTargetNotesOnEachString:
            // [null, 1, 5]
            // [null, 6, 10]
            // [null, 10, 14]
            // [null, 15, 19]
            // [null, 3, 20, 24]
            // [null, 8]
            //
            // Each of those notes (other than null) leads back to one or more start notes with good voiceleading
            // (and does not violate any other constraints). Take every combination of a note (or null) from string 1, 
            // a note (or null) from string 2, etc. Along the way we can determine whether the chord in progress will
            // ever meet all requirements (such as fret width, the possibility of it ending up with all required notes,
            // etc.) and either continue or break. These predictions make the algorithm very efficient.

            var numStringsLeftIncludingThis = ValidTargetNotesOnEachString.Count - currentStringIndex;
            var numRequiredNotesLeft = RequiredNotes.Count - requiredNoteLettersObtained.Count;

            // If there is no possibility of hitting all required notes given how many strings are left, 
            // get out so we can start the next iteration of the string above. 
            if (numStringsLeftIncludingThis < numRequiredNotesLeft)
            {
                return true;
            }

            var thisString = ValidTargetNotesOnEachString[currentStringIndex];

            foreach (var note in thisString)
            {
                // This hashset will hold the data passed down as well as data for the note
                // at this level. A copy must be made so we don't overwrite data
                // which needs to stay the same inside this loop.
                var requiredNoteLettersObtainedUpdated = new HashSet<int>();
                IEnumerable<StringedMusicalNote> chordInProgressUpdated;

                if (note != null)
                {
                    if (RequiredNotes.Contains(note.Letter))
                    {
                        requiredNoteLettersObtainedUpdated.Add((int)note.Letter);
                    }

                    chordInProgressUpdated = new List<StringedMusicalNote>() { note }.Concat(chordInProgress);
                }
                else
                {
                    // If there is no note being played on this string just copy the chord in progress
                    chordInProgressUpdated = new List<StringedMusicalNote>(chordInProgress);
                }

                // Update the collection that tracks required notes obtained
                requiredNoteLettersObtainedUpdated.UnionWith(requiredNoteLettersObtained);

                // If the chord in progress is too wide so far, continue to the next iteration 
                if (ChordIsTooWide(chordInProgressUpdated))
                    continue;

                if (currentStringIndex == ValidTargetNotesOnEachString.Count - 1)
                {
                    if (!chordInProgressUpdated.Any())
                    {
                        continue;
                    }

                    if (HighestRequiredNoteLetter != null)
                    {
                        var highestNoteInChord = chordInProgressUpdated.OrderByDescending(x => x.IntValue).First();

                        if ((HighestRequiredNoteLetter != highestNoteInChord.Letter))
                        {
                            continue;
                        }
                    }

                    // If the chord made it this far, it had at most one more required note
                    // to go, and may have just obtained it. So if its length is numRequiredNotes,
                    // we are good. It will never exceed numRequiredNotes because we are using a hash sets.
                    if (requiredNoteLettersObtainedUpdated.Count != RequiredNotes.Count)
                        continue;

                    // If melody splitting/convergence is allowed, we just need to make sure all start 
                    // notes have been matched
                    if (GetStartNotesMatched(chordInProgressUpdated).Count != StartingChordNotes.Count)
                        continue;

                    // Chord is acceptable
                    ChordMovements.Add(new Chord(chordInProgressUpdated.ToList()));

                    if (ChordMovements.Count >= MaxVoicings)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!CalculateVoicingsRecursive(currentStringIndex + 1, chordInProgressUpdated, requiredNoteLettersObtainedUpdated))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void OrganizeVoicingsByPitchSet()
        {
            VoicingSets = new SIVoicingSetGrouper(ChordMovements, new Chord(StartingChordNotes)).VoicingSets;
        }

        private List<MusicalNote> GetStartNotesMatched(IEnumerable<MusicalNote> chord)
        {
            var startNotesMatched = new List<MusicalNote>();

            foreach (var startNote in StartingChordNotes)
            {
                foreach (var endNote in chord)
                {
                    if (HasGoodVoiceleading(startNote, endNote))
                    {
                        startNotesMatched.Add(startNote);
                        break;
                    }
                }
            }

            return startNotesMatched;
        }

        private bool ChordIsTooWide(IEnumerable<StringedMusicalNote> chord)
        {
            var frets = new HashSet<int>();

            foreach (StringedMusicalNote note in chord)
            {
                frets.Add(note.Fret);
            }

            var withoutOpens = frets.Where(fret => (fret != 0));

            // If they were all open notes, this cannot be too wide
            if (!withoutOpens.Any())
                return false;

            // Could compute this in loop above for performance
            int max = withoutOpens.Max();
            int min = withoutOpens.Min();

            if ((max - min) > MaxFretsToStretch)
            {
                return true;
            }

            return false;
        }
    }
}