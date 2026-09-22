using FluentFTP.Helpers;
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
            await SaveSetting(result.appSettings);
            return result;
        }

        public async Task SaveSetting(AppSettings appSettings)
        {
            _settingRepository.Save(appSettings);
        }

        public async Task<AppSettings> LoadSetting()
        {
            return _settingRepository.Load();
        }

    }

    public interface ISettingService
    {
        public Task<SettingResult> FetchSettingAsync();
        public Task SaveSetting(AppSettings appSettings);
        public Task<AppSettings> LoadSetting();
    }
}