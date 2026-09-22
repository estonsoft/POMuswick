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

        public void SaveChanges(IReadOnlyDictionary<string, string> changes)
        {
            var allowedKeys = new HashSet<string>(StringComparer.Ordinal)
            {
                nameof(AppSettings.UserName),
                nameof(AppSettings.BaseUrl),
                nameof(AppSettings.CustomerNo),
                nameof(AppSettings.IsLoggedIn),
                nameof(AppSettings.IsCredits),
                nameof(AppSettings.IsSalesUser),
                nameof(AppSettings.HoldForReview),
                nameof(AppSettings.ForceSubmit),
                nameof(AppSettings.QOHDisplay),
                nameof(AppSettings.BlockItemsNoQOH)
            };

            foreach (var change in changes)
            {
                if (allowedKeys.Contains(change.Key))
                {
                    SaveIfChanged(change.Key, change.Value);
                }
            }
        }

        private void SaveIfChanged(string key, string newValue)
        {
            if (_db.GetString(key) != newValue)
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
                BaseUrl =
                    _db.GetString(nameof(AppSettings.BaseUrl)) ?? "https://muswicksales.ddns.net",

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
        public void SaveChanges(IReadOnlyDictionary<string, string> changes);
        public AppSettings Load();
    }
}