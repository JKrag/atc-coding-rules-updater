using Atc.CodingRules.AnalyzerProviders.Models;

namespace Atc.CodingRules.AnalyzerProviders.Providers
{
    public class AsyncifyProvider : AnalyzerProviderBase
    {
        public override AnalyzerProviderBaseRuleData CollectBaseRules()
        {
            var data = new AnalyzerProviderBaseRuleData("Asyncify");

            // TODO: Fix this..
            return data;
        }
    }
}