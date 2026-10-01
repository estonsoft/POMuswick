using Realms;

namespace POMuswick
{
    public class Database
    {
        private readonly RealmConfiguration _config;
        private Realm _mainThreadRealm;
        public Database()
        {
            // Realm config safely mapped to the application sandbox data directory on iOS and Android
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, Constants.DBName ?? "appdata.realm");

            _config = new RealmConfiguration(dbPath)
            {
                SchemaVersion = 1,
                // Automatically handles schema migrations during rapid local prototyping phases
                MigrationCallback = (migration, oldSchemaVersion) => { }
            };
        }

        // Helper to retrieve thread-confined local instance dynamically on each execution request
        private Realm GetRealm()
        {
            // If on the UI thread, keep a persistent instance alive
            if (MainThread.IsMainThread)
            {
                if (_mainThreadRealm == null || _mainThreadRealm.IsClosed)
                {
                    _mainThreadRealm = Realm.GetInstance(_config);
                }
                return _mainThreadRealm;
            }

            // Background threads spin up transient instances dynamically
            return Realm.GetInstance(_config);
        }


        #region Smart Item Search Engine
        public List<Item> SearchItems(bool stockOnly, string sSearch, Category category, string sBarcode, Subcategory subcategory)
        {
            // Save category parameters before evaluating any structural reset conditionals
            string sSavedCategoryCode = category.Code;
            string sSavedSubcategoryCode = subcategory.Code;

            decimal dItemNo = 0;
            string sBarcodeShort = sBarcode ?? "";
            if (sBarcodeShort.Length > 11)
            {
                sBarcodeShort = sBarcodeShort.Substring(0, 11);
            }

            if (!string.IsNullOrEmpty(sBarcodeShort) && sBarcodeShort.Length <= 6 && decimal.TryParse(sBarcodeShort, out var parsedBarcode))
            {
                dItemNo = parsedBarcode;
                sBarcodeShort = dItemNo.ToString();
                sSavedCategoryCode = "";
                sSavedSubcategoryCode = "";
            }

            if (!string.IsNullOrEmpty(sSearch))
            {
                if (decimal.TryParse(sSearch, out var parsedSearchId))
                {
                    dItemNo = parsedSearchId;
                    sBarcodeShort = dItemNo.ToString();
                    sSavedCategoryCode = "";
                    sSavedSubcategoryCode = "";
                }
            }

            string cleanSearch = (sSearch ?? "").Replace("'", "");
            string sSearchShort = cleanSearch;
            if (sSearchShort.Length > 11)
            {
                sSearchShort = sSearchShort.Substring(0, 11);
            }

            var realm = GetRealm();
            var query = realm.All<Item>().Where(i => i.Status == "A");

            if (stockOnly)
            {
                query = query.Where(i => i.QOH > 0);
            }

            if (!string.IsNullOrEmpty(sSavedCategoryCode))
            {
                query = sSavedCategoryCode == "NEW ITEMS"
                    ? query.Where(i => i.NewItem == "Y")
                    : query.Where(i => i.CategoryCode == sSavedCategoryCode);
            }

            if (!string.IsNullOrEmpty(sSavedSubcategoryCode))
            {
                query = query.Where(i => i.SubcategoryCode == sSavedSubcategoryCode);
            }

            if (!string.IsNullOrEmpty(sBarcode))
            {
                query = query.Where(i => i.ItemNoDisplay == sBarcode ||
                                         i.ItemNoDisplay == sBarcodeShort ||
                                         i.UPC_1.Contains(sBarcode) || i.UPC_1.Contains(sBarcodeShort) ||
                                         i.UPC_2.Contains(sBarcode) || i.UPC_2.Contains(sBarcodeShort) ||
                                         i.UPC_3.Contains(sBarcode) || i.UPC_3.Contains(sBarcodeShort) ||
                                         i.UPC_4.Contains(sBarcode) || i.UPC_4.Contains(sBarcodeShort));
            }
            else if (!string.IsNullOrEmpty(cleanSearch))
            {
                string dItemNoStr = dItemNo > 0 ? dItemNo.ToString() : "";
                var numericItemMatches = string.IsNullOrEmpty(dItemNoStr)
                    ? new List<Item>()
                    : query.Where(i => i.ItemNoDisplay.Contains(dItemNoStr)).ToList();

                query = query.Where(i => i.SearchDescription.Contains(cleanSearch) ||
                                         i.ItemNoDisplay.Contains(cleanSearch) ||
                                         i.UPC_1.Contains(cleanSearch) || i.UPC_1.Contains(sSearchShort) ||
                                         i.UPC_2.Contains(cleanSearch) || i.UPC_2.Contains(sSearchShort) ||
                                         i.UPC_3.Contains(cleanSearch) || i.UPC_3.Contains(sSearchShort) ||
                                         i.UPC_4.Contains(cleanSearch) || i.UPC_4.Contains(sSearchShort));

                if (numericItemMatches.Count > 0)
                {
                    query = query.AsEnumerable()
                                 .Concat(numericItemMatches)
                                 .DistinctBy(i => i.ItemNo)
                                 .AsQueryable();
                }
            }

            // Memory-mapped queries run evaluations natively in the C++ layer during collection transformations
            var resultList = query.ToList();

            if (!string.IsNullOrEmpty(sBarcode))
            {
                return resultList.OrderBy(i => i.ItemNoDisplay == sBarcode || i.ItemNoDisplay == sBarcodeShort ? 1 :
                                               i.UPC_1.StartsWith(sBarcode) || i.UPC_2.StartsWith(sBarcode) ||
                                               i.UPC_3.StartsWith(sBarcode) || i.UPC_4.StartsWith(sBarcode) ? 2 : 3)
                                 .ThenBy(i => i.SearchDescription)
                                 .ToList()
                                 .Select(item => item.CopyDetached())
                                 .ToList();
            }

            if (!string.IsNullOrEmpty(cleanSearch))
            {
                return resultList.OrderBy(i => i.ItemNoDisplay.StartsWith(cleanSearch) ? 1 :
                                               i.SearchDescription.StartsWith(cleanSearch) ? 2 :
                                               i.ItemNoDisplay.Contains(cleanSearch) ? 3 :
                                               i.SearchDescription.Contains(cleanSearch) ? 4 : 5)
                                 .ThenBy(i => i.SearchDescription)
                                 .ToList()
                                 .Select(item => item.CopyDetached())
                                 .ToList();
            }

            return resultList.Select(item => item.CopyDetached()).ToList();
        }

