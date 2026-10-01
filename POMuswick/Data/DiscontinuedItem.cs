using Realms;

namespace POMuswick
{
    class DiscontinuedItem : Realms.RealmObject
    {
        [PrimaryKey]
        public int ItemNo { get; set; }
    }
}
