using System.Collections.Concurrent;
using POMuswick.Models;

namespace POMuswick.Parsers
{
    public class SettingParser : BaseParser
    {
        public SettingParser()
        {

        }
        public async Task<SettingResult> Parse(string response)
        {
            Console.WriteLine("GetSettings returned");
            SettingResult result = new SettingResult();
            try
            {
                var settings = response.Split('|');
                AppSettings appSettings = new(){
                    HoldForReview = settings.ElementAtOrDefault(0) == "1",
                    ForceSubmit = settings.ElementAtOrDefault(1) == "1",
                    QOHDisplay = settings.ElementAtOrDefault(2) ?? "X",
                    BlockItemsNoQOH = settings.ElementAtOrDefault(3) == "1"
                };
                result.appSettings = appSettings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetSettings Exception: {ex}");
            }
            return result;
        }
    }
}