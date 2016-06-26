using System;
using System.Linq;

namespace MusicTheory.Voiceleading
{
    public static class SIVoiceleaderConfigValidator
    {
        public static void Validate(SIVoiceleaderConfig config)
        {
            if (config.StartingChordNotes == null || !config.StartingChordNotes.Any())
            {
                throw new ArgumentException(nameof(config.StartingChordNotes) + " is null or empty.");
            }

            if (config.EndChordRoot == null)
            {
                throw new ArgumentException(nameof(config.EndChordRoot) + " is null.");
            }

            if (config.StringedInstrument == null)
            {
                throw new ArgumentException(nameof(config.StringedInstrument) + " is null.");
            }

            if (config.StringedInstrument.NumFrets <= 0)
            {
                throw new ArgumentException(nameof(config.StringedInstrument.NumFrets) + " is less than or equal to zero.");
            }

            if (config.StringedInstrument.Tuning == null || !config.StringedInstrument.Tuning.Any())
            {
                throw new ArgumentException(nameof(config.StringedInstrument.Tuning) + " is numm or empty.");
            }

            if (config.StringedInstrument.Tuning.Distinct().Count() != config.StringedInstrument.Tuning.Count())
            {
                throw new ArgumentException(nameof(config.StringedInstrument.Tuning) + " contains duplicates.");
            }

            if (config.TargetChordIntervalOptionalPairs == null || !config.TargetChordIntervalOptionalPairs.Any())
            {
                throw new ArgumentException(nameof(config.TargetChordIntervalOptionalPairs) + " is null or empty.");
            }

            if (config.TargetChordIntervalOptionalPairs.Select(o => o.Interval).Distinct().Count() != config.TargetChordIntervalOptionalPairs.Count)
            {
                throw new ArgumentException(nameof(config.TargetChordIntervalOptionalPairs) + " contains duplicates.");
            }

            if (config.MaxFretsToStretch == null)
            {
                throw new ArgumentException(nameof(config.MaxFretsToStretch) + " is null.");
            }

            if (config.MaxFretsToStretch > config.StringedInstrument.NumFrets)
            {
                throw new ArgumentException(nameof(config.MaxFretsToStretch) + " is greater than " + nameof(config.StringedInstrument.NumFrets) + ".");
            }

            if (config.MaxVoiceleadingDistance == null)
            {
                throw new ArgumentException(nameof(config.MaxVoiceleadingDistance) + " is null.");
            }

            if (config.MaxVoiceleadingDistance < 0)
            {
                throw new ArgumentException(nameof(config.MaxVoiceleadingDistance) + " is less than zero.");
            }

            if (config.MaxVoiceleadingDistance > Interval.Third)
            {
                throw new ArgumentException(nameof(config.MaxVoiceleadingDistance) + " is greater than a major third.");
            }

            if (config.FretToStayAtOrBelow < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(config.FretToStayAtOrBelow) + " is less than zero.");
            }

            if (config.FretToStayAtOrAbove > config.StringedInstrument.NumFrets)
            {
                throw new ArgumentOutOfRangeException(nameof(config.FretToStayAtOrAbove) + " is greater than " + nameof(config.StringedInstrument.NumFrets) + ".");
            }
        }
    }
}