        public List<Item> SearchItemsKeyword(string sSearch, bool stockOnly)
        {
            var search = sSearch?.Trim() ?? "";
            var realm = GetRealm();

            var query = realm.All<Item>().Where(i => i.Status == "A");

            if (stockOnly)
            {
                query = query.Where(i => i.QOH > 0);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(i => (i.Keyword1.Contains(search) && i.Keyword1 != "") ||
                                         (i.Keyword2.Contains(search) && i.Keyword2 != "") ||
                                         (i.Keyword3.Contains(search) && i.Keyword3 != ""));
            }

            return query.ToList()
                        .OrderBy(i => i.Keyword1 == search || i.Keyword2 == search || i.Keyword3 == search ? 1 :
                                       i.Keyword1.StartsWith(search) || i.Keyword2.StartsWith(search) || i.Keyword3.StartsWith(search) ? 2 : 3)
                        .ThenBy(i => i.Description)
                        .ToList()
                        .Select(item => item.CopyDetached())
                        .ToList();
        }

        public List<Item> SearchItemsQuickEntry(string sSearch)
        {
            decimal dItemNo = 0;
            if (!string.IsNullOrEmpty(sSearch) && sSearch.Length <= 6 && decimal.TryParse(sSearch, out var parsed))
            {
                dItemNo = parsed;
            }

            string sSearch2 = "";
            string cleanSearch = (sSearch ?? "").Replace("'", "");

            if (cleanSearch.Length >= 6 && cleanSearch.Length <= 8)
            {
                sSearch2 = cleanSearch;
                string expanded = UPCExpand(cleanSearch);
                if (!string.IsNullOrEmpty(expanded))
                {
                    cleanSearch = expanded;
                }
            }

            string sSearchShort = cleanSearch;
            string sSearchShort2 = cleanSearch;
            if (cleanSearch.Length == 13)
            {
                sSearchShort = cleanSearch.Substring(2, 11);
            }
            else if (cleanSearch.Length > 11)
            {
                sSearchShort2 = cleanSearch.Substring(0, 11);
            }

            var realm = GetRealm();
            var query = realm.All<Item>().Where(i => i.Status != "D");

            string dItemNoStr = dItemNo > 0 ? dItemNo.ToString() : "";

            var matchingItems = query.Where(i =>
                ((i.UPC_1.Contains(cleanSearch) || i.UPC_1.Contains(sSearchShort) || i.UPC_1.Contains(sSearchShort2)) && i.UPC_1 != "") ||
                ((i.UPC_2.Contains(cleanSearch) || i.UPC_2.Contains(sSearchShort) || i.UPC_2.Contains(sSearchShort2)) && i.UPC_2 != "") ||
                ((i.UPC_3.Contains(cleanSearch) || i.UPC_3.Contains(sSearchShort) || i.UPC_3.Contains(sSearchShort2)) && i.UPC_3 != "") ||
                ((i.UPC_4.Contains(cleanSearch) || i.UPC_4.Contains(sSearchShort) || i.UPC_4.Contains(sSearchShort2)) && i.UPC_4 != ""))
                .ToList();

            if (!string.IsNullOrEmpty(sSearch2))
            {
                matchingItems.AddRange(query.Where(i => i.UPC_1 == sSearch2 || i.UPC_2 == sSearch2 || i.UPC_3 == sSearch2 || i.UPC_4 == sSearch2).ToList());
            }

            if (!string.IsNullOrEmpty(dItemNoStr))
            {
                matchingItems.AddRange(query.Where(i => i.ItemNoDisplay.Contains(dItemNoStr)).ToList());
            }

            return matchingItems.DistinctBy(i => i.ItemNo)
                                .Select(item => item.CopyDetached())
                                .ToList();
        }

