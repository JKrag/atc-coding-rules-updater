namespace Atc.CodingRules.Updater.CLI.Models;

public class OptionsFolderMappings
{
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "OK.")]
    public IList<string> Paths { get; set; } = new List<string>();

    public override string ToString()
        => $"{nameof(Paths)}.Count: {Paths.Count}";
}