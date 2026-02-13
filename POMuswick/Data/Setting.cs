using SQLite;

namespace POMuswick
{
    public class Setting
    {
        [PrimaryKey]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
