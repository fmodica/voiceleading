using MusicTheory;
using System;
using System.Linq;
using HelperExtensions;

namespace Voiceleading
{
    public static class ConfigValidator
    {
        public static void Validate(Config config)
        {
            config.ValidateIsNotNull(nameof(config));

            config.StartChord.ValidateIsNotNull(nameof(config.StartChord));

            config.StringedInstrument.ValidateIsNotNull(nameof(config.StringedInstrument));
            config.StringedInstrument.NumFrets.ValidateIsGreaterThan(0, nameof(config.StringedInstrument.NumFrets));
            config.StringedInstrument.Tuning.ValidateIsNotNullOrEmptyOrHasNullItem(nameof(config.StringedInstrument.Tuning));
            config.StringedInstrument.Tuning.ValidateDoesNotContainDuplicates(nameof(config.StringedInstrument.Tuning));

            config.TargetChordIntervalOptionalPairs.ValidateIsNotNullOrEmptyOrHasNullItem(nameof(config.TargetChordIntervalOptionalPairs));
            config.TargetChordIntervalOptionalPairs.Select(o => o.Interval).ValidateDoesNotContainDuplicates(nameof(config.TargetChordIntervalOptionalPairs));

            ((int)config.MaxVoiceleadingDistance).ValidateIsGreaterThan(0, nameof(config.MaxVoiceleadingDistance), true);
            ((int)config.MaxVoiceleadingDistance).ValidateIsLessThan((int)Interval.Third, nameof(config.MaxVoiceleadingDistance), true);

            config.MaxFretsToStretch.ValidateIsGreaterThan(0, nameof(config.MaxFretsToStretch), true);
            config.MaxFretsToStretch.ValidateIsLessThan(config.StringedInstrument.NumFrets, nameof(config.MaxFretsToStretch), true);
            
            config.MinFret.ValidateIsGreaterThan(0, nameof(config.MinFret), true);
            config.MinFret.ValidateIsLessThan(config.StringedInstrument.NumFrets, nameof(config.MinFret), true);

            config.MaxFret.ValidateIsGreaterThan(0, nameof(config.MaxFret), true);
            config.MaxFret.ValidateIsLessThan(config.StringedInstrument.NumFrets, nameof(config.MaxFret), true);

            config.CalculationTimeoutInMilliseconds.ValidateIsGreaterThan(0, nameof(config.CalculationTimeoutInMilliseconds));
        }
    }
}