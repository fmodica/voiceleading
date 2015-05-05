using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MusicTheory;
using MusicTheory.Voiceleading;
using Instruments;

namespace MusicTheory.Voiceleading
{
    /// <summary>
    /// Handles all operations related to generating voiceleading movements on a stringed instrument.
    /// </summary>
    public class SIVoiceleader
    {
        // Constructor parameters
        private List<MusicalNote> StartingChordNotes { get; set; }
        private List<IntervalOptionalPair> TargetChordIntervalOptionalPairs { get; set; }
        private NoteLetter TargetChordRoot { get; set; }
        private Interval MaxVoiceleadingDistance { get; set; }
        private int MaxFretsToStretch { get; set; }
        //private bool MelodySplittingAndConvergenceAllowed { get; set; }
        private int? FretToStayAtOrBelow { get; set; }
        private int? FretToStayAtOrAbove { get; set; }
        private StringedInstrument StringedInstrument { get; set; }
        private bool RandomVoiceleading { get; set; }
        private NoteLetter? HighestNote { get; set; }

        // To be calculated
        private HashSet<NoteLetter> RequiredNotes { get; set; }
        private List<List<StringedMusicalNote>> TargetNoteOptionsPerString { get; set; }
        public List<List<StringedMusicalNote>> ChordMovements { get; set; }
        public Dictionary<HashSet<MusicalNote>, SIVoicingSet> ChordMovementsOrganizedByPitchSet { get; set; }

        // Extra
        public int MaxVoicings { get { return 20000; } }
        public bool VoicingsLimitReached { get; private set; }

        // Constructor
        // The parameters should be made into a single config object
        public SIVoiceleader(SIVoiceleaderConfig config)
        {
            MaxVoiceleadingDistance = config.MaxVoiceleadingDistance;
            RandomVoiceleading = config.RandomVoiceleading;
            HighestNote = config.HighestNote;

            if (config.StartingChordNotes == null || config.StartingChordNotes.Count == 0)
            {
                throw new ArgumentException("You must provide some notes for the starting chord");
            }

            // Have the lowest pitch note be first so we can identify it in case we want to 
            // allow it to move by more than the voiceleading limit. ToInt() has been overridden
            // for MusicalNote objects to produce a unique value.
            StartingChordNotes = config.StartingChordNotes.OrderBy(o => o.IntValue).ToList();

            if (config.StringedInstrument == null)
            {
                throw new ArgumentException("You must provide a stringed instrument.");
            }

            StringedInstrument = config.StringedInstrument;

            if (StringedInstrument.Tuning.Distinct().Count() != StringedInstrument.Tuning.Count)
            {
                throw new ArgumentException("The algorithm cannot currently compute voiceleading for a stringed instrument for which two or more strings are tuned to the same pitch.");
            }

            // This is an enum (value type), check for null not necessary
            TargetChordRoot = config.EndChordRoot;

            if ((RandomVoiceleading == false) && (config.TargetChordIntervalOptionalPairs == null || config.TargetChordIntervalOptionalPairs.Count == 0))
            {
                throw new ArgumentException("You must provide some interval-optional pairs for the new chord");
            }

            if ((RandomVoiceleading == false) && (config.TargetChordIntervalOptionalPairs.Select(o => o.Interval).Distinct().Count() != config.TargetChordIntervalOptionalPairs.Count))
            {
                throw new ArgumentException("The list of intervals cannot contain duplicates.");
            }

            TargetChordIntervalOptionalPairs = config.TargetChordIntervalOptionalPairs;

            if ((config.MaxFretsToStretch > StringedInstrument.NumFrets))
            {
                throw new ArgumentException("The maximum frets to stretch is greater than the number of frets on the instrument");
            }

            MaxFretsToStretch = config.MaxFretsToStretch;

            FretToStayAtOrBelow = config.FretToStayAtOrBelow ?? StringedInstrument.NumFrets;
            FretToStayAtOrAbove = config.FretToStayAtOrAbove ?? 0;

            if (FretToStayAtOrBelow < 0)
            {
                throw new ArgumentOutOfRangeException("The fret to stay at or below must be at least zero (open note)");
            }

            if (FretToStayAtOrAbove > StringedInstrument.NumFrets)
            {
                throw new ArgumentOutOfRangeException("The fret to stay at or above cannot be greater than the number of frets on the instrument");
            }

            if ((MaxVoiceleadingDistance < 0) || MaxVoiceleadingDistance > Interval.Fourth)
            {
                throw new ArgumentException("The maximum voiceleading is negative or greater than an interval of a fourth. The point of voiceleading is not to jump by large intervals.");
            }

            // Initialize collections
            RequiredNotes = new HashSet<NoteLetter>();
            TargetNoteOptionsPerString = new List<List<StringedMusicalNote>>();
            ChordMovements = new List<List<StringedMusicalNote>>();
            ChordMovementsOrganizedByPitchSet = new Dictionary<HashSet<MusicalNote>, SIVoicingSet>();

            GetFretboardLocationsOfValidTargetNotes();
        }

