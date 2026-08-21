using System.Collections.Concurrent;
using System.Globalization;
using POMuswick.Data;

namespace POMuswick
{
    internal class XMLResponseParser
    {
        public static async Task commService_GetBannersCompleted(String response)
        {
            try
            {
                Console.WriteLine("Get Banners returned");
                String sBanners = response;
                String[] aBanners = sBanners.Split('|');
                ConcurrentBag<Banner> lstBanners = new ConcurrentBag<Banner>();
                if (aBanners.Length >= 1)
                {

                    Parallel.ForEach(aBanners, s =>
                    {
                        Banner banner = new Banner();
                        banner.BannerName = s;
                        banner.BannerURL = Constants.BannerUrl + banner.BannerName;
                        lstBanners.Add(banner);
                    });
                    try
                    {

                        await App.g_db.DeleteBannersAsync();
                        await App.g_db.SaveBannerAsync(lstBanners.ToList());
                        Console.WriteLine("Get Banners returned Completed");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occurred while saving banners: " + ex.Message);
                    }
                    await App.CommManager.GetCategoriesAndSubcategoriesCust(App.g_Customer.CustNo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get Banners Error");
                Console.WriteLine(ex.Message);
            }
        }


        public static async Task commService_GetCategoriesAndSubcategoriesCompleted(String response)
        {
            Console.WriteLine("Get Categories and Subcategories returned");

            try
            {
                String sCategories = response;
                String[] aCategories = sCategories.Split('~');
                ConcurrentBag<Category> lstCategories = new ConcurrentBag<Category>();
                ConcurrentBag<Subcategory> lstSubcategories = new ConcurrentBag<Subcategory>();

                if (aCategories.Length > 1)
                {
                    Parallel.ForEach(aCategories, s =>
                    {
                        String[] aCategory = s.Split("|");

                        if (aCategory.Count() < 4)
                        {
                            return;
                        }

                        if (aCategory[1].Length == 0)
                        {
                            Category cat = new Category();
                            cat.Code = aCategory[0];
                            cat.Description = aCategory[2].Trim();
                            cat.ImageURL = Constants.CategoryImageUrl + cat.Code + ".png";
                            cat.Rank = GetIntegerValue("Category Rank", aCategory[3].Trim(), 0);
                            cat.HomePage = GetIntegerValue("Category Home Page", aCategory[4].Trim(), 0);
                            lstCategories.Add(cat);
                        }
                        else
                        {
                            Subcategory subcat = new Subcategory();
                            subcat.Category = aCategory[0];
                            subcat.Code = aCategory[1];
                            subcat.Description = aCategory[2].Trim();
                            subcat.Rank = GetIntegerValue("Subcategory Rank", aCategory[3].Trim(), 0);
                            lstSubcategories.Add(subcat);
                        }
                    });

                    try
                    {

                        await App.g_db.DeleteAllCategory();
                        await App.g_db.DeleteAllSubcategory();
                        await App.g_db.SaveCategory(lstCategories.ToList());
                        await App.g_db.SaveSubcategory(lstSubcategories.ToList());

                        Console.WriteLine("Get Categories and Subcategories returned Completed");
                        App.g_HomePageCategoryList = await App.g_db.GetHomePageCategories();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occurred while parsing categories and subcategories: " + ex.Message);
                    }
                }

                try
                {
                    String CustNo = "0";
                    try
                    {
                        CustNo = App.g_Customer.CustNo;
                    }
                    catch
                    {
                        CustNo = "0";
                    }

                    //Database db = new Database();
                    string sDate = await App.g_db.GetSetting("LastUpdateItems");

                    // if (sDate == "")
                    // {
                    //     sDate = "0";
                    // }

                    // for now always refresh all items
                    sDate = "0";
                    if (App.g_Customer.CustNo == "0")
                    {
                        await App.CommManager.GetItems("0", sDate);
                    }
                    else
                    {
                        await App.CommManager.GetItems(App.g_Customer.CustNo, sDate);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get Categories and Subcategories exception" + ex.Message + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get Categories and Subcategories exception" + ex.Message + ex.StackTrace);
            }
        }

        public static async Task commService_GetCategoriesAndSubcategoriesCustCompleted(String response)
        {
            Console.WriteLine("Get Categories and Subcategories Cust returned");

            try
            {
                String sCategories = response;
                String[] aCategories = sCategories.Split('~');
                ConcurrentBag<Category> lstCategories = new ConcurrentBag<Category>();
                ConcurrentBag<Subcategory> lstSubcategories = new ConcurrentBag<Subcategory>();

                if (aCategories.Length > 1)
                {
                    Parallel.ForEach(aCategories, s =>
                    {
                        String[] aCategory = s.Split("|");

                        if (aCategory.Count() < 4)
                        {
                            return;
                        }

                        if (aCategory[1].Length == 0)
                        {
                            Category cat = new Category();
                            cat.Code = aCategory[0];
                            cat.Description = aCategory[2].Trim();
                            cat.ImageURL = Constants.CategoryImageUrl + cat.Code + ".png";
                            cat.Rank = GetIntegerValue("Category Rank", aCategory[3].Trim(), 0);
                            cat.HomePage = GetIntegerValue("Category Home Page", aCategory[4].Trim(), 0);
                            lstCategories.Add(cat);
                        }
                        else
                        {
                            Subcategory subcat = new Subcategory();
                            subcat.Category = aCategory[0];
                            subcat.Code = aCategory[1];
                            subcat.Description = aCategory[2].Trim();
                            subcat.Rank = GetIntegerValue("Subcategory Rank", aCategory[3].Trim(), 0);
                            lstSubcategories.Add(subcat);
                        }
                    });

                    try
                    {

                        await App.g_db.DeleteAllCategory();
                        await App.g_db.DeleteAllSubcategory();
                        await App.g_db.SaveCategory(lstCategories.ToList());
                        await App.g_db.SaveSubcategory(lstSubcategories.ToList());

                        Console.WriteLine("Get Categories Subcategories and Subsubcategories returned Completed");
                        // App.g_HomePageCategoryList = await App.g_db.GetHomePageCategories();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occurred while parsing categories subcategories and subsubcategories: " + ex.Message);
                    }
                }

                try
                {
                    String CustNo = "0";
                    try
                    {
                        CustNo = App.g_Customer.CustNo;
                    }
                    catch
                    {
                        CustNo = "0";
                    }

                    //Database db = new Database();
                    string sDate = await App.g_db.GetSetting("LastUpdateItems");

                    if (sDate == "")
                    {
                        sDate = "0";
                    }

                    // for now always refresh all items
                    sDate = "0";
                    if (App.g_Customer.CustNo == "0")
                    {
                        await App.CommManager.GetItems("0", sDate);
                    }
                    else
                    {
                        await App.CommManager.GetItems(App.g_Customer.CustNo, sDate);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get Categories and Subcategories Cust exception" + ex.Message + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get Categories and Subcategories Cust exception" + ex.Message + ex.StackTrace);
            }
        }

        public static async Task commService_GetItemsCompletedAsync(String response)
        {
            try
            {
                Console.WriteLine(DateTime.Now.ToString() + " - Get Items returned");

                String sItems = response;
                String[] aItems = sItems.Split('~');

                if (aItems.Length > 1)
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();

                    List<Item> lstCartItems = await App.g_db.GetCartItems();
                    var cartDict = lstCartItems.ToDictionary(c => c.ItemNo); // O(1) lookup instead of nested loop

                    var itemsToSave = new ConcurrentBag<Item>();
                    var processedItemNos = new ConcurrentBag<int>();

                    Parallel.ForEach(aItems, s =>
                    {
                        try
                        {
                            String[] aItem = s.Split("|");

                            if (aItem.Count() < 20)
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
                            if (item.UPC_1.Length > 0)
                            {
                                item.ItemNoDisplayUPC = "(" + item.UPC_1 + ")";
                            }
                            else
                            {
                                item.ItemNoDisplayUPC = "";
                            }
                            item.UPC_2 = aItem[9].Trim();
                            item.UPC_3 = aItem[10].Trim();
                            item.UPC_4 = aItem[11].Trim();
                            item.RetailUOM = aItem[12].Trim();
                            item.RetailSize = aItem[13].Trim();
                            item.RetailPrice = GetDecimalValue("Retail Price", aItem[14].Trim(), 0);
                            item.RetailPriceDisplay = string.Format("{0:C}", aItem[14]);
                            item.UOM = aItem[15].Trim();
                            item.SizeUOM = "/" + item.UOM;
                            item.Size = GetIntegerValue("Size", aItem[16], 1);
                            item.SizeDisplay = aItem[16].Trim();
                            item.Form = aItem[17].Trim();
                            item.Price = GetIntegerValue("Price Value", aItem[18], 0);
                            item.PriceDisplay = string.Format("{0:C}", aItem[18]);
                            item.Tax = GetDecimalValue("Tax", aItem[19].Trim(), 0);
                            item.TaxDisplay = string.Format("{0:C}", item.Tax);
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

                            if (cartDict.TryGetValue(item.ItemNo, out var ci))
                            {
                                item.QtyOrder = ci.QtyOrder;
                            }
                            itemsToSave.Add(item);
                            processedItemNos.Add(item.ItemNo);
                        }
                        catch (Exception pe)
                        {
                            Console.WriteLine("Parse Items exception" + pe.Message + pe.StackTrace);
                        }
                    });
                    Console.WriteLine($"Parse loop: {sw.ElapsedMilliseconds}ms"); sw.Restart();

                    try
                    {

                        await App.g_db.InsertDiscontinuedItems();
                        await App.g_db.DeleteItems();
                        await App.g_db.SaveItems(itemsToSave.ToList());
                        Console.WriteLine("Items saves Successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occurred while bulk-saving items: " + ex.Message);
                    }
                    try
                    {
                        await App.g_db.DeleteDiscontinuedItems(processedItemNos.ToList());

                        Console.WriteLine($"Delete discontinued: {sw.ElapsedMilliseconds}ms"); sw.Restart();

                        await App.g_db.UpdateDiscontinuedItems();
                        Console.WriteLine("Update Discontinued Items completed");

                        await App.g_db.SaveSetting("LastUpdateItems", DateTime.Now.ToString("1yyMMdd"));

                        App.g_ItemList = await App.g_db.GetItems();


                        Console.WriteLine($"Finalize + commit: {sw.ElapsedMilliseconds}ms");

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occurred while removing discontinued items: " + ex.Message);
                    }

                    await App.CommManager.GetItemQOH(App.g_Customer.CustNo);
                }
            }
            catch (Exception ex)
            {
                String sMsg = ex.Message + ex.StackTrace;
                Console.WriteLine(DateTime.Now.ToString() + " - Get Items exception: " + sMsg);
            }
            Console.WriteLine("Get Items Completed");
        }


        public static async Task commService_GetItemQOHCompletedAsync(string response)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(response))
                    return;

                if (response == "X")
                {
                    App.g_Shell.Logout();
                    return;
                }

                string[] aItems = response.Split(
                    '~',
                    StringSplitOptions.RemoveEmptyEntries);

                if (aItems.Length == 0)
                    return;

                var qohUpdates = new List<(int ItemNo, int QOH)>(aItems.Length);

                foreach (string item in aItems)
                {
                    if (string.IsNullOrWhiteSpace(item))
                        continue;

                    string[] aItem = item.Split('|');

                    if (aItem.Length < 2)
                        continue;

                    int itemNo = GetIntegerValue(
                        "Item Number",
                        aItem[0],
                        0);

                    int qoh = GetIntegerValue(
                        "QOH",
                        aItem[1],
                        0);

                    if (itemNo <= 0)
                        continue;

                    qohUpdates.Add((itemNo, qoh));
                }

                if (qohUpdates.Count == 0)
                    return;

                // Bulk update
                await App.g_db.UpdateAllItemQOH(qohUpdates);

                Console.WriteLine(
                    $"Item QOH updated successfully: {qohUpdates.Count} items");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Get Item QOH Exception: {ex.Message}");

                Console.WriteLine(ex.StackTrace);
            }
        }

