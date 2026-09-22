using POMuswick.Models;

namespace POMuswick.Repository
{

    public class SettingRepository : ISettingRepository
    {
        private readonly Database _db;

        public SettingRepository(Database db)
        {
            _db = db;
        }

        public void Save(AppSettings setting)
        {
            if (!string.IsNullOrWhiteSpace(setting.UserName))
                _db.SaveString(nameof(setting.UserName), setting.UserName);

            if (!string.IsNullOrWhiteSpace(setting.LastUserName))
                _db.SaveString(nameof(setting.LastUserName), setting.LastUserName);

            if (!string.IsNullOrWhiteSpace(setting.CustomerNo))
                _db.SaveString(nameof(setting.CustomerNo), setting.CustomerNo);

            if (!string.IsNullOrWhiteSpace(setting.QOHDisplay))
                _db.SaveString(nameof(setting.QOHDisplay), setting.QOHDisplay);

            if (!string.IsNullOrWhiteSpace(setting.LastCategoryUpdate))
                _db.SaveString(nameof(setting.LastCategoryUpdate), setting.LastCategoryUpdate);

            if (!string.IsNullOrWhiteSpace(setting.LastItemUpdate))
                _db.SaveString(nameof(setting.LastItemUpdate), setting.LastItemUpdate);

            // Booleans are always present, so they will always be saved
            _db.SaveString(nameof(setting.IsLoggedIn), setting.IsLoggedIn ? "1" : "0");
            _db.SaveString(nameof(setting.IsCredits), setting.IsCredits ? "1" : "0");
            _db.SaveString(nameof(setting.IsSalesUser), setting.IsSalesUser ? "1" : "0");
            _db.SaveString(nameof(setting.HoldForReview), setting.HoldForReview ? "1" : "0");
            _db.SaveString(nameof(setting.ForceSubmit), setting.ForceSubmit ? "1" : "0");
            _db.SaveString(nameof(setting.BlockItemsNoQOH), setting.BlockItemsNoQOH ? "1" : "0");
        }
        private void SaveIfChanged(string key, string newValue)
        {
            var currentValue = _db.GetString(key);

            if (currentValue != newValue)
            {
                _db.SaveString(key, newValue);
            }
        }

        public AppSettings Load()
        {
            return new AppSettings
            {
                IsLoggedIn =
                    _db.GetString(nameof(AppSettings.IsLoggedIn)) == "1",
                IsCredits =
                    _db.GetString(nameof(AppSettings.IsCredits)) == "1",
                IsSalesUser =
                    _db.GetString(nameof(AppSettings.IsSalesUser)) == "1",
                UserName =
                    _db.GetString(nameof(AppSettings.UserName)) ?? "",
                LastUserName =
                    _db.GetString(nameof(AppSettings.LastUserName)) ?? "",
                CustomerNo =
                    _db.GetString(nameof(AppSettings.CustomerNo)) ?? "",
                LastCategoryUpdate =
                    _db.GetString(nameof(AppSettings.LastCategoryUpdate)) ?? "",
                LastItemUpdate =
                    _db.GetString(nameof(AppSettings.LastItemUpdate)) ?? "",

                ForceSubmit =
                    _db.GetString(nameof(AppSettings.ForceSubmit)) == "1",

                QOHDisplay =
                    _db.GetString(nameof(AppSettings.QOHDisplay)) ?? "X",

                BlockItemsNoQOH =
                    _db.GetString(nameof(AppSettings.BlockItemsNoQOH)) == "1"
            };
        }
    }

    public interface ISettingRepository
    {
        public void Save(AppSettings Setting);
        public AppSettings Load();
    }
}