        // Invalid notes will be eliminated based on constraints (e.g. if small leaps cannot occur to the note
        // from any start-chord note, if fret location is too high or low on the neck, etc.). Ultimately we end up with 
        // TargetNoteOptionsPerString, which holds each target-note possibility for each string which we will later 
        // loop over, finding every combination.

        // Example (6-string guitar, standard tuning, major-second voiceleading max, target chord = F maj):
        //
        // Start-Chord     Target-Chord matches (TargetNoteOptionsPerString)
        //     5                  [1, 5]
        //     6                  [6, 10]
        //     X                  [10, 14]
        //     X                  [15, 19]
        //     5                  [3, 20, 24]
        //     X                  [8]
        private void GetFretboardLocationsOfValidTargetNotes()
        {
            var allTargetNoteLocationsPerString = new Dictionary<MusicalNote, HashSet<StringedMusicalNote>>();

            // The note letters will be returned in the same order as the intervals (thus TargetChordIntervalOptionalPairs can be 
            // referenced by the same index as targetChordLetters)
            NoteLetter[] targetChordLetters;

            if (RandomVoiceleading)
            {
                // Any note can be used
                targetChordLetters = Enum.GetValues(typeof(NoteLetter)).Cast<NoteLetter>().ToArray();
            }
            else
            {
                targetChordLetters = TargetChordRoot.GetNoteLettersOfChord(TargetChordIntervalOptionalPairs.Select(o => o.Interval).ToArray());
            }

            // For each target note letter, find every valid location on the fretboard
            for (int i = 0; i < targetChordLetters.Length; i++)
            {
                NoteLetter targetNoteLetter = targetChordLetters[i];

                var availableLocationsForThisTargetNote = StringedInstrument.GetNotesByNoteLetter(targetNoteLetter);

                bool isOptional;

                if (RandomVoiceleading)
                {
                    // All are optional if voiceleaidng is random
                    isOptional = true;
                }
                else
                {
                    isOptional = TargetChordIntervalOptionalPairs[i].IsOptional;
                }

                if (!isOptional)
                    RequiredNotes.Add(targetNoteLetter);

                foreach (var endNote in availableLocationsForThisTargetNote)
                {
                    if (endNote.Fret > FretToStayAtOrBelow)
                        continue;

                    if ((endNote.Fret < FretToStayAtOrAbove) && endNote.Fret != 0)
                        continue;

                    // TEST - start notes are ordered by pitch.
                    // Let the lowest move by an octave.
                    //bool isLowest = true;
                    foreach (var startNote in StartingChordNotes)
                    {
                        // If this has good voiceleading...
                        // If we end up having an option to allow the bass note to 
                        // violate MaxVoiceleadingDistance, create a method for this
                        // if statement as it exists in multiple places.

                        /*if (
                            (isLowest && ((endNote - startNote) <= (int)Interval.Seventh + 1))
                        || (!isLowest && ((endNote - startNote) <= (int)MaxVoiceleadingDistance))
                            )
                        {*/

                        // The minus/plus operators have been overloaded for notes to subtract/add their pitch values
                        if ((endNote - startNote) <= (int)MaxVoiceleadingDistance)
                        {
                            if (allTargetNoteLocationsPerString.ContainsKey(endNote.StringItsOn))
                            {
                                allTargetNoteLocationsPerString[endNote.StringItsOn].Add(endNote);
                            }
                            else
                            {
                                // Throw in the null as well which represents no note played on that string
                                allTargetNoteLocationsPerString.Add(endNote.StringItsOn, new HashSet<StringedMusicalNote>() { null, endNote });
                            }
                        }
                        //isLowest = false;
                    }
                }
            }

            // OrderBy seems to give better performance for the voiceleading algorithm,
            // compared to OrderByDescending (about 20% faster), but need more tests. It
            // might be dependent on context.
            TargetNoteOptionsPerString = allTargetNoteLocationsPerString.Values.Select(o => o.ToList()).OrderBy(o => o.Count).ToList();
        }