        private string UPCExpand(string sUPC)
        {
            if (string.IsNullOrEmpty(sUPC)) return "";
            if (sUPC.Length == 8) sUPC = sUPC.Substring(1, 6);
            if (sUPC.Length == 6) sUPC = "0" + sUPC;
            if (sUPC.Length < 7) return "";

            string D1 = sUPC.Substring(0, 1);
            string D2 = sUPC.Substring(1, 1);
            string D3 = sUPC.Substring(2, 1);
            string D4 = sUPC.Substring(3, 1);
            string D5 = sUPC.Substring(4, 1);
            string D6 = sUPC.Substring(5, 1);
            string D7 = sUPC.Substring(6, 1);

            return D7 switch
            {
                "0" => D1 + D2 + D3 + "00000" + D4 + D5 + D6,
                "1" => D1 + D2 + D3 + D7 + "0000" + D4 + D5 + D6,
                "2" => D1 + D2 + D3 + D7 + "0000" + D4 + D5 + D6,
                "3" => D1 + D2 + D3 + D4 + "00000" + D5 + D6,
                "4" => D1 + D2 + D3 + D4 + D5 + "00000" + D6,
                _ => D1 + D2 + D3 + D4 + D5 + D6 + "0000" + D7
            };
        }

        public List<Item> GetNewItems(string sSearch, bool bQOHOnly, bool stockOnly)
        {
            var realm = GetRealm();
            var query = realm.All<Item>().Where(i => i.NewItem == "Y" && i.Status == "A");

            if (!string.IsNullOrEmpty(sSearch))
            {
                string cleanSearch = sSearch.Replace("'", "");
                query = query.Where(i => i.Description.Contains(cleanSearch) ||
                                         i.ItemNoDisplay.Contains(cleanSearch) ||
                                         (i.UPC_1.Contains(cleanSearch) && i.UPC_1 != "") ||
                                         (i.UPC_2.Contains(cleanSearch) && i.UPC_2 != "") ||
                                         (i.UPC_3.Contains(cleanSearch) && i.UPC_3 != "") ||
                                         (i.UPC_4.Contains(cleanSearch) && i.UPC_4 != ""));
            }

            if (stockOnly || bQOHOnly)
            {
                query = query.Where(i => i.QOH > 0);
            }

            var results = query.ToList();

            if (!string.IsNullOrEmpty(sSearch))
            {
                string cleanSearch = sSearch.Replace("'", "");
                return results.OrderBy(i => i.Description.StartsWith(cleanSearch) || i.ItemNoDisplay.StartsWith(cleanSearch) ? 1 : 2)
                              .ThenByDescending(i => i.DateAdded)
                              .ToList()
                              .Select(item => item.CopyDetached())
                              .ToList();
            }

            return results.OrderByDescending(i => i.DateAdded)
                          .Select(item => item.CopyDetached())
                          .ToList();
        }
        #endregion

