using MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Voiceleading
{
    public class Voiceleader
    {
        public List<VoicingSet> VoicingSets { get; set; }

        private Config Config { get; set; }
        private List<NoteLetter> TargetChordNoteLetters { get; set; }
        private HashSet<NoteLetter> RequiredTargetChordNoteLetters { get; set; }
        private MusicalNote HighestNoteInStartChord { get; set; }
        private MusicalNote SecondHighestNoteInStartChord { get; set; }
        private MusicalNote LowestNoteInStartChord { get; set; }
        private MusicalNote SecondLowestNoteInStartChord { get; set; }
        private List<List<StringedMusicalNote>> ValidTargetNotesOnEachString { get; set; }
        private List<Chord<StringedMusicalNote>> CalculatedFingerings { get; set; }

        public Voiceleader(Config config)
        {
            ConfigValidator.Validate(config);
            Config = config;

            List<MusicalNote> distinctOrderedStartNotes = Config.StartChord.Notes.Distinct().OrderByDescending(note => note.IntValue).ToList();
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
            TargetChordNoteLetters = Config.TargetChordRoot
                .GetLettersOfChord(Config.TargetChordIntervalOptionalPairs.Select(o => o.Interval))
                .ToList();
        }

        private void CalculateRequiredNoteLetters()
        {
            RequiredTargetChordNoteLetters = new HashSet<NoteLetter>();

            for (var i = 0; i < TargetChordNoteLetters.Count; i++)
            {
                if (Config.TargetChordIntervalOptionalPairs[i].IsOptional) continue;

                RequiredTargetChordNoteLetters.Add(TargetChordNoteLetters[i]);
            }
        }

        private void CalculateValidTargetNotesOnEachString()
        {
            /*
                Given a list of target notes like [F, A, C] we find the location of each note on the fretboard.
                Then we filter out notes that do not meet constraints, such as not leading back to any StartChord 
                note via good voiceleading, being too low/high on the neck, etc. Ultimately we end up with 
                ValidTargetNotesOnEachString, which holds each target-note possibility for each string. Later we 
                loop over each string, finding each combination.

                Example (6-string guitar, standard tuning, major-second voiceleading max, target chord = F maj):

                Start-Chord    ValidTargetNotesOnEachString
                5              [1, 5]
                6              [6, 10]
                X              [10, 14]
                X              [15, 19]
                5              [3, 20, 24]
                X              [8]
            */

            ValidTargetNotesOnEachString = Config.StringedInstrument
                .CreateMapFromStringToNotesOnString(TargetChordNoteLetters)
                .Select(kvp =>
                {
                    List<StringedMusicalNote> notesOnString = kvp.Value.Where(NoteMeetsConstraints).ToList();

                    // Add a null to represent the possibility of a note not being played on that string.
                    notesOnString.Add(null);

                    return notesOnString;
                }).ToList();
        }

        private async Task CalculateWithTimeout()
        {
            await Task.Run(() =>
            {
                CalculatedFingerings = new List<Chord<StringedMusicalNote>>();
                CalculateRecursive(0, CalculatedFingerings, new List<StringedMusicalNote>(), new HashSet<int>(), new CancellationTokenSource(Config.CalculationTimeoutInMilliseconds).Token);
            });
        }

        private void CalculateRecursive(int currentStringIndex, IList<Chord<StringedMusicalNote>> allChords, IEnumerable<StringedMusicalNote> chordInProgress, HashSet<int> requiredNoteLettersObtained, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            /*
                For each string on the instrument, we have a list of available notes, including a null which 
                represents not playing anything on that string. 

                ValidTargetNotesOnEachString:

                [null, 1, 5]
                [null, 6, 10]
                [null, 10, 14]
                [null, 15, 19]
                [null, 3, 20, 24]
                [null, 8]
            
                Each of those notes (other than null) leads back to one or more StartChord notes with good 
                voiceleading. Take every combination of a note (or null) from string 1, a note (or null) from 
                string 2, etc. Along the way we can determine whether the chord we are building could ever 
                meet all constraints. If it cannot, we break out of that level of the loop, avoiding all 
                levels below and improving performance. 
            */

            // If there is no possibility of hitting all required notes given how many strings are left, 
            // get out so we can start the next iteration of the string above.
            if (!RemainingRequiredNotesCouldBeFound(currentStringIndex, requiredNoteLettersObtained.Count)) return;

            foreach (StringedMusicalNote note in ValidTargetNotesOnEachString[currentStringIndex])
            {
                // This hashset will hold the data passed down as well as data for the note
                // at this level. A copy must be made so we don't overwrite data
                // which needs to stay the same inside this loop.
                var requiredNoteLettersObtainedUpdated = new HashSet<int>(requiredNoteLettersObtained);
                var chordInProgressUpdated = new List<StringedMusicalNote>(chordInProgress);

                if (note != null)
                {
                    if (RequiredTargetChordNoteLetters.Contains(note.Letter))
                    {
                        requiredNoteLettersObtainedUpdated.Add((int)note.Letter);
                    }

                    chordInProgressUpdated.Add(note);
                }

                if (IsBottomString(currentStringIndex))
                {
                    if (ChordMeetsAllRequirements(chordInProgressUpdated, requiredNoteLettersObtainedUpdated))
                    {
                        allChords.Add(new Chord<StringedMusicalNote>(chordInProgressUpdated));
                    }
                }
                else
                {
                    CalculateRecursive(currentStringIndex + 1, allChords, chordInProgressUpdated, requiredNoteLettersObtainedUpdated, token);
                }
            }
        }

        private bool NoteMeetsConstraints(StringedMusicalNote targetNote)
        {
            if (targetNote.Fret > Config.MaxFret) return false;

            if (targetNote.Fret < Config.MinFret)
            {
                if (targetNote.Fret != 0 || (targetNote.Fret == 0 && Config.FilterOutOpenNotes)) return false;
            }

            return Config.StartChord.Notes.Any(startNote => NoteHasGoodVoiceleading(startNote, targetNote));
        }

        private bool NoteHasGoodVoiceleading(MusicalNote startNote, MusicalNote targetNote)
        {
            // If the highest note can travel it can go as high as possible.
            // But it cannot go lower than the 2nd highest note - the max
            // voiceleading jump.
            if (Config.HighestNoteCanTravel && startNote.Equals(HighestNoteInStartChord))
            {
                return targetNote.IntValue >= GetLowerLimitOfHighestNoteThatCanJump();
            }

            // If there is a highest required note letter, then the highest note
            // can only jump as high as possible if the end note is that required
            // note letter. Otherwise, it has to satisfy the basic criteria for
            // all other end notes.
            if (Config.HighestNote != null && startNote.Equals(HighestNoteInStartChord))
            {
                return targetNote.Letter == Config.HighestNote
                    ? targetNote.IntValue >= GetLowerLimitOfHighestNoteThatCanJump()
                    : SatisfiesBasicVoiceleadingJumpCriteria(startNote, targetNote);
            }

            if (Config.LowestNoteCanTravel && startNote.Equals(LowestNoteInStartChord))
            {
                return targetNote.IntValue <= GetUpperLimitOfLowestNoteThatCanJump();
            }

            return SatisfiesBasicVoiceleadingJumpCriteria(startNote, targetNote);
        }

        private int GetUpperLimitOfLowestNoteThatCanJump()
        {
            return SecondLowestNoteInStartChord == null
                ? Config.StringedInstrument.Tuning.Max(note => note.IntValue)
                : SecondLowestNoteInStartChord.IntValue + (int)Config.MaxVoiceleadingDistance;
        }

        private bool SatisfiesBasicVoiceleadingJumpCriteria(MusicalNote startNote, MusicalNote endNote)
        {
            return Math.Abs(endNote.IntValue - startNote.IntValue) <= (int)Config.MaxVoiceleadingDistance;
        }

        private int GetLowerLimitOfHighestNoteThatCanJump()
        {
            return SecondHighestNoteInStartChord == null
                ? Config.StringedInstrument.Tuning.Min(note => note.IntValue)
                : (SecondHighestNoteInStartChord.IntValue - (int)Config.MaxVoiceleadingDistance);
        }

        private bool ChordMeetsAllRequirements(List<StringedMusicalNote> chord, HashSet<int> requiredNoteLettersObtained)
        {
            if (!chord.Any()) return false;

            if (requiredNoteLettersObtained.Count != RequiredTargetChordNoteLetters.Count) return false;

            if (ChordIsTooWide(chord)) return false;

            // Each StartChord note must have a match within the voiceleading limit.
            if (GetStartNotesWithGoodVoiceleading(chord).Count != Config.StartChord.Notes.Count) return false;

            if (!HasRequiredLowAndHighNotes(chord)) return false;

            return true;
        }

        private bool IsBottomString(int currentStringIndex)
        {
            return currentStringIndex == ValidTargetNotesOnEachString.Count - 1;
        }

        private bool HasRequiredLowAndHighNotes(List<StringedMusicalNote> chord)
        {
            if (Config.HighestNote == null && Config.LowestNote == null)
            {
                return true;
            }

            IOrderedEnumerable<StringedMusicalNote> ordered = chord.OrderByDescending(chordNote => chordNote.IntValue);

            if (Config.HighestNote != null)
            {
                var highestNoteInChord = ordered.First();

                if (Config.HighestNote != highestNoteInChord.Letter)
                {
                    return false;
                }
            }

            if (Config.LowestNote != null)
            {
                var lowestNoteInChord = ordered.Last();

                if (Config.LowestNote != lowestNoteInChord.Letter)
                {
                    return false;
                }
            }

            return true;
        }

        private bool RemainingRequiredNotesCouldBeFound(int currentStringIndex, int numRequiredNoteLettersObtained)
        {
            var numStringsLeftIncludingThis = ValidTargetNotesOnEachString.Count - currentStringIndex;
            var numRequiredNotesLeft = RequiredTargetChordNoteLetters.Count - numRequiredNoteLettersObtained;

            return numStringsLeftIncludingThis >= numRequiredNotesLeft;
        }

        private void GroupFingeringsIntoVoicingSets()
        {
            var grouped = CalculatedFingerings
                .GroupBy(chord => chord.ToSortedPitchString())
                .ToDictionary(x => x.Key, x => x.ToList());

            // We cannot modify the dictionary in place if we interate over the entries or
            // keys, so we iterate over a copy of the keys instead.
            grouped.Keys.ToList().ForEach(key =>
            {
                var fingerings = grouped[key];

                grouped[key] = fingerings.OrderBy(x => GetLowestFret(x.Notes)).ToList();
            });

            VoicingSets = grouped
                .Select(group => new VoicingSet(group.Value, Config.StartChord))
                .ToList();
        }

        private List<MusicalNote> GetStartNotesWithGoodVoiceleading(IEnumerable<MusicalNote> targetChord)
        {
            return Config.StartChord.Notes
                .Where(startNote => targetChord.Any(targetNote => NoteHasGoodVoiceleading(startNote, targetNote)))
                .ToList();
        }

        private bool ChordIsTooWide(IEnumerable<StringedMusicalNote> chord)
        {
            var withoutOpens = FilterOpenNotes(chord);

            // If they were all open notes, this cannot be too wide
            if (!withoutOpens.Any()) return false;

            var max = withoutOpens.Max();
            var min = withoutOpens.Min();

            return (max - min) > Config.MaxFretsToStretch;
        }

        private IEnumerable<int> FilterOpenNotes(IEnumerable<StringedMusicalNote> chord)
        {
            return chord
                .Select(note => note.Fret)
                .Where(fret => fret != 0);
        }

        private int GetLowestFret(IEnumerable<StringedMusicalNote> chord)
        {
            var withoutOpens = FilterOpenNotes(chord);

            return withoutOpens.Any() ? withoutOpens.Min() : 0;
        }
    }
}