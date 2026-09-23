using System.Collections.Concurrent;
using POMuswick.Models;

namespace POMuswick.Parsers
{
    public class ItemParser : BaseParser
    {
        public ItemParser()
        {

        }
        public async Task<ItemResult> Parse(string response)
        {
            Console.WriteLine(DateTime.Now.ToString() + " - Get Items returned");

            String sItems = response;
            String[] aItems = sItems.Split('~');
            var itemsToSave = new ConcurrentBag<Item>();
            var processedItemNos = new ConcurrentBag<int>();
            if (aItems.Length > 1)
            {
                Parallel.ForEach(aItems, s =>
                {
                    try
                    {
                        String[] aItem = s.Split("|");

                        if (aItem.Length < 20)
                        {
                            return;
                        }

                        Item item = new Item();
                        item.ItemNo = GetIntegerValue("Items", aItem[0], 0);
                        item.ItemNoDisplay = aItem[0];
                        item.Description = aItem[1].Trim();
                        item.ImageURL = Constants.ItemImageUrl + item.ItemNo.ToString() + ".jpg";
                        item.CategoryCode = aItem[2].Trim();
                        item.CategoryDesc = aItem[3].Trim();
                        item.SubcategoryCode = aItem[4].Trim();
                        item.SubcategoryDesc = aItem[5].Trim();
                        item.VendorCode = aItem[6].Trim();
                        item.VendorName = aItem[7].Trim();
                        item.UPC_1 = aItem[8].Trim();
                        item.ItemNoDisplayUPC = aItem[8].Trim();
                        item.UPC_2 = aItem[9].Trim();
                        item.UPC_3 = aItem[10].Trim();
                        item.UPC_4 = aItem[11].Trim();
                        item.RetailUOM = aItem[12].Trim();
                        item.RetailSize = aItem[13].Trim();
                        item.RetailPrice = GetDecimalValue("Retail Price", aItem[14].Trim(), 0);
                        item.RetailPriceDisplay = aItem[14].Trim();
                        item.UOM = aItem[15].Trim();
                        item.SizeUOM = item.UOM;
                        item.Size = GetIntegerValue("Size", aItem[16], 1);
                        item.SizeDisplay = aItem[16].Trim();
                        item.Form = aItem[17].Trim();
                        item.Price = GetIntegerValue("Price Value", aItem[18], 0);
                        item.PriceDisplay = $"${item.Price:0.00}";
                        item.Tax = GetDecimalValue("Tax", aItem[19].Trim(), 0);
                        item.TaxDisplay = $"${item.Tax:0.00}";
                        item.CategoryRank = GetIntegerValue("Category Rank", aItem[20].Trim(), 0);
                        item.SellUnitsInPurchaseUnit = GetIntegerValue("Sell Units in Purchase Unit", aItem[21].Trim(), 1);
                        item.Status = aItem[22];
                        item.QOH = GetIntegerValue("Item QOH", aItem[23].Trim(), 0);

                        try
                        {
                            item.NewItem = aItem[24].Trim();
                        }
                        catch (Exception e)
                        {
                            item.NewItem = "N";
                            Console.WriteLine("Get Items New Item exception" + e.Message + e.StackTrace);
                        }
                        try
                        {
                            int iAddedDate = GetIntegerValue("Added Date", aItem[25].Trim(), 0);
                            if (iAddedDate > 0)
                            {
                                int yy = 2000 + GetIntegerValue("Added Date Year", iAddedDate.ToString().Substring(1, 2), 0);
                                int mm = GetIntegerValue("Added Date Month", iAddedDate.ToString().Substring(3, 2), 0);
                                int dd = GetIntegerValue("Added Date Day", iAddedDate.ToString().Substring(5, 2), 0);
                                item.DateAdded = new DateTime(yy, mm, dd);
                                item.DateAddedDisplay = item.DateAdded.ToString("MM/dd/yy");
                            }
                            else
                            {
                                item.DateAdded = new DateTime(2001, 1, 1);
                                item.DateAddedDisplay = "";
                            }
                        }
                        catch (Exception e)
                        {
                            item.DateAdded = new DateTime(2001, 1, 1);
                            item.DateAddedDisplay = "";
                            Console.WriteLine("Get Items Date Added exception" + e.Message + e.StackTrace);
                        }
                        try
                        {
                            item.MaxOrderQty = GetIntegerValue("Max Order Qty", aItem[26], 0);
                            if ((item.MaxOrderQty == 0) || (item.MaxOrderQty >= 9999))
                            {
                                item.IsMaxOrderQtyVisible = false;
                            }
                            else
                            {
                                item.MaxOrderQtyDisplay = "Max " + aItem[26];
                                item.IsMaxOrderQtyVisible = true;
                            }
                        }
                        catch (Exception e)
                        {
                            item.MaxOrderQty = 0;
                            item.IsMaxOrderQtyVisible = false;
                            Console.WriteLine("Get Items Max Order Qty exception" + e.Message + e.StackTrace);
                        }
                        try
                        {
                            item.Keyword1 = aItem[28];
                            item.Keyword2 = aItem[29];
                            item.Keyword3 = aItem[30];
                        }
                        catch (Exception e)
                        {
                            item.Keyword1 = "";
                            item.Keyword2 = "";
                            item.Keyword3 = "";
                            Console.WriteLine("Get Items Keywords exception" + e.Message + e.StackTrace);
                        }

                        try
                        {
                            item.LastPurchDateDisplay = aItem[31];
                        }
                        catch (Exception e)
                        {
                            item.LastPurchDateDisplay = "";
                            Console.WriteLine("Get Items Last Purch Date exception" + e.Message + e.StackTrace);
                        }
                        item.LastPurchDate = GetDateTime("Last Purchase Date", item.LastPurchDateDisplay);
                        item.QtyLastOrder = GetIntegerValue("last order", aItem[32], 0);
                        item.QtyLastOrderDisplay = item.QtyLastOrder.ToString();
                        try
                        {
                            item.LongDescription = aItem[36];
                            try
                            {
                                if (item.LongDescription != "")
                                {
                                    item.SearchDescription = item.LongDescription;
                                }
                                else
                                {
                                    item.SearchDescription = item.Description;
                                }
                            }
                            catch (Exception e)
                            {
                                item.SearchDescription = item.Description;
                                Console.WriteLine("Get Items Search Description exception" + e.Message + e.StackTrace);
                            }
                        }
                        catch (Exception e)
                        {
                            item.LongDescription = "";
                            item.SearchDescription = item.Description;
                            Console.WriteLine("Get Items Long Description exception" + e.Message + e.StackTrace);
                        }

                        item.QtyOrder = 0;

                        itemsToSave.Add(item);
                        processedItemNos.Add(item.ItemNo);
                    }
                    catch (Exception pe)
                    {
                        Console.WriteLine("Parse Items exception" + pe.Message + pe.StackTrace);
                    }
                });
            }
            Console.WriteLine("Get Items Completed");
            ItemResult result = new ItemResult()
            {
                items = itemsToSave.ToList(),
                itemNumbers = processedItemNos.ToList()
            };
            return result;
        }
    }
}