        public void CalculateVoicings()
        {
            int numDimensions = TargetNoteOptionsPerString.Count();

            int startDimensionIndex = 0;

            VoicingsLimitReached = !CalculateVoicingsRecursive(startDimensionIndex, numDimensions, new List<StringedMusicalNote>(),
                                                               RequiredNotes.Count(), StartingChordNotes.Count(), new HashSet<int>());

            OrganizeVoicingsByPitchSet();
        }

        // return false if the voicings limit has been reached
        private bool CalculateVoicingsRecursive(int currentDimensionIndex, int numDimensions, IEnumerable<StringedMusicalNote> chordMovementInProgress, int numRequiredNotes, int numStartNotes, HashSet<int> requiredNoteLettersObtained)
        {
            // For each string on the instrument, we have a list of available notes, including
            // a null which represents not playing anything on that string. 
            //
            // Target-Chord matches (TargetNoteOptionsPerString)
            //     [1, 5, null]
            //     [6, 10, null]
            //     [10, 14, null]
            //     [15, 19, null]
            //     [3, 20, 24, null]
            //     [8, null]
            //
            // Each of those notes (other than null) leads back to one or more start notes with good voiceleading
            // (and does not violate any other constraints). Take every combination of a note (or null) from string 1, 
            // a note (or null) from string 2, etc. Along the way we can determine whether the chord in progress will
            // ever meet all requirements (such as fret width, the possibility of it ending up with all required notes,
            // etc.) and either continue or break. These predictions make the algorithm very efficient.

            int numDimensionsLeftIncludingThis = numDimensions - (currentDimensionIndex);

            int howManyMoreRequiredNotesNeeded = numRequiredNotes - requiredNoteLettersObtained.Count;

            // If there is no possibility of hitting all required notes given how many strings are left, 
            // get out so we can start the next iteration of the string above. 

            if (numDimensionsLeftIncludingThis < howManyMoreRequiredNotesNeeded)
                return true;

            var thisString = TargetNoteOptionsPerString[currentDimensionIndex];

            foreach (StringedMusicalNote thisNote in thisString)
            {
                // This hashset will hold the data passed down as well as data for the note
                // at this level. A copy must be made so we don't overwrite data
                // which needs to stay the same inside this loop.
                var requiredNoteLettersObtainedUpdated = new HashSet<int>();

                IEnumerable<StringedMusicalNote> chordMovementUpdated;

                if (thisNote != null)
                {
                    // If this note already exists in the chord in progress, continue to the next iteration because 
                    // that means two of the same exact note are on different strings.
                    // EDIT - a lot of cool chords have duplicate notes, such as a fretted note 
                    // which is the same as one of the open notes. So don't rule this out.
                    //if (chordMovementInProgress.Contains(thisNote))
                    //    continue;

                    int noteLetterValue = (int)thisNote.Letter;

                    if (RequiredNotes.Contains(thisNote.Letter))
                        requiredNoteLettersObtainedUpdated.Add(noteLetterValue);

                    chordMovementUpdated = new List<StringedMusicalNote>() { thisNote }.Concat(chordMovementInProgress);
                }
                else
                {
                    // If there is no note being played on this string just copy the chord in progress
                    chordMovementUpdated = new List<StringedMusicalNote>(chordMovementInProgress);
                }

                // Update the collection that tracks required notes obtained
                requiredNoteLettersObtainedUpdated.UnionWith(requiredNoteLettersObtained);

                // If the chord in progress is too wide so far, continue to the next iteration 
                if (ChordIsTooWide(chordMovementUpdated))
                    continue;

                if (currentDimensionIndex == numDimensions - 1)
                {
                    var highestNoteInChord = chordMovementUpdated.GetHighest();

                    if (HighestNote != null && highestNoteInChord != null)
                    {
                        if ((HighestNote != highestNoteInChord.Letter))
                        {
                            continue;
                        }
                    }

                    // If the chord made it this far, it had at most one more required note
                    // to go, and may have just obtained it. So if its length is numRequiredNotes,
                    // we are good. It will never exceed numRequiredNotes because we are using a hash sets.
                    if (requiredNoteLettersObtainedUpdated.Count != numRequiredNotes)
                        continue;

                    // If melody splitting/convergence is allowed, we just need to make sure all start 
                    // notes have been matched
                    if (GetStartNotesMatched(chordMovementUpdated).Count != numStartNotes)
                        continue;

                    // Chord is acceptable
                    ChordMovements.Add(chordMovementUpdated.ToList());

                    if (ChordMovements.Count >= MaxVoicings)
                    {
                        return false;
                    }
                }
                else
                {
                    /*numIterationsDiagnostic = */
                    // Just return the method result?
                    if (!CalculateVoicingsRecursive(currentDimensionIndex + 1, numDimensions, chordMovementUpdated, /*numIterationsDiagnostic,*/ numRequiredNotes, numStartNotes, /*startNoteMatchesUpdated*/ requiredNoteLettersObtainedUpdated /*,noteOctavePairsObtainedUpdated*/))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void OrganizeVoicingsByPitchSet()
        {
            // Create a dictionary where each key is a pitch set, and each value is a list
            // of chords with that pitch set. We use the SetEquals function to determine 
            // whether one hashset of pitches equals another. Thus the set:
            // {A5 (any string), C4 (any string)} is equal to the set:
            // {C4 (any string), A5 (any string)},
            // because the notes evaluate to the same integers, just in different orders (the 
            // strings the notes are on do not affect the integer values of the musical notes). 
            foreach (var chord in ChordMovements)
            {
                var noteValues = new HashSet<MusicalNote>(chord);

                bool matched = false;

                foreach (var alreadyStoredChord in ChordMovementsOrganizedByPitchSet.Keys)
                {
                    if (noteValues.SetEquals(alreadyStoredChord))
                    {
                        ChordMovementsOrganizedByPitchSet[alreadyStoredChord].Fingerings.Add(chord);
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    var voicingSet = new SIVoicingSet();
                    voicingSet.Fingerings.Add(chord);

                    ChordMovementsOrganizedByPitchSet.Add(noteValues, voicingSet);
                }
            }
        }

        private List<MusicalNote> GetStartNotesMatched(IEnumerable<MusicalNote> chord)
        {
            var startNotesMatched = new List<MusicalNote>();

            // compare each note in the chord to each
            // start note to see if there's a match
            //bool isLowest = true;

            foreach (var startNote in StartingChordNotes)
            {
                foreach (var endNote in chord)
                {
                    // If this has good voiceleading...
                    if ((endNote - startNote) <= (int)MaxVoiceleadingDistance)
                    {
                        startNotesMatched.Add(startNote);
                        break;
                    }
                }

                //isLowest = false;
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
            if (withoutOpens.Count() == 0)
                return false;
            else
            {
                int max = withoutOpens.Max();
                int min = withoutOpens.Min();

                if ((max - min) > MaxFretsToStretch)
                    return true;
                else
                    return false;
            }
        }
    }
}