        #region Discontinued Processing Matrix
        public int InsertDiscontinuedItems()
        {
            var realm = GetRealm();
            var allItems = realm.All<Item>().ToList();

            realm.Write(() =>
            {
                realm.RemoveAll<DiscontinuedItem>();
                foreach (var item in allItems)
                {
                    realm.Add(new DiscontinuedItem { ItemNo = item.ItemNo });
                }
            });
            return allItems.Count;
        }

        public void DeleteDiscontinuedItems(List<int> itemNos)
        {
            if (itemNos == null || itemNos.Count == 0) return;

            var realm = GetRealm();
            var itemsToDelete = realm.All<DiscontinuedItem>()
                                     .AsEnumerable()
                                     .Where(d => itemNos.Contains(d.ItemNo))
                                     .ToList();

            realm.Write(() =>
            {
                foreach (var item in itemsToDelete)
                {
                    realm.Remove(item);
                }
            });
        }

        public int DeleteDiscontinuedItem(string itemNoStr)
        {
            if (!int.TryParse(itemNoStr, out int targetNo)) return 0;

            var realm = GetRealm();
            var item = realm.All<DiscontinuedItem>().FirstOrDefault(d => d.ItemNo == targetNo);

            if (item == null) return 0;

            realm.Write(() => realm.Remove(item));
            return 1;
        }

        public int UpdateDiscontinuedItems()
        {
            var realm = GetRealm();
            var targetIds = realm.All<DiscontinuedItem>().ToList().Select(d => d.ItemNo).ToList();
            var matches = realm.All<Item>().AsEnumerable().Where(i => targetIds.Contains(i.ItemNo)).ToList();

            realm.Write(() =>
            {
                foreach (var item in matches)
                {
                    item.Status = "D";
                }
            });
            return matches.Count;
        }
        #endregion

        #region Cart & Order Collections
        public List<Item> GetCartItems()
        {
            var realm = GetRealm();
            return realm.All<Item>()
                        .Where(i => i.QtyOrder > 0)
                        .OrderBy(i => i.Description)
                        .ToList()
                        .Select(item => item.CopyDetached())
                        .ToList();
        }

        public List<Item> GetCheckoutItems()
        {
            var realm = GetRealm();
            return realm.All<Item>()
                        .Where(i => i.QtyOrder > 0)
                        .OrderBy(i => i.Description)
                        .ToList()
                        .Select(item => item.CopyDetached())
                        .ToList();
        }

        public int GetCartPieces()
        {
            var realm = GetRealm();
            return realm.All<Item>()
                        .Where(i => i.QtyOrder > 0)
                        .ToList()
                        .Sum(i => i.QtyOrder);
        }

        public int ClearCartItems()
        {
            var realm = GetRealm();
            var items = realm.All<Item>().Where(i => i.QtyOrder != 0 || i.PriceOrder != 0).ToList();

            realm.Write(() =>
            {
                foreach (var i in items)
                {
                    i.QtyOrder = 0;
                    i.PriceOrder = 0;
                }
            });
            return items.Count;
        }
        #endregion

        #region Standard Object Management (CRUD)
        public int GetItemCount()
        {
            var realm = GetRealm();
            return realm.All<Item>().Count();
        }

        public Item? FindItem(int itemNo)
        {
            var realm = GetRealm();
            return realm.Find<Item>(itemNo)?.CopyDetached();
        }

        public int SaveItems(List<Item> items)
        {
            if (items == null) return 0;
            var realm = GetRealm();
            realm.Write(() =>
            {
                foreach (var item in items) realm.Add(item, update: true);
            });
            return items.Count;
        }

