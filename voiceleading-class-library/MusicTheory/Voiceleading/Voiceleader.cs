using System;
using System.Collections.Generic;
using System.Linq;
using Instruments;
using System.Threading;
using System.Threading.Tasks;

namespace MusicTheory.Voiceleading
{
    public class Voiceleader
    {
        public List<VoicingSet> VoicingSets { get; private set; } = new List<VoicingSet>();

        private Config Config { get; set; }
        private List<NoteLetter> TargetChordNoteLetters { get; set; } = new List<NoteLetter>();
        private HashSet<NoteLetter?> RequiredTargetChordNoteLetters { get; set; } = new HashSet<NoteLetter?>();
        private MusicalNote HighestNoteInStartChord { get; set; }
        private MusicalNote SecondHighestNoteInStartChord { get; set; }
        private MusicalNote LowestNoteInStartChord { get; set; }
        private MusicalNote SecondLowestNoteInStartChord { get; set; }
        private CancellationTokenSource TokenSource { get; set; }
        private List<List<StringedMusicalNote>> ValidTargetNotesOnEachString { get; set; } = new List<List<StringedMusicalNote>>();
        private List<Chord<StringedMusicalNote>> CalculatedFingerings { get; set; } = new List<Chord<StringedMusicalNote>>();

        public Voiceleader(Config config)
        {
            Config = config;
            Config.FretToStayAtOrBelow = Config.FretToStayAtOrBelow ?? Config.StringedInstrument.NumFrets;
            Config.FretToStayAtOrAbove = Config.FretToStayAtOrAbove ?? 0;
            ConfigValidator.Validate(Config);

            var distinctOrderedStartNotes = Config.StartChord.Notes.Distinct().OrderByDescending(x => x.IntValue).ToList();
            HighestNoteInStartChord = distinctOrderedStartNotes[0];
            SecondHighestNoteInStartChord = distinctOrderedStartNotes.Count > 1 ? distinctOrderedStartNotes[1] : null;
            LowestNoteInStartChord = distinctOrderedStartNotes[distinctOrderedStartNotes.Count - 1];
            SecondLowestNoteInStartChord = distinctOrderedStartNotes.Count > 1 ? distinctOrderedStartNotes[distinctOrderedStartNotes.Count - 2] : null;
        }

        public async Task CalculateVoicings()
        {
            CalculateTargetChordLetters();
            CalculateRequiredNoteLetters();
            CalculateValidTargetNotesOnEachString();
            await CalculateWithTimeout();
            GroupFingeringsIntoVoicingSets();
        }

        private void CalculateTargetChordLetters()
        {
            TargetChordNoteLetters = Config.TargetChordRoot.GetLettersOfChord(Config.TargetChordIntervalOptionalPairs.Select(o => o.Interval)).ToList();
        }

        private void CalculateRequiredNoteLetters()
        {
            for (var i = 0; i < TargetChordNoteLetters.Count; i++)
            {
                if (!Config.TargetChordIntervalOptionalPairs[i].IsOptional)
                {
                    RequiredTargetChordNoteLetters.Add(TargetChordNoteLetters[i]);
                }
            }
        }

        // Given a list of target notes like [F, A, C] we find the location of each note on the fretboard.
        // Then we filter out notes that do not meet constraints, such as not leading back to any StartChord 
        // note via good voiceleading, being too low/high on the neck, etc. Ultimately we end up with 
        // ValidTargetNotesOnEachString, which holds each target-note possibility for each string. Later we 
        // loop over each string, finding each combination.

        // Example (6-string guitar, standard tuning, major-second voiceleading max, target chord = F maj):
        //
        // Start-Chord    ValidTargetNotesOnEachString
        // 5              [1, 5]
        // 6              [6, 10]
        // X              [10, 14]
        // X              [15, 19]
        // 5              [3, 20, 24]
        // X              [8]
        private void CalculateValidTargetNotesOnEachString()
        {
            ValidTargetNotesOnEachString = CreateMapFromStringToTargetNotes().Values.Select(x => x.ToList()).ToList();
        }