        public static async Task commService_GetItemQOH2CompletedAsync(string response)
        {
            Console.WriteLine("Get Item QOH 2 returned");

            try
            {
                if (string.IsNullOrWhiteSpace(response))
                    return;

                if (response == "X")
                {
                    App.g_Shell.Logout();
                    return;
                }

                var updates = new List<(int ItemNo, int QOH)>();

                foreach (string item in response.Split(
                    '~',
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] values = item.Split('|');

                    if (values.Length < 2)
                        continue;

                    int itemNo = GetIntegerValue(
                        "Item Number",
                        values[0],
                        0);

                    int qoh = GetIntegerValue(
                        "QOH",
                        values[1],
                        0);

                    if (itemNo <= 0)
                        continue;

                    updates.Add((itemNo, qoh));
                }

                if (updates.Count > 0)
                {
                    // ONE DB call + ONE transaction
                    await App.g_db.UpdateAllItemQOH(updates);

                    Console.WriteLine(
                        $"QOH 2 updated: {updates.Count} items");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Get Item QOH 2 Exception: {ex.Message}");

                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("Get Item QOH 2 Completed");
        }

        public static async Task commService_ValidateLoginCompletedAsync(String response)
        {
            Console.WriteLine("ValidateLogin Complete");
            try
            {
                String sUser = response;

                String[] aInfo = sUser.Split("~");

                String[] aUser = aInfo[0].Split("|");
                String[] aCust = aInfo[1].Split("|");
                String OldCustNo = "0";

                try
                {
                    if (aUser[0] == "V")
                    {
                        try
                        {
                            if (aUser[2] == "1")
                            {
                                App.g_IsCredits = "yes";
                            }
                            else
                            {
                                App.g_IsCredits = "no";
                            }
                            await App.g_db.SaveSetting("Credits", App.g_IsCredits);

                            if (aUser[3] == "1")
                            {
                                App.g_HoldForReview = true;
                            }
                            else
                            {
                                App.g_HoldForReview = false;
                            }
                            await App.g_db.SaveSetting("HoldForReview", aUser[3]);

                            try
                            {
                                if (aUser[4] == "1")
                                {
                                    App.g_ForceSubmit = true;
                                }
                                else
                                {
                                    App.g_ForceSubmit = false;
                                }
                                await App.g_db.SaveSetting("ForceSubmit", aUser[4]);
                            }
                            catch (Exception ex)
                            {
                                App.g_ForceSubmit = false;
                                await App.g_db.SaveSetting("ForceSubmit", "0");
                                Console.WriteLine("Get Validate Login Force Submit exception: " + ex.Message + ex.StackTrace);
                            }

                            try
                            {
                                App.g_QOHDisplay = aUser[5];
                            }
                            catch (Exception ex)
                            {
                                App.g_QOHDisplay = "X";
                                Console.WriteLine("Get Validate Login QOH Display exception: " + ex.Message + ex.StackTrace);
                            }
                            await App.g_db.SaveSetting("QOHDisplay", App.g_QOHDisplay);

                            try
                            {
                                if (aUser[6] == "1")
                                {
                                    App.g_BlockItemsNoQOH = true;
                                }
                                else
                                {
                                    App.g_BlockItemsNoQOH = false;
                                }
                                await App.g_db.SaveSetting("BlockItemsNoQOH", aUser[6]);
                            }
                            catch (Exception ex)
                            {
                                App.g_BlockItemsNoQOH = false;
                                await App.g_db.SaveSetting("BlockItemsNoQOH", "0");
                                Console.WriteLine("Get Validate Login Block Items No QOH exception: " + ex.Message + ex.StackTrace);
                            }
                            try
                            {
                                if (aUser[8] == "1")
                                {
                                    App.g_IsSalesUser = true;
                                }
                                else
                                {
                                    App.g_IsSalesUser = false;
                                }
                                await App.g_db.SaveSetting("IsSalesUser", aUser[8]);
                            }
                            catch (Exception ex)
                            {
                                App.g_IsSalesUser = false;
                                await App.g_db.SaveSetting("IsSalesUser", "0");
                                Console.WriteLine("Get Validate Login Is Sales User exception: " + ex.Message + ex.StackTrace);
                            }

                            if (!App.g_IsSalesUser)
                            {
                                App.g_Customer.Status = "9";
                                App.g_Customer.CompanyName = aCust[1];
                                App.g_Customer.Warehouse = GetIntegerValue("Warehouse", aCust[3], 0);
                                App.g_Customer.Address1 = aCust[4];
                                App.g_Customer.City = aCust[5];
                                App.g_Customer.State = aCust[6];
                                App.g_Customer.Zip = aCust[7];
                                App.g_Customer.CityStateZip = aCust[5] + ", " + aCust[6] + "  " + aCust[7];
                                App.g_Customer.Phone = aCust[8];
                                App.g_Customer.Contact = aCust[9];
                                App.g_Customer.Delivery = GetIntegerValue("Delivery", aCust[10], 0);
                                App.g_Customer.Pickup = GetIntegerValue("Pickup", aCust[11], 0);
                                App.g_Customer.CreditLimit = GetDecimalValue("Credit Limit", aCust[12], 0);
                                App.g_Customer.ARBalance = GetDecimalValue("AR Balance", aCust[13], 0);

                                App.g_Customer.MinOrderAmount = GetDecimalValue("Min Order Amount", aCust[20], 0);
                                App.g_Customer.ShippingFee = GetDecimalValue("Shipping Fee", aCust[21], 0);

                                Location loc = new Location();
                                loc.LocationId = 1;
                                loc.Name = aCust[14];
                                loc.Address = aCust[15];
                                loc.City = aCust[16];
                                loc.State = aCust[17];
                                loc.Zip = aCust[18];
                                loc.CityStateZip = loc.City + ", " + loc.State + " " + loc.Zip;
                                loc.Phone = aCust[19];

                                OldCustNo = App.g_Customer.CustNo;
                                App.g_Customer.CustNo = aUser[1];
                                //Database db = new Database();
                                await App.g_db.SaveCustomer(App.g_Customer);
                                await App.g_db.SaveLocation(loc);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }


                        if (App.g_Customer.CustNo != OldCustNo)
                        {
                            if (App.g_UserName.ToLower() == "app_test")
                            {
                                await App.g_db.DeleteCategories();
                                await App.g_db.DeleteItems();
                            }
                        }
                        await App.RefreshAll();
                        await App.CommManager.GetOrderHistory(App.g_Customer.CustNo);
                        if (App.g_IsSalesUser)
                        {
                            await App.CommManager.GetSalespersonCustomers(App.g_UserName);
                        }
                        await App.g_db.SaveSetting("LoggedIn", "1");
                        await App.g_db.SaveSetting("UserName", App.g_UserName);
                        App.g_IsLoggedIn = true;
                        try
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                _ = await App.g_Shell.GoToHome();
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }
                    }
                    else if (aUser[0] == "P")
                    {
                        try
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Invalid password.  Please try again.", "Ok");
                                App.g_LoginPage.HideAnimation();
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }
                    }
                    else if (aUser[0] == "I")
                    {
                        try
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Inactive account.  Please contact Customer Service.", "Ok");
                                App.g_LoginPage.HideAnimation();
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }
                    }
                    else if (aUser[0] == "U")
                    {
                        try
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Account does not exist.", "Ok");
                                App.g_LoginPage.HideAnimation();
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }
                    }
                    else if (aUser[0] == "X")
                    {
                        try
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Error attempting to login.", "Ok");
                                App.g_LoginPage.HideAnimation();
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }
                    }
                    else
                    {
                        try
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Error attempting to login.", "Ok");
                                App.g_LoginPage.HideAnimation();
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Error attempting to login.", "Ok");
                            App.g_LoginPage.HideAnimation();
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Get Validate Login exception: " + e.Message + e.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Error attempting to login.", "Ok");
                        App.g_LoginPage.HideAnimation();
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Get Validate Login exception: " + e.Message + e.StackTrace);
                }
            }
        }

        public static async Task commService_SubmitOrderCompletedAsync(String response)
        {
            try
            {
                if (response == "S")
                {
                    await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Thank you! Your order has been placed.", "OK");

                    //Database db = new Database();
                    await App.g_db.ClearCartItems();

                    App.g_IsOrderSubmitting = false;
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await App.g_Shell.GoToHome();
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    }
                }
                else if (response == "X")
                {
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Account disabled.  Please contact customer support.", "Ok");

                            App.g_IsOrderSubmitting = false;
                            await App.g_Shell.GoToHome();
                            App.g_Shell.Logout();
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    }
                }
                else
                {
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Error submitting order.  Please try again.", "Ok");
                            App.g_IsOrderSubmitting = false;
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
            }
        }

        public static async Task commService_SubmitOrder2CompletedAsync(String response)
        {
            try
            {
                if (response == "S")
                {
                    await App.g_db.ClearCartItems();
                    App.g_IsOrderSubmitting = false;

                    await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Thank you! Your order has been placed.", "OK");

                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await App.g_Shell.GoToHome();
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    }
                }
                else if (response == "X")
                {
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Account disabled.  Please contact customer support.", "Ok");

                            App.g_IsOrderSubmitting = false;
                            await App.g_Shell.GoToHome();
                            App.g_Shell.Logout();
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    }
                }
                else
                {
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Error submitting order.  Please try again. ", "Ok");
                            App.g_IsOrderSubmitting = false;
                            await App.g_Shell.GoToHome();
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
            }
        }

        public static async Task<ValidateResponse> commService_ValidateOrderQOHCompletedAsync(String response)
        {
            ValidateResponse validateResponse = new ValidateResponse();
            try
            {
                if (response == "V")
                {
                    try
                    {
                        validateResponse.IsValid = true;
                        validateResponse.Message = "Order is valid.";
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await App.g_Shell.GoToCheckout();
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    }
                }
                else if (response.StartsWith("F~"))
                {
                    string[] aItems = response.Split("~");

                    foreach (string s in aItems)
                    {
                        if (s == "F")
                        {
                            continue;
                        }
                        string[] aDetails = s.Split("|");
                        int iItemNo = 0;
                        int iQOH = 0;
                        try
                        {
                            iItemNo = GetIntegerValue("Item Number", aDetails[0], 0);
                            iQOH = GetIntegerValue("QOH", aDetails[1], 0);
                            try
                            {
                                await App.g_db.UpdateItemQOH(iItemNo, iQOH);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Get Item QOH exception: " + ex.Message + ex.StackTrace);
                            }

                            if (iQOH == 0)
                            {
                                await App.g_db.UpdateItemQtySet(iItemNo, -1);
                            }
                            else
                            {
                                await App.g_db.UpdateItemQtySet(iItemNo, iQOH);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }
                    }
                    validateResponse.IsValid = false;
                    validateResponse.Message = "Some items in your cart are now out of stock.  Please review your shopping cart.";
                }
                else
                {
                    validateResponse.IsValid = false;
                    validateResponse.Message = "Error validating order.  Please try again.";
                }
            }
            catch (Exception ex)
            {
                validateResponse.IsValid = false;
                validateResponse.Message = "Error validating order.  Please try again.";
            }
            return validateResponse;
        }

        public static async Task commService_ValidateOrderCompletedAsyncOld(String response)
        {
            try
            {
                if (response == "S")
                {
                    await App.g_db.ClearCartItems();
                    App.g_IsOrderSubmitting = false;

                    await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Thank you! Your order has been placed.", "OK");

                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await App.g_Shell.GoToHome();
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    }
                }
                else if (response == "X")
                {
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Account disabled.  Please contact customer support.", "Ok");

                            App.g_IsOrderSubmitting = false;
                            await App.g_Shell.GoToHome();
                            App.g_Shell.Logout();
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    }
                }
                else if (response.StartsWith("F~"))
                {
                    string[] aItems = response.Split("~");

                    foreach (string s in aItems)
                    {
                        if (s == "F")
                        {
                            continue;
                        }
                        string[] aDetails = s.Split("|");
                        int iItemNo = 0;
                        int iQOH = 0;
                        try
                        {
                            iItemNo = GetIntegerValue("Item Number", aDetails[0], 0);
                            iQOH = GetIntegerValue("QOH", aDetails[1], 0);
                            try
                            {
                                await App.g_db.UpdateItemQOH(iItemNo, iQOH);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Get Item QOH exception: " + ex.Message + ex.StackTrace);
                            }

                            if (iQOH == 0)
                            {
                                await App.g_db.UpdateItemQtySet(iItemNo, -1);
                            }
                            else
                            {
                                await App.g_db.UpdateItemQtySet(iItemNo, iQOH);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }
                    }

                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Some items in your cart are now out of stock.  Please review your shopping cart.", "Ok");

                            App.g_IsOrderSubmitting = false;
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    }
                }
                else
                {
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Error submitting order.  Please try again.", "Ok");
                            App.g_IsOrderSubmitting = false;
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
            }
        }

        public static async Task commService_GetOrderHistoryCompletedAsync(string response)
        {
            Console.WriteLine("Get Order History Returned");

            try
            {
                if (string.IsNullOrWhiteSpace(response))
                    return;

                string[] aOrders = response.Split(
                    '~',
                    StringSplitOptions.RemoveEmptyEntries);

                if (aOrders.Length == 0)
                    return;

                // Get existing orders once
                List<OrderHeader> existingHeaders =
                    await App.g_db.GetOrderHeaders();

                // O(1) lookup instead of foreach for every order
                HashSet<string> existingOrderNumbers =
                    existingHeaders
                        .Where(x => !string.IsNullOrEmpty(x.OrderNo))
                        .Select(x => x.OrderNo)
                        .ToHashSet();

                // Prevent duplicate headers in current response
                HashSet<string> addedOrderNumbers =
                    new(StringComparer.Ordinal);

                List<OrderHeader> headersToSave = new();
                List<OrderDetail> detailsToSave = new();

                foreach (string s in aOrders)
                {
                    if (string.IsNullOrWhiteSpace(s))
                        continue;

                    string[] aOrder = s.Split('|');

                    // We need at least indexes 0-23
                    if (aOrder.Length < 24)
                        continue;

                    string orderNo = aOrder[0];

                    if (string.IsNullOrWhiteSpace(orderNo))
                        continue;

                    // -----------------------------
                    // ORDER HEADER
                    // -----------------------------
                    if (!existingOrderNumbers.Contains(orderNo) &&
                        addedOrderNumbers.Add(orderNo))
                    {
                        OrderHeader oh = new()
                        {
                            OrderNo = orderNo,
                            CustId = GetIntegerValue(
                                "Customer ID",
                                aOrder[1],
                                0),

                            OrderDate = GetDateTime(
                                "Order Date",
                                aOrder[2]),

                            OrderDateDisplay = aOrder[2],

                            Total = GetDecimalValue(
                                "Order Total",
                                aOrder[3],
                                0),

                            Items = GetIntegerValue(
                                "Order Items",
                                aOrder[4],
                                0),

                            Pieces = GetIntegerValue(
                                "Order Pieces",
                                aOrder[5],
                                0)
                        };

                        oh.TotalDisplay = $"{oh.Total:C}";

                        headersToSave.Add(oh);
                    }

                    // -----------------------------
                    // ORDER DETAIL
                    // -----------------------------
                    OrderDetail od = new()
                    {
                        OrderNo = orderNo,

                        LineNo = GetIntegerValue(
                            "Line Number",
                            aOrder[6],
                            0),

                        ItemNo = GetIntegerValue(
                            "Item Number",
                            aOrder[7],
                            0),

                        ItemNoDisplay = aOrder[7],

                        QtyOrdered = GetIntegerValue(
                            "Quantity Ordered",
                            aOrder[8],
                            0),

                        // Verify that [8] is really shipped quantity.
                        QtyShipped = GetIntegerValue(
                            "Quantity Shipped",
                            aOrder[8],
                            0),

                        Price = GetDecimalValue(
                            "Price",
                            aOrder[9],
                            0),

                        UPC = aOrder[10],

                        Description = aOrder[11],

                        UOM = aOrder[12],

                        SellUnitsInPurch = aOrder[13],

                        SizeDisplay = $"{aOrder[12]}/{aOrder[13]}",

                        SizeUOM = $"/{aOrder[12]}",

                        Size = aOrder[14],

                        Form = aOrder[15],

                        CategoryCode = aOrder[16],

                        CategoryDesc = aOrder[17],

                        SubcategoryCode = aOrder[18],

                        SubcategoryDesc = aOrder[19],

                        VendorId = aOrder[20],

                        VendorName = aOrder[21],

                        Status = aOrder[22],

                        QOH = GetIntegerValue(
                            "QOH",
                            aOrder[23].Trim(),
                            0)
                    };

                    od.PriceDisplay = string.Format("{0:C}", aOrder[9]);

                    od.ItemNoDisplayUPC =
                        string.IsNullOrWhiteSpace(od.UPC)
                            ? string.Empty
                            : $"({od.UPC})";

                    od.IsAvailable =
                        od.Status == "A" &&
                        od.QOH > 0;

                    od.ImageURL =
                        $"{Constants.ItemImageUrl}{od.ItemNo}.jpg";

                    detailsToSave.Add(od);
                }

                // -----------------------------
                // BULK SAVE
                // -----------------------------

                if (headersToSave.Count > 0)
                {
                    await App.g_db.SaveOrderHeader(headersToSave);
                }

                if (detailsToSave.Count > 0)
                {
                    await App.g_db.SaveOrderDetail(detailsToSave);
                }

                // Refresh reorder list once
                App.g_ReorderItemList =
                    await App.g_db.GetReorderItems();

                Console.WriteLine(
                    $"Order History Complete. " +
                    $"Headers: {headersToSave.Count}, " +
                    $"Details: {detailsToSave.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Get Order History Exception: {ex.Message}");

                Console.WriteLine(ex.StackTrace);
            }
        }

        public static async Task commService_GetSettingsCompletedAsync(String response)
        {
            Console.WriteLine("GetSettings Complete");

            try
            {
                String sSettings = response;

                String[] aSettings = sSettings.Split("|");

                if (aSettings[0] == "1")
                {
                    App.g_HoldForReview = true;
                }
                else
                {
                    App.g_HoldForReview = false;
                }
                await App.g_db.SaveSetting("HoldForReview", aSettings[0]);

                try
                {
                    if (aSettings[1] == "1")
                    {
                        App.g_ForceSubmit = true;
                    }
                    else
                    {
                        App.g_ForceSubmit = false;
                    }
                    await App.g_db.SaveSetting("ForceSubmit", aSettings[1]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    App.g_ForceSubmit = false;
                    await App.g_db.SaveSetting("ForceSubmit", "0");
                }

                try
                {
                    App.g_QOHDisplay = aSettings[2];
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    App.g_QOHDisplay = "X";
                }
                await App.g_db.SaveSetting("QOHDisplay", App.g_QOHDisplay);
                App.UpdateProgress(2, "Saving Settings");
                try
                {
                    if (aSettings[3] == "1")
                    {
                        App.g_BlockItemsNoQOH = true;
                    }
                    else
                    {
                        App.g_BlockItemsNoQOH = false;
                    }
                    await App.g_db.SaveSetting("BlockItemsNoQOH", aSettings[3]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                    App.g_BlockItemsNoQOH = false;
                    await App.g_db.SaveSetting("BlockItemsNoQOH", "0");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
            }
        }

        public static async Task commService_GetSalespersonCustomersCompletedAsync(String response)
        {
            try
            {
                Console.WriteLine("Get Salesperson Customers returned");
                Console.WriteLine(response);

                String sCustomers = response;
                String[] aCustomers = sCustomers.Split('~');

                if (aCustomers.Length > 1)
                {


                    await App.g_db.DeleteSalesCustomers();

                    foreach (String s in aCustomers)
                    {
                        String[] aCust = s.Split("|");

                        if (aCust.Count() < 2)
                        {
                            continue;
                        }

                        SalesCustomer c = new SalesCustomer();
                        c.CustNo = aCust[0];
                        c.CompanyName = aCust[1];
                        c.Address1 = aCust[2];
                        c.City = aCust[3];
                        c.State = aCust[4];
                        c.Zip = aCust[5];
                        c.CityStateZip = c.City.Trim() + ", " + c.State.Trim() + " " + c.Zip.Trim();
                        c.ARBalance = 0;
                        c.ARBalance = GetDecimalValue("AR Balance", aCust[6], 0);

                        c.ARBalanceDisplay = string.Format("{0:C2}", c.ARBalance);
                        c.CreditLimit = GetDecimalValue("Credit Limit", aCust[7], 0);
                        if (c.CreditLimit > 0)
                        {
                            c.CreditLimitDisplay = string.Format("{0:C2}", c.CreditLimit);
                        }
                        else
                        {
                            c.CreditLimitDisplay = "N/A";
                        }
                        c.Contact = aCust[8];
                        c.Phone = aCust[9];
                        c.Email = aCust[10];
                        // invoice multiplier aCust[11]
                        c.TermsDesc = aCust[12];
                        try
                        {
                            if (aCust[13] == "0")
                            {
                                c.LastPaymentDate = "N/A";
                            }
                            else
                            {
                                c.LastPaymentDate = aCust[13].Substring(3, 2) + "/";
                                c.LastPaymentDate += aCust[13].Substring(5, 2) + "/";
                                c.LastPaymentDate += aCust[13].Substring(1, 2);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }
                        try
                        {
                            if ((aCust[14] == "0") || (aCust[14] == ""))
                            {
                                c.LastOrderDate = "N/A";
                            }
                            else
                            {
                                c.LastOrderDate = aCust[14].Substring(3, 2) + "/";
                                c.LastOrderDate += aCust[14].Substring(5, 2) + "/";
                                c.LastOrderDate += aCust[14].Substring(1, 2);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }

                        c.MinOrderAmount = GetDecimalValue("Min Order Amount", aCust[15], 0);
                        c.ShippingFee = GetDecimalValue("Shipping Fee", aCust[16], 0);
                        try
                        {
                            await App.g_db.SaveSalesCustomer(c);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
            }
        }

        public static async Task commService_GetFlyerItemsPDFCompleted(String response)
        {
            Console.WriteLine("GetFlyerItemsPDFCompleted");
        }

        public static async Task commService_ValidateUserActiveCompletedAsync(String response)
        {
            String sUser = response;
            if (sUser == "0")
            {
                try
                {
                    await App.g_db.SaveSetting("LoggedIn", "0");
                    await App.g_db.SaveSetting("UserName", App.g_UserName);

                    try
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await App.g_Shell.GoToLogin();
                        });
                    }
                    catch
                    {
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                }
            }
        }

        public static DateTime GetDateTime(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DateTime.MinValue;

            value = value.Trim();

            // First try exact formats
            string[] formats =
            {
                "M/d/yyyy",
                "MM/dd/yyyy",
                "yyyy-MM-dd",
                "yyyyMMdd",
                "M/d/yy",
                "MM/dd/yy"
            };

            if (DateTime.TryParseExact(
                    value,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
            {
                return date;
            }

            // Fallback to normal parsing
            if (DateTime.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date))
            {
                return date;
            }

            Console.WriteLine($"{key} Invalid Date: '{value}'");

            return DateTime.MinValue;
        }
        public static int GetIntegerValue(String key, String value, int defaultValue)
        {
            try
            {
                string sizeValue = value.Trim();
                if (sizeValue.Length > 0)
                {
                    string digits = new string(sizeValue
                    .TakeWhile(char.IsDigit)
                    .ToArray());

                    return int.TryParse(digits, out var size)
                        ? size
                        : defaultValue;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(key + "Converting string to int" + e.Message);
                return defaultValue;
            }
        }

        public static Decimal GetDecimalValue(String key, String value, Decimal defaultValue)
        {
            try
            {
                string sizeValue = value.Trim();
                if (sizeValue.Length != 0)
                    return Convert.ToDecimal(sizeValue);
                else
                    return defaultValue;
            }
            catch (Exception e)
            {
                Console.WriteLine(key + "Converting string to Decimal " + e.Message);
                return defaultValue;
            }
        }
    }
}