        public int SaveItemReplace(Item item)
        {
            if (item == null) return 0;
            var realm = GetRealm();
            realm.Write(() => realm.Add(item, update: true));
            return 1;
        }

        public int UpdateItem(Item item)
        {
            return SaveItemReplace(item);
        }

        public int DeleteItems()
        {
            var realm = GetRealm();
            int count = realm.All<Item>().Count();
            realm.Write(() => realm.RemoveAll<Item>());
            return count;
        }

        public List<Item> GetItems()
        {
            var realm = GetRealm();
            return realm.All<Item>()
                        .ToList()
                        .Select(item => item.CopyDetached())
                        .ToList();
        }
        #endregion

        #region Structured Inventory Adjustments
        public int UpdateItemQty(int iItem, int iQty)
        {
            var realm = GetRealm();
            var item = realm.Find<Item>(iItem);
            if (item == null) return 0;

            realm.Write(() => { item.QtyOrder += iQty; });
            return 1;
        }

        public int UpdateItemQtySet(Dictionary<int, int> keyValuePairs)
        {
            if (keyValuePairs == null || keyValuePairs.Count == 0) return 0;

            var realm = GetRealm();
            realm.Write(() =>
            {
                foreach (var kvp in keyValuePairs)
                {
                    var item = realm.Find<Item>(kvp.Key);
                    if (item != null)
                    {
                        item.QtyOrder = kvp.Value;
                    }
                }
            });
            return keyValuePairs.Count;
        }

        public int UpdateItemQOH(Dictionary<int, int> keyValuePairs)
        {
            if (keyValuePairs == null || keyValuePairs.Count == 0) return 0;

            var realm = GetRealm();
            realm.Write(() =>
            {
                foreach (var kvp in keyValuePairs)
                {
                    int itemNo = kvp.Key;
                    int newQoh = kvp.Value;

                    var item = realm.Find<Item>(itemNo);
                    if (item != null) item.QOH = newQoh;

                    var reorder = realm.Find<ReorderItem>(itemNo);
                    if (reorder != null) reorder.QOH = newQoh;

                    var details = realm.All<OrderDetail>().Where(d => d.ItemNo == itemNo);
                    foreach (var d in details) d.QOH = newQoh;
                }
            });
            return keyValuePairs.Count;
        }

        public int GetItemQty(int iItem)
        {
            var realm = GetRealm();
            var item = realm.Find<Item>(iItem);
            return item?.QtyOrder ?? 0;
        }
        #endregion

        #region Categorization Methods
        public List<Category> GetCategories()
        {
            var realm = GetRealm();
            var baseList = realm.All<Category>()
                                .OrderBy(c => c.Rank)
                                .ThenBy(c => c.Description)
                                .ToList()
                                .Select(category => category.CopyDetached())
                                .ToList();

            var compositeList = new List<Category>
            {
                new Category { Code = "NEW ITEMS", Description = "NEW ITEMS", Rank = -1 }
            };
            compositeList.AddRange(baseList);
            return compositeList;
        }

        public List<Category> GetHomePageCategories()
        {
            var realm = GetRealm();
            return realm.All<Category>()
                        .Where(c => c.HomePage > 0)
                        .OrderBy(c => c.HomePage)
                        .ToList()
                        .Take(4)
                        .Select(category => category.CopyDetached())
                        .ToList();
        }

        public Category? GetCategory(string sCategoryCode)
        {
            var realm = GetRealm();
            return realm.Find<Category>(sCategoryCode)?.CopyDetached();
        }

        public int DeleteAllCategory()
        {
            var realm = GetRealm();
            int count = realm.All<Category>().Count();
            realm.Write(() => realm.RemoveAll<Category>());
            return count;
        }

        public int SaveCategory(List<Category> categories)
        {
            if (categories == null) return 0;
            var realm = GetRealm();
            realm.Write(() =>
            {
                foreach (var cat in categories) realm.Add(cat, update: true);
            });
            return categories.Count;
        }

        public int DeleteCategory(Category category)
        {
            if (category == null) return 0;
            var realm = GetRealm();
            var match = realm.Find<Category>(category.Code);
            if (match == null) return 0;

            realm.Write(() => realm.Remove(match));
            return 1;
        }

