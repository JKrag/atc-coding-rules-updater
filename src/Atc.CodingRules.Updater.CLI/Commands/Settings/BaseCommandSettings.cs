using System.ComponentModel;
using Spectre.Console;

namespace Atc.CodingRules.Updater.CLI.Commands.Settings
{
    public class BaseCommandSettings : CommandSettings
    {
        [CommandOption($"{CommandConstants.ArgumentShortProjectPath}|{CommandConstants.ArgumentLongProjectPath} <PROJECTPATH>")]
        [Description("Path to the project directory (default current diectory)")]
        public string ProjectPath { get; init; } = string.Empty;

        [CommandOption($"{CommandConstants.ArgumentShortVerbose}|{CommandConstants.ArgumentLongVerbose} ")]
        [Description("Use verbose for more debug/trace information")]
        public bool Verbose { get; init; }

        [CommandOption($"{CommandConstants.ArgumentShortOptionsPath}|{CommandConstants.ArgumentLongOptionsTarget} [OPTIONSPATH]")]
        [Description("Path to an optional options json-file")]
        public FlagValue<string>? OptionsPath { get; init; }

        public override ValidationResult Validate()
        {
            var validationResult = base.Validate();
            if (!validationResult.Successful)
            {
                return validationResult;
            }

            return string.IsNullOrEmpty(ProjectPath)
                ? ValidationResult.Error("ProjectPath is missing.")
                : ValidationResult.Success();
        }

        internal string GetOptionsPath()
        {
            var optionsPath = string.Empty;
            if (this.OptionsPath is not null && this.OptionsPath.IsSet)
            {
                optionsPath = this.OptionsPath.Value;
            }

            return optionsPath;
        }
    }
}