        private Dictionary<MusicalNote, HashSet<StringedMusicalNote>> CreateMapFromStringToTargetNotes()
        {
            var mapFromStringToTargetNotes = new Dictionary<MusicalNote, HashSet<StringedMusicalNote>>();

            foreach (var targetChordLetter in TargetChordNoteLetters)
            {
                var validTargetNotesForThisNoteLetter = FilterNotesAgainstVoiceleadingOptions(Config.StringedInstrument.GetNotesOnInstrument(targetChordLetter));

                MergeIntoMapFromStringToTargetNotes(mapFromStringToTargetNotes, validTargetNotesForThisNoteLetter);
            }

            return mapFromStringToTargetNotes;
        }

        private static void MergeIntoMapFromStringToTargetNotes(Dictionary<MusicalNote, HashSet<StringedMusicalNote>> mapFromStringToTargetNotes, IEnumerable<StringedMusicalNote> validTargetNotesForOneNote)
        {
            foreach (var note in validTargetNotesForOneNote)
            {
                AddNoteToMapFromStringToTargetNotes(mapFromStringToTargetNotes, note);
            }
        }

        private static void AddNoteToMapFromStringToTargetNotes(Dictionary<MusicalNote, HashSet<StringedMusicalNote>> mapFromStringToTargetNotes, StringedMusicalNote note)
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

        private IEnumerable<StringedMusicalNote> FilterNotesAgainstVoiceleadingOptions(IEnumerable<StringedMusicalNote> notes)
        {
            var validNotes = new List<StringedMusicalNote>();

            foreach (var note in notes)
            {
                if (note.Fret > Config.FretToStayAtOrBelow) continue;

                if (note.Fret < Config.FretToStayAtOrAbove)
                {
                    if (note.Fret != 0 || (note.Fret == 0 && Config.FilterOutOpenNotes)) continue;
                }

                foreach (var startNote in Config.StartChord.Notes)
                {
                    if (!HasGoodVoiceleading(startNote, note)) continue;

                    validNotes.Add(note);
                }
            }

            return validNotes;
        }

        private bool HasGoodVoiceleading(MusicalNote startNote, MusicalNote endNote)
        {
            // If the highest note can travel it can go as high as possible.
            // But it cannot go lower than the 2nd highest note - the max
            // voiceleading jump.
            if (Config.HighestNoteCanTravel && startNote.Equals(HighestNoteInStartChord))
            {
                return endNote.IntValue >= GetLowerLimitOfHighestNoteThatCanJump();
            }

            // If there is a highest required note letter, then the highest note
            // can only jump as high as possible if the end note is that required
            // note letter. Otherwise, it has to satisfy the basic criteria for
            // all other end notes.
            if (Config.HighestNote != null && startNote.Equals(HighestNoteInStartChord))
            {
                return endNote.Letter == Config.HighestNote ? endNote.IntValue >= GetLowerLimitOfHighestNoteThatCanJump() : SatisfiesBasicVoiceleadingJumpCriteria(startNote, endNote);
            }

            if (Config.LowestNoteCanTravel && startNote.Equals(LowestNoteInStartChord))
            {
                return endNote.IntValue <= GetUpperLimitOfLowestNoteThatCanJump();
            }

            return SatisfiesBasicVoiceleadingJumpCriteria(startNote, endNote);
        }

        private int GetUpperLimitOfLowestNoteThatCanJump()
        {
            return SecondLowestNoteInStartChord == null
                                ? Config.StringedInstrument.Tuning.Max(x => x.IntValue)
                                : (SecondLowestNoteInStartChord.IntValue + (int)Config.MaxVoiceleadingDistance);
        }

        private bool SatisfiesBasicVoiceleadingJumpCriteria(MusicalNote startNote, MusicalNote endNote)
        {
            return Math.Abs(endNote.IntValue - startNote.IntValue) <= (int)Config.MaxVoiceleadingDistance;
        }