        public int DeleteCategories() => DeleteAllCategory();

        public List<Subcategory> GetSubcategory()
        {
            var realm = GetRealm();
            return realm.All<Subcategory>()
                        .OrderBy(s => s.Description)
                        .ToList()
                        .Select(subcategory => subcategory.CopyDetached())
                        .ToList();
        }

        public List<Subcategory> GetSubcategory(string sCategoryCode)
        {
            var realm = GetRealm();
            return realm.All<Subcategory>()
                        .Where(s => s.Category == sCategoryCode)
                        .OrderBy(s => s.Description)
                        .ToList()
                        .Select(subcategory => subcategory.CopyDetached())
                        .ToList();
        }

        public int DeleteAllSubcategory()
        {
            var realm = GetRealm();
            int count = realm.All<Subcategory>().Count();
            realm.Write(() => realm.RemoveAll<Subcategory>());
            return count;
        }

        public int SaveSubcategory(List<Subcategory> subcategories)
        {
            if (subcategories == null) return 0;
            var realm = GetRealm();
            realm.Write(() =>
            {
                foreach (var sub in subcategories) realm.Add(sub, update: true);
            });
            return subcategories.Count;
        }

        public int DeleteSubcategory(Subcategory subcategory)
        {
            if (subcategory == null) return 0;
            var realm = GetRealm();
            var match = realm.Find<Subcategory>(subcategory.Code);
            if (match == null) return 0;

            realm.Write(() => realm.Remove(match));
            return 1;
        }

        public int DeleteSubcategories() => DeleteAllSubcategory();
        #endregion

        #region Graphic Banner Management
        public int DeleteBannersAsync()
        {
            var realm = GetRealm();
            int count = realm.All<Banner>().Count();
            realm.Write(() => realm.RemoveAll<Banner>());
            return count;
        }

        public int SaveBannerAsync(List<Banner> banners)
        {
            if (banners == null) return 0;
            var realm = GetRealm();
            realm.Write(() =>
            {
                foreach (var b in banners) realm.Add(b, update: true);
            });
            return banners.Count;
        }

        public List<Banner> GetBanners()
        {
            var realm = GetRealm();
            return realm.All<Banner>()
                        .OrderBy(b => b.BannerName)
                        .ToList()
                        .Select(banner => banner.CopyDetached())
                        .ToList();
        }
        #endregion

        #region B2B Identity Profiles
        public int SaveCustomer(Customer cust)
        {
            if (cust == null) return 0;
            var realm = GetRealm();
            realm.Write(() =>
            {
                var match = realm.Find<Customer>(cust.CustId);
                if (match != null) realm.Remove(match);
                realm.Add(cust);
            });
            return 1;
        }

        public Customer? GetCustomer()
        {
            var realm = GetRealm();
            return realm.Find<Customer>(-1);
        }

        public int DeleteCustomer()
        {
            var realm = GetRealm();
            realm.Write(() => realm.RemoveAll<Customer>());
            return 0;
        }
        #endregion

        #region Dynamic Key-Value Configuration Engine
        public string GetString(string sKey)
        {
            try
            {
                var realm = GetRealm();
                var setting = realm.Find<Setting>(sKey);
                return setting?.Value ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving setting for key: {sKey}. Exception: {ex.Message}");
                return "";
            }
        }

        public int SaveString(string sKey, string sValue)
        {
            var realm = GetRealm();
            realm.Write(() =>
            {
                realm.Add(new Setting { Key = sKey, Value = sValue }, update: true);
            });
            return 1;
        }
        #endregion

        #region Location Logistics Matrix
        public int SaveLocation(Location location)
        {
            if (location == null) return 0;
            var realm = GetRealm();
            realm.Write(() => realm.Add(location, update: true));
            return 1;
        }

        public int DeleteLocations()
        {
            var realm = GetRealm();
            realm.Write(() => realm.RemoveAll<Location>());
            return 0;
        }

        public Location? GetLocation(int iLocation)
        {
            var realm = GetRealm();
            return realm.Find<Location>(iLocation);
        }
        #endregion

