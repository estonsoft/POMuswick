using POMuswick.Data;
using POMuswick.Models;
using POMuswick.Parsers;
using POMuswick.Repository;
namespace POMuswick.Services
{
    public class SettingService : ISettingService
    {
        private readonly CommManager _comm;
        private readonly SettingParser _parser;
        private readonly ISettingRepository _settingRepository;

        public SettingService(
            CommManager comm,
            SettingParser parser,
            ISettingRepository settingRepository)
        {
            _comm = comm;
            _parser = parser;
            _settingRepository = settingRepository;
        }

        public async Task<SettingResult> FetchSettingAsync()
        {
            var response = await _comm.GetSettings();
            SettingResult result = await _parser.Parse(response);
            _settingRepository.SaveChanges(new Dictionary<string, string>
            {
                [nameof(AppSettings.HoldForReview)] = result.appSettings.HoldForReview ? "1" : "0",
                [nameof(AppSettings.ForceSubmit)] = result.appSettings.ForceSubmit ? "1" : "0",
                [nameof(AppSettings.QOHDisplay)] = result.appSettings.QOHDisplay ?? "X",
                [nameof(AppSettings.BlockItemsNoQOH)] = result.appSettings.BlockItemsNoQOH ? "1" : "0"
            });
            return result;
        }

        public Task SaveChanges(IReadOnlyDictionary<string, string> changes)
        {
            _settingRepository.SaveChanges(changes);
            return Task.CompletedTask;
        }

        public async Task<AppSettings> LoadSetting()
        {
            return _settingRepository.Load();
        }

    }

    public interface ISettingService
    {
        public Task<SettingResult> FetchSettingAsync();
        public Task SaveChanges(IReadOnlyDictionary<string, string> changes);
        public Task<AppSettings> LoadSetting();
    }
}