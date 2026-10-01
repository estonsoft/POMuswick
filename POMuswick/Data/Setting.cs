using Realms;

namespace POMuswick
{
    public class Setting : Realms.RealmObject
    {
        [PrimaryKey]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