        #region B2B Invoice History Engine
        public int SaveOrderHeader(OrderHeader oh)
        {
            if (oh == null) return 0;
            var realm = GetRealm();
            realm.Write(() => realm.Add(oh, update: true));
            return 1;
        }

        public int SaveAllOrderHeader(List<OrderHeader> ohList)
        {
            if (ohList == null) return 0;
            var realm = GetRealm();
            realm.Write(() =>
            {
                foreach (var oh in ohList) realm.Add(oh, update: true);
            });
            return ohList.Count;
        }

        public List<OrderHeader> GetOrderHeaders(string customerNumber)
        {
            if (!int.TryParse(customerNumber, out var customerId) || customerId.ToString() != customerNumber)
            {
                return new List<OrderHeader>();
            }

            var realm = GetRealm();
            return realm.All<OrderHeader>()
                        .Where(oh => oh.CustId == customerId)
                        .ToList()
                        .OrderByDescending(oh => oh.OrderDate)
                        .ToList()
                        .Select(order => order.CopyDetached())
                        .ToList();
        }

        public OrderHeader? GetOrderHeader(string sOrderNo)
        {
            var realm = GetRealm();
            return realm.Find<OrderHeader>(sOrderNo)?.CopyDetached();
        }

        public int DeleteOrderHistory()
        {
            var realm = GetRealm();
            realm.Write(() =>
            {
                realm.RemoveAll<OrderHeader>();
                realm.RemoveAll<OrderDetail>();
            });
            return 0;
        }

        public int SaveAllOrderDetail(List<OrderDetail> odList)
        {
            if (odList == null) return 0;
            var realm = GetRealm();
            realm.Write(() =>
            {
                foreach (var od in odList) realm.Add(od, update: true);
            });
            return odList.Count;
        }

        public int SaveOrderDetail(OrderDetail od)
        {
            if (od == null) return 0;
            var realm = GetRealm();
            realm.Write(() => realm.Add(od, update: true));
            return 1;
        }

        public int DeleteOrderDetail(string sOrderNo)
        {
            var realm = GetRealm();
            var targets = realm.All<OrderDetail>().Where(d => d.OrderNo == sOrderNo).ToList();
            realm.Write(() =>
            {
                foreach (var t in targets) realm.Remove(t);
            });
            return targets.Count;
        }

        public List<OrderDetail> GetOrderDetail(string sOrderNo)
        {
            var realm = GetRealm();
            return realm.All<OrderDetail>()
                        .Where(d => d.OrderNo == sOrderNo)
                        .OrderBy(d => d.Description)
                        .ToList()
                        .Select(detail => detail.CopyDetached())
                        .ToList();
        }
        #endregion

        #region Dynamic Stock Reordering Triggers
        public List<Item> GetReorderItems()
        {
            var realm = GetRealm();
            return realm.All<Item>()
                        .Where(i => i.Status == "A" && i.LastPurchDateDisplay != "")
                        .ToList()
                        .OrderByDescending(i => i.LastPurchDate)
                        .ThenBy(i => i.Description)
                        .ToList()
                        .Select(item => item.CopyDetached())
                        .ToList();
        }

        public List<ReorderItem> GetReorderItemsOld()
        {
            var realm = GetRealm();
            return realm.All<ReorderItem>()
                        .Where(r => r.Status == "A")
                        .ToList()
                        .OrderByDescending(r => r.LastPurchDate)
                        .ThenBy(r => r.Description)
                        .ToList();
        }

        public int SaveReorderItem(ReorderItem ri)
        {
            if (ri == null) return 0;
            var realm = GetRealm();
            realm.Write(() => realm.Add(ri, update: true));
            return 1;
        }

        public int GetReorderItemsCount()
        {
            var realm = GetRealm();
            return realm.All<Item>().Where(i => i.LastPurchDateDisplay != "").Count();
        }

        public int DeleteReorderItems()
        {
            var realm = GetRealm();
            realm.Write(() => realm.RemoveAll<ReorderItem>());
            return 0;
        }
        #endregion

        #region Session Cache Serialization
        public int DeleteSavedCartItems()
        {
            var realm = GetRealm();
            realm.Write(() => realm.RemoveAll<CartItem>());
            return 0;
        }

