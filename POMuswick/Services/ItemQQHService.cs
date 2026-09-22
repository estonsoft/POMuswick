using POMuswick.Data;
using POMuswick.Models;
using POMuswick.Parsers;
using POMuswick.Repository;
namespace POMuswick.Services
{
    public class ItemQQHService : IItemQQHService
    {
        private readonly CommManager _comm;
        private readonly ItemQQHParser _parser;
        private readonly IItemQQHRepository _itemQQHRepository;
        private readonly ISettingService _settingService;
        public ItemQQHService(
            CommManager comm,
            ItemQQHParser parser,
            IItemQQHRepository itemQQHRepository,
            ISettingService settingService)
        {
            _comm = comm;
            _parser = parser;
            _itemQQHRepository = itemQQHRepository;
            _settingService = settingService;
        }

        public async Task<ItemQQHResult> FetchItemQQHAsync()
        {
            AppSettings appSettings = await _settingService.LoadSetting();
            var response = await _comm.GetItemQOH(appSettings.CustomerNo);
            ItemQQHResult result = await _parser.Parse(response);
            _itemQQHRepository.UpdateQQH(result.itemsQQH);
            return result;
        }

        public async Task<ItemQQHResult> FetchItemQQH2Async()
        {
            AppSettings appSettings = await _settingService.LoadSetting();
            var response = await _comm.GetItemQOH2(appSettings.UserName,appSettings.CustomerNo);
            ItemQQHResult result = await _parser.Parse(response);
            _itemQQHRepository.UpdateQQH(result.itemsQQH);
            return result;
        }

        public async Task Clear()
        {
            _itemQQHRepository.Clear();
        }
    }

    public interface IItemQQHService
    {
        public Task<ItemQQHResult> FetchItemQQHAsync();
        public Task<ItemQQHResult> FetchItemQQH2Async();
        public Task Clear();
    }
}