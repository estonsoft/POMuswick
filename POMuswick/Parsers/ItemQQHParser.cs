using System.Collections.Concurrent;
using POMuswick.Models;

namespace POMuswick.Parsers
{
    public class ItemQQHParser : BaseParser
    {
        public ItemQQHParser()
        {

        }
        public async Task<ItemQQHResult> Parse(string response)
        {
            Console.WriteLine("Get ItemQQHs returned");
            ConcurrentDictionary<int,int> keyValuePairs = new ConcurrentDictionary<int, int>();
            try
            {
                String sItems = response;
                String[] aItems = sItems.Split('~');
                
                if (aItems.Length > 1)
                {
                    Parallel.ForEach(aItems, s =>
                    {
                        String[] aItem = s.Split("|");
                        if (aItem.Count() < 2)
                        {
                            return;
                        }
                        int iItemNo = GetIntegerValue("Item Number", aItem[0], 0);
                        int iQOH = GetIntegerValue("QOH", aItem[1], 0);
                        keyValuePairs.TryAdd(iItemNo,iQOH);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get Item QOH Exception: " + ex.Message + ex.StackTrace);
            }
            ItemQQHResult result = new ItemQQHResult()
            {
                itemsQQH = keyValuePairs.ToDictionary()
            };
            return result;
        }
    }
}