        public int SaveCartItems()
        {
            var realm = GetRealm();
            var sourceItems = realm.All<Item>()
                                   .Where(i => i.QtyOrder > 0)
                                   .ToList();

            realm.Write(() =>
            {
                foreach (var i in sourceItems)
                {
                    realm.Add(new CartItem
                    {
                        ItemNo = i.ItemNo,
                        QtyOrder = i.QtyOrder
                    });
                }
            });
            return sourceItems.Count;
        }

        public List<CartItem> GetSavedCartItems()
        {
            var realm = GetRealm();
            return realm.All<CartItem>().ToList();
        }
        #endregion

        #region Client Management & Suspended Checkouts
        public int DeleteSalesCustomers()
        {
            var realm = GetRealm();
            realm.Write(() => realm.RemoveAll<SalesCustomer>());
            return 0;
        }

        public List<SalesCustomer> GetSalesCustomers()
        {
            var realm = GetRealm();
            return realm.All<SalesCustomer>()
                        .ToList()
                        .Select(customer => customer.CopyDetached())
                        .ToList();
        }

        public List<SalesCustomer> GetSalesCustomers(string searchCustomer)
        {
            var realm = GetRealm();
            var query = realm.All<SalesCustomer>();

            if (!string.IsNullOrEmpty(searchCustomer))
            {
                string cleanStr = searchCustomer.Trim().Replace("'", "");
                query = query.Where(c => c.CompanyName.Contains(cleanStr) || c.CustNo == searchCustomer.Trim());
            }

            return query.OrderBy(c => c.CompanyName)
                        .ToList()
                        .Select(customer => customer.CopyDetached())
                        .ToList();
        }

        public List<SalesCustomer> GetSalesCustomers(string searchCustomer, int skip, int take)
        {
            var realm = GetRealm();
            var query = realm.All<SalesCustomer>();

            if (!string.IsNullOrEmpty(searchCustomer))
            {
                string cleanStr = searchCustomer.Trim().Replace("'", "");
                query = query.Where(c => c.CompanyName.Contains(cleanStr) || c.CustNo == searchCustomer.Trim());
            }

            return query.OrderBy(c => c.CompanyName)
                        .ToList()
                        .Skip(skip)
                        .Take(take)
                        .Select(customer => customer.CopyDetached())
                        .ToList();
        }

        public SalesCustomer? FindSalesCustomer(string custNo)
        {
            var realm = GetRealm();
            return realm.Find<SalesCustomer>(custNo)?.CopyDetached();
        }

        public int SaveSalesCustomer(List<SalesCustomer> salesCustomers)
        {
            if (salesCustomers == null) return 0;
            var realm = GetRealm();
            realm.Write(() =>
            {
                foreach (var sc in salesCustomers) realm.Add(sc, update: true);
            });
            return salesCustomers.Count;
        }

        public int SuspendCartItems(string custNo)
        {
            var realm = GetRealm();
            var activeItems = realm.All<Item>().Where(i => i.QtyOrder > 0).ToList();

            realm.Write(() =>
            {
                foreach (var i in activeItems)
                {
                    realm.Add(new SuspendItem
                    {
                        CustNo = custNo,
                        ItemNo = i.ItemNo,
                        QtyOrder = i.QtyOrder
                    });
                }
            });
            return activeItems.Count;
        }

        public List<SuspendItem> GetSuspendedCartItems(string custNo)
        {
            var realm = GetRealm();
            return realm.All<SuspendItem>()
                        .Where(s => s.CustNo == custNo)
                        .ToList();
        }

        public int RestoreCartItems(string custNo)
        {
            var items = GetSuspendedCartItems(custNo);
            var qtyUpdates = items.Where(x => x.QtyOrder > 0)
                                  .ToDictionary(x => x.ItemNo, x => x.QtyOrder);

            UpdateItemQtySet(qtyUpdates);
            DeleteSuspendedCartItems(custNo);
            return 0;
        }

        public int DeleteSuspendedCartItems(string custNo)
        {
            var realm = GetRealm();
            var targets = realm.All<SuspendItem>().Where(s => s.CustNo == custNo).ToList();

            realm.Write(() =>
            {
                foreach (var t in targets) realm.Remove(t);
            });
            return targets.Count;
        }
        #endregion
    }
}