        private int GetLowerLimitOfHighestNoteThatCanJump()
        {
            return SecondHighestNoteInStartChord == null
                ? Config.StringedInstrument.Tuning.Min(x => x.IntValue)
                : (SecondHighestNoteInStartChord.IntValue - (int)Config.MaxVoiceleadingDistance);
        }

        private async Task CalculateWithTimeout()
        {
            await Task.Run(() =>
            {
                CalculateRecursive(0, new List<StringedMusicalNote>(), new HashSet<int>(), new CancellationTokenSource(Config.CalculationTimeoutInMilliseconds).Token);
            });
        }

        private void CalculateRecursive(int currentStringIndex, IEnumerable<StringedMusicalNote> chordInProgress, HashSet<int> requiredNoteLettersObtained, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            // For each string on the instrument, we have a list of available notes, including a null which 
            // represents not playing anything on that string. 
            //
            // ValidTargetNotesOnEachString:
            // [null, 1, 5]
            // [null, 6, 10]
            // [null, 10, 14]
            // [null, 15, 19]
            // [null, 3, 20, 24]
            // [null, 8]
            //
            // Each of those notes (other than null) leads back to one or more StartChord notes with good 
            // voiceleading. Take every combination of a note (or null) from string 1, a note (or null) from 
            // string 2, etc. Along the way we can determine whether the chord we are building could ever 
            // meet all constraints. If it cannot, we break out of that level of the loop, avoiding all 
            // levels below and improving performance.

            var numStringsLeftIncludingThis = ValidTargetNotesOnEachString.Count - currentStringIndex;
            var numRequiredNotesLeft = RequiredTargetChordNoteLetters.Count - requiredNoteLettersObtained.Count;

            // If there is no possibility of hitting all required notes given how many strings are left, 
            // get out so we can start the next iteration of the string above. 
            if (numStringsLeftIncludingThis < numRequiredNotesLeft)
            {
                return;
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
                    if (RequiredTargetChordNoteLetters.Contains(note.Letter))
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

                    if (Config.HighestNote != null)
                    {
                        var highestNoteInChord = chordInProgressUpdated.OrderByDescending(x => x.IntValue).First();

                        if ((Config.HighestNote != highestNoteInChord.Letter))
                        {
                            continue;
                        }
                    }

                    // If the chord made it this far, it had at most one more required note
                    // to go, and may have just obtained it. So if its length is numRequiredNotes,
                    // we are good. It will never exceed numRequiredNotes because we are using a hash sets.
                    if (requiredNoteLettersObtainedUpdated.Count != RequiredTargetChordNoteLetters.Count)
                        continue;

                    // If melody splitting/convergence is allowed, we just need to make sure all start 
                    // notes have been matched
                    if (GetStartNotesMatched(chordInProgressUpdated).Count != Config.StartChord.Notes.Count)
                        continue;

                    // Chord is acceptable
                    CalculatedFingerings.Add(new Chord<StringedMusicalNote>(chordInProgressUpdated.ToList()));
                }
                else
                {
                    CalculateRecursive(currentStringIndex + 1, chordInProgressUpdated, requiredNoteLettersObtainedUpdated, token);
                }
            }
        }

        private void GroupFingeringsIntoVoicingSets()
        {
            var map = new Dictionary<string, List<Chord<StringedMusicalNote>>>();

            foreach (var chord in CalculatedFingerings)
            {
                var key = chord.ToSortedPitchString();

                if (map.ContainsKey(key))
                {
                    map[key].Add(chord);
                }
                else
                {
                    map[key] = new List<Chord<StringedMusicalNote>>() { chord };
                }
            }

            foreach (var kvp in map)
            {
                VoicingSets.Add(new VoicingSet(kvp.Value, Config.StartChord));
            }
        }

        private List<MusicalNote> GetStartNotesMatched(IEnumerable<MusicalNote> chord)
        {
            var startNotesMatched = new List<MusicalNote>();

            foreach (var startNote in Config.StartChord.Notes)
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

            if ((max - min) > Config.MaxFretsToStretch)
            {
                return true;
            }

            return false;
        }
    }
}