using System;
using System.Linq;

namespace MusicTheory.Voiceleading
{
    public static class ConfigValidator
    {
        const string CANNOT_BE_EMPTY = "The collection cannot be empty.";
        const string CANNOT_CONTAIN_DUPLICATES = "The collection cannot contain duplicates.";
        const string MUST_BE_GREATER_THAN_ZERO = "The value must be greater than zero.";
        const string MUST_BE_GREATER_THAN_OR_EQUAL_TO_ZERO = "The value must be greater than or equal to zero.";
        const string MUST_BE_LESS_THAN_OR_EQUAL_TO_MAJOR_THIRD = "The value must be less than or equal to " + nameof(Interval.Third) + ".";

        public static void Validate(Config config)
        {
            if (config.StartChord == null)
            {
                throw new ArgumentNullException(nameof(config.StartChord));
            }

            if (config.TargetChordRoot == null)
            {
                throw new ArgumentNullException(nameof(config.TargetChordRoot));
            }

            if (config.StringedInstrument == null)
            {
                throw new ArgumentNullException(nameof(config.StringedInstrument));
            }

            if (config.StringedInstrument.NumFrets <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(config.StringedInstrument.NumFrets), MUST_BE_GREATER_THAN_ZERO);
            }

            if (config.StringedInstrument.Tuning == null)
            {
                throw new ArgumentNullException(nameof(config.StringedInstrument.Tuning));
            }

            if (!config.StringedInstrument.Tuning.Any())
            {
                throw new ArgumentException(CANNOT_BE_EMPTY, nameof(config.StringedInstrument.Tuning));
            }

            if (config.StringedInstrument.Tuning.Distinct().Count() != config.StringedInstrument.Tuning.Count())
            {
                throw new ArgumentException(CANNOT_CONTAIN_DUPLICATES, nameof(config.StringedInstrument.Tuning));
            }

            if (config.TargetChordIntervalOptionalPairs == null)
            {
                throw new ArgumentNullException(nameof(config.TargetChordIntervalOptionalPairs));
            }

            if (!config.TargetChordIntervalOptionalPairs.Any())
            {
                throw new ArgumentException(CANNOT_BE_EMPTY, nameof(config.TargetChordIntervalOptionalPairs));
            }

            if (config.TargetChordIntervalOptionalPairs.Select(o => o.Interval).Distinct().Count() != config.TargetChordIntervalOptionalPairs.Count)
            {
                throw new ArgumentException(CANNOT_CONTAIN_DUPLICATES, nameof(config.TargetChordIntervalOptionalPairs));
            }

            if (config.MaxFretsToStretch == null)
            {
                throw new ArgumentNullException(nameof(config.MaxFretsToStretch));
            }

            if (config.MaxFretsToStretch < 0 || config.MaxFretsToStretch > config.StringedInstrument.NumFrets)
            {
                throw new ArgumentOutOfRangeException(nameof(config.MaxFretsToStretch), GetBetweenZeroAndMaxMessage(nameof(config.StringedInstrument.NumFrets)));
            }

            if (config.MaxVoiceleadingDistance == null)
            {
                throw new ArgumentNullException(nameof(config.MaxVoiceleadingDistance));
            }

            if (config.MaxVoiceleadingDistance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(config.MaxVoiceleadingDistance), MUST_BE_GREATER_THAN_OR_EQUAL_TO_ZERO);
            }

            if (config.MaxVoiceleadingDistance > Interval.Third)
            {
                throw new ArgumentOutOfRangeException(nameof(config.MaxVoiceleadingDistance), MUST_BE_LESS_THAN_OR_EQUAL_TO_MAJOR_THIRD);
            }

            if (config.FretToStayAtOrBelow == null)
            {
                throw new ArgumentNullException(nameof(config.FretToStayAtOrBelow));
            }

            if (config.FretToStayAtOrAbove == null)
            {
                throw new ArgumentNullException(nameof(config.FretToStayAtOrAbove));
            }

            if (config.FretToStayAtOrBelow < 0 || config.FretToStayAtOrAbove > config.StringedInstrument.NumFrets)
            {
                throw new ArgumentOutOfRangeException(nameof(config.FretToStayAtOrBelow), GetBetweenZeroAndMaxMessage(nameof(config.StringedInstrument.NumFrets)));
            }

            if (config.CalculationTimeoutInMilliseconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(config.CalculationTimeoutInMilliseconds), MUST_BE_GREATER_THAN_ZERO);
            }
        }

        private static string GetBetweenZeroAndMaxMessage(string name)
        {
            return "The value must be between 0 and " + name + ".";
        }
    }
}