using SQLite;

namespace POMuswick
{
    public class Database
    {
        readonly SQLiteConnection _database;
        static object locker = new object();

        public Database()
        {
            string dbPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.DBName);

            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<Item>();
            _database.CreateTable<Customer>();
            _database.CreateTable<Banner>();
            _database.CreateTable<Category>();
            _database.CreateTable<Subcategory>();
            _database.CreateTable<Setting>();
            _database.CreateTable<PaymentToken>();
            _database.CreateTable<Location>();
            _database.CreateTable<OrderHeader>();
            _database.CreateTable<OrderDetail>();
            _database.CreateTable<OrderSubmitted>();
            _database.CreateTable<ReorderItem>();
            _database.CreateTable<CartItem>();
            _database.CreateTable<DiscontinuedItem>();
            _database.CreateTable<SuspendItem>();
            _database.CreateTable<SalesCustomer>();
        }

        public void BeginTransaction()
        {
            _database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _database.Commit();
        }

        public void RollbackTransaction()
        {
            _database.Rollback();
        }

        public List<Item> SearchItems(String sSearch, Category category, String sBarcode, Subcategory subcategory)
        {
            Decimal dItemNo = 0;

            String sBarcodeShort = sBarcode;
            if (sBarcode.Length > 11)
            {
                sBarcodeShort = sBarcodeShort.Substring(0, 11);
            }
            try
            {
                if ((sBarcode.Length <= 6) && (sBarcode != ""))
                {
                    dItemNo = Decimal.Parse(sBarcode);
                    sBarcodeShort = dItemNo.ToString();

                    category.Code = "";
                    subcategory.Code = "";
                }
            }
            catch
            {
            }

            if (sSearch != "")
            {
                if (Decimal.TryParse(sSearch, out dItemNo))
                {
                    dItemNo = Decimal.Parse(sSearch);
                    sBarcodeShort = dItemNo.ToString();

                    category.Code = "";
                    subcategory.Code = "";
                }
            }

            sSearch = sSearch.Replace("'", "");

            String sSearchShort = sSearch;
            if (sSearch.Length > 11)
            {
                sSearchShort = sSearchShort.Substring(0, 11);
            }

            String sQuery = "SELECT * FROM [Item] WHERE ";

            if (!string.IsNullOrEmpty(sBarcode))
            {
                sQuery += " (";
                sQuery += " [ItemNoDisplay] = '" + sBarcode + "' OR ";
                sQuery += " [ItemNoDisplay] = '" + sBarcodeShort + "' OR ";

                sQuery += " ([UPC_1] LIKE '%" + sBarcode + "%' OR [UPC_1] LIKE '%" + sBarcodeShort + "') OR ";
                sQuery += " ([UPC_2] LIKE '%" + sBarcode + "%' OR [UPC_2] LIKE '%" + sBarcodeShort + "') OR ";
                sQuery += " ([UPC_3] LIKE '%" + sBarcode + "%' OR [UPC_3] LIKE '%" + sBarcodeShort + "') OR ";
                sQuery += " ([UPC_4] LIKE '%" + sBarcode + "%' OR [UPC_4] LIKE '%" + sBarcodeShort + "') ";
                sQuery += " ) ";
            }
            else
            {
                sQuery += " (";
                sQuery += " [SearchDescription] LIKE '%" + sSearch + "%' OR ";
                sQuery += " [ItemNoDisplay] LIKE '%" + sSearch + "%' ";

                if (dItemNo > 0)
                {
                    sQuery += " OR [ItemNoDisplay] LIKE '%" + dItemNo.ToString() + "%' ";
                }

                sQuery += " OR ([UPC_1] LIKE '%" + sSearch + "%' OR [UPC_1] LIKE '%" + sSearchShort + "') ";
                sQuery += " OR ([UPC_2] LIKE '%" + sSearch + "%' OR [UPC_2] LIKE '%" + sSearchShort + "') ";
                sQuery += " OR ([UPC_3] LIKE '%" + sSearch + "%' OR [UPC_3] LIKE '%" + sSearchShort + "') ";
                sQuery += " OR ([UPC_4] LIKE '%" + sSearch + "%' OR [UPC_4] LIKE '%" + sSearchShort + "') ";
                sQuery += " ) ";
            }

            if (!string.IsNullOrEmpty(category.Code))
            {
                if (category.Code == "NEW ITEMS")
                    sQuery += " AND NewItem = 'Y' ";
                else
                    sQuery += " AND CategoryCode = '" + category.Code + "' ";
            }

            if (App.g_InStockOnly)
            {
                sQuery += " AND QOH > 0 ";
            }

            sQuery += " AND Status = 'A' ";


            // 🔥 SMART ORDERING
            if (!string.IsNullOrEmpty(sBarcode))
            {
                sQuery += " ORDER BY ";
                sQuery += " CASE ";
                sQuery += " WHEN [ItemNoDisplay] = '" + sBarcode + "' THEN 1 ";
                sQuery += " WHEN [ItemNoDisplay] = '" + sBarcodeShort + "' THEN 1 ";
                sQuery += " WHEN [UPC_1] LIKE '" + sBarcode + "%' THEN 2 ";
                sQuery += " WHEN [UPC_2] LIKE '" + sBarcode + "%' THEN 2 ";
                sQuery += " WHEN [UPC_3] LIKE '" + sBarcode + "%' THEN 2 ";
                sQuery += " WHEN [UPC_4] LIKE '" + sBarcode + "%' THEN 2 ";
                sQuery += " ELSE 3 END, ";
                sQuery += " SearchDescription ";
            }
            else
            {
                sQuery += " ORDER BY ";
                sQuery += " CASE ";
                sQuery += " WHEN [ItemNoDisplay] LIKE '" + sSearch + "%' THEN 1 ";
                sQuery += " WHEN [SearchDescription] LIKE '" + sSearch + "%' THEN 2 ";
                sQuery += " WHEN [ItemNoDisplay] LIKE '%" + sSearch + "%' THEN 3 ";
                sQuery += " WHEN [SearchDescription] LIKE '%" + sSearch + "%' THEN 4 ";
                sQuery += " ELSE 5 END, ";
                sQuery += " SearchDescription ";
            }

            return _database.Query<Item>(sQuery);
        }

        public List<Item> SearchItemsKeyword(String sSearch)
        {
            var search = sSearch?.Trim() ?? "";

            var query = $@"
            SELECT * FROM [Item]
            WHERE Status = 'A'
            {(App.g_InStockOnly ? "AND QOH > 0" : "")}
            AND (
                ([Keyword1] LIKE ? AND [Keyword1] <> '') OR
                ([Keyword2] LIKE ? AND [Keyword2] <> '') OR
                ([Keyword3] LIKE ? AND [Keyword3] <> '')
            )
            ORDER BY
                CASE
                    WHEN [Keyword1] = ? OR [Keyword2] = ? OR [Keyword3] = ? THEN 1
                    WHEN [Keyword1] LIKE ? OR [Keyword2] LIKE ? OR [Keyword3] LIKE ? THEN 2
                    ELSE 3
                END,
                Description ASC";

            return _database.Query<Item>(
                query,
                "%" + search + "%",   // WHERE
                "%" + search + "%",
                "%" + search + "%",

                search,               // exact match
                search,
                search,

                search + "%",         // starts with
                search + "%",
                search + "%"
            );
        }

        public List<Item> SearchItemsQuickEntry(String sSearch)
        {
            Decimal dItemNo = 0;
            try
            {
                if ((sSearch.Length <= 6) && (sSearch != ""))
                {
                    dItemNo = Decimal.Parse(sSearch);
                }
            }
            catch
            {
            }

            String sSearch2 = "";

            sSearch = sSearch.Replace("'", "");
            if (sSearch.Length >= 6 && sSearch.Length <= 8)
            {
                sSearch2 = sSearch;
                string sUPCExpand = UPCExpand(sSearch);
                if (sUPCExpand != "")
                {
                    sSearch = sUPCExpand;
                }
            }

            String sSearchShort = sSearch;
            String sSearchShort2 = sSearch;
            if (sSearch.Length == 13)
            {
                sSearchShort = sSearchShort.Substring(2, 11);
            }
            else if (sSearch.Length > 11)
            {
                sSearchShort2 = sSearchShort2.Substring(0, 11);
            }

            String sQuery = "select * from [Item] where ";

            sQuery += " ((([UPC_1] like '%" + sSearch + "%' or [UPC_1] like '%" + sSearchShort + "' or [UPC_1] like '%" + sSearchShort2 + "') and [UPC_1] > '') or ";
            sQuery += " (([UPC_2] like '%" + sSearch + "%' or [UPC_2] like '%" + sSearchShort + "' or [UPC_2] like '%" + sSearchShort2 + "') and [UPC_2] > '') or ";
            sQuery += " (([UPC_3] like '%" + sSearch + "%' or [UPC_3] like '%" + sSearchShort + "' or [UPC_3] like '%" + sSearchShort2 + "') and [UPC_3] > '') or ";
            sQuery += " (([UPC_4] like '%" + sSearch + "%' or [UPC_4] like '%" + sSearchShort + "' or [UPC_4] like '%" + sSearchShort2 + "') and [UPC_4] > '') ";

            if (sSearch2 != "")
            {
                sQuery += " or ([UPC_1] = '" + sSearch2 + "' or [UPC_2] = '" + sSearch2 + "' or [UPC_3] = '" + sSearch2 + "' or [UPC_4] = '" + sSearch2 + "') ";
            }

            if (dItemNo > 0)
            {
                sQuery += " or ([ItemNoDisplay] like '%" + dItemNo.ToString() + "%')";
            }

            sQuery += " ) and Status <> 'D' ";

            return _database.Query<Item>(sQuery);
        }

        private string UPCExpand(string sUPC)
        {
            string sUPCExpand = "";

            if (sUPC.Length == 8)
            {
                //return UPC8Expand(sUPC);
                sUPC = sUPC.Substring(1, 6);
            }

            if (sUPC.Length == 6)
            {
                sUPC = "0" + sUPC;
            }

            string D1 = sUPC.Substring(0, 1);
            string D2 = sUPC.Substring(1, 1);
            string D3 = sUPC.Substring(2, 1);
            string D4 = sUPC.Substring(3, 1);
            string D5 = sUPC.Substring(4, 1);
            string D6 = sUPC.Substring(5, 1);
            string D7 = sUPC.Substring(6, 1);

            switch (D7)
            {
                case "0":
                    sUPCExpand = D1 + D2 + D3 + "00000" + D4 + D5 + D6;
                    break;

                case "1":
                    sUPCExpand = D1 + D2 + D3 + D7 + "0000" + D4 + D5 + D6;
                    break;

                case "2":
                    sUPCExpand = D1 + D2 + D3 + D7 + "0000" + D4 + D5 + D6;
                    break;

                case "3":
                    sUPCExpand = D1 + D2 + D3 + D4 + "00000" + D5 + D6;
                    break;

                case "4":
                    sUPCExpand = D1 + D2 + D3 + D4 + D5 + "00000" + D6;
                    break;

                case "5":
                    sUPCExpand = D1 + D2 + D3 + D4 + D5 + D6 + "0000" + D7;
                    break;

                case "6":
                    sUPCExpand = D1 + D2 + D3 + D4 + D5 + D6 + "0000" + D7;
                    break;

                case "7":
                    sUPCExpand = D1 + D2 + D3 + D4 + D5 + D6 + "0000" + D7;
                    break;

                case "8":
                    sUPCExpand = D1 + D2 + D3 + D4 + D5 + D6 + "0000" + D7;
                    break;

                case "9":
                    sUPCExpand = D1 + D2 + D3 + D4 + D5 + D6 + "0000" + D7;
                    break;

                default:
                    sUPCExpand = "";
                    break;
            }

            return sUPCExpand;
        }

        public List<Item> GetNewItems(String sSearch, bool bQOHOnly)
        {
            String sQuery = "select * from [Item] where NewItem = 'Y' and Status = 'A' ";

            if (sSearch != "")
            {
                sSearch = sSearch.Replace("'", "");

                sQuery += " AND (";
                sQuery += " [Description] LIKE '%" + sSearch + "%' OR ";
                sQuery += " [ItemNoDisplay] LIKE '%" + sSearch + "%' OR ";
                sQuery += " (([UPC_1] LIKE '%" + sSearch + "%') AND [UPC_1] > '') OR ";
                sQuery += " (([UPC_2] LIKE '%" + sSearch + "%') AND [UPC_2] > '') OR ";
                sQuery += " (([UPC_3] LIKE '%" + sSearch + "%') AND [UPC_3] > '') OR ";
                sQuery += " (([UPC_4] LIKE '%" + sSearch + "%') AND [UPC_4] > '') ";
                sQuery += ") ";
            }

            if (App.g_InStockOnly || bQOHOnly)
            {
                sQuery += " AND QOH > 0 ";
            }

            if (sSearch != "")
            {
                sQuery += " ORDER BY ";
                sQuery += " CASE ";
                sQuery += " WHEN [Description] LIKE '" + sSearch + "%' THEN 1 ";
                sQuery += " WHEN [ItemNoDisplay] LIKE '" + sSearch + "%' THEN 1 ";
                sQuery += " ELSE 2 END, ";
                sQuery += " DateAdded DESC ";
            }
            else
            {
                sQuery += " ORDER BY DateAdded DESC ";
            }

            return _database.Query<Item>(sQuery);
        }

        public int InsertDiscontinuedItems()
        {
            String sQuery = "delete from [DiscontinuedItem]";
            _database.Execute(sQuery);

            sQuery = "insert into [DiscontinuedItem] select ItemNo from [Item]";
            return _database.Execute(sQuery);
        }

        public int DeleteDiscontinuedItem(string ItemNo)
        {
            String sQuery = "delete from [DiscontinuedItem] where ItemNo = " + ItemNo;
            return _database.Execute(sQuery);
        }

        public int UpdateDiscontinuedItems()
        {
            String sQuery = "update [Item] set Status = 'D' where ItemNo in (select ItemNo from [DiscontinuedItem])";
            return _database.Execute(sQuery);
        }

        public List<Item> GetCartItems()
        {
            String sQuery = "select * from [Item] where QtyOrder <> 0 order by Description";
            return _database.Query<Item>(sQuery);
        }

        public List<Item> GetCheckoutItems()
        {
            String sQuery = "select * from [Item] where QtyOrder > 0 order by Description";
            return _database.Query<Item>(sQuery);
        }

        public int GetCartPieces()
        {
            String sQuery = "select sum(QtyOrder) from [Item] where QtyOrder > 0";
            return _database.ExecuteScalar<int>(sQuery);
        }

        public int ClearCartItems()
        {
            String sQuery = "update [Item] set QtyOrder = 0, PriceOrder = 0";
            return _database.Execute(sQuery);
        }

        public int GetItemCount()
        {
            String sQuery = "select count(*) from [Item]";
            return _database.ExecuteScalar<int>(sQuery);
        }

        public Item FindItem(int item_no)
        {
            return _database.Find<Item>(s => s.ItemNo == item_no);
        }

        public int SaveItem(Item item)
        {
            return _database.InsertOrReplace(item);
        }

        public int SaveItemReplace(Item item)
        {
            return _database.InsertOrReplace(item);
            /*
            Item _item = FindItem(item.ItemNo);

            if (_item == null)
            {
                return _database.Insert(item);
            }
            else
            {
                item.QtyOrder = _item.QtyOrder;
                _database.Update(item);

                UpdateItemPriceOrder(item.ItemNo);

                return 1;
            }
            */
        }

        public int UpdateItem(Item item)
        {
            return _database.Update(item);
        }

        public int DeleteItems()
        {
            return _database.Execute("delete from Item");
        }

        public List<Item> GetItems()
        {
            String sQuery = "select * from [Item] ";
            return _database.Query<Item>(sQuery);
        }

        public int UpdateItemQty(int iItem, int iQty)
        {
            _database.Execute("update Item set QtyOrder = QtyOrder + " + iQty.ToString() + " where ItemNo = " + iItem.ToString());

            UpdateItemPriceOrder(iItem);

            try
            {
                Vibration.Vibrate(200);
            }
            catch (Exception e)
            {
            }

            return 1;
        }

        public int UpdateItemQtySet(int iItem, int iQty)
        {
            _database.Execute("update Item set QtyOrder = " + iQty.ToString() + " where ItemNo = " + iItem.ToString());

            UpdateItemPriceOrder(iItem);

            try
            {
                Vibration.Vibrate(200);
            }
            catch (Exception e)
            {
            }

            return 1;
        }

        public int UpdateItemPriceOrder(int iItem)
        {
            /*
            Item item = _database.Find<Item>(s => s.ItemNo == iItem);

            if (item.QtyBreak3 >= item.QtyOrder)
            {
                item.PriceOrder = item.PriceBreak3;
            }
            else if (item.QtyBreak2 >= item.QtyOrder)
            {
                item.PriceOrder = item.PriceBreak2;
            }
            else if (item.QtyBreak1 >= item.QtyOrder)
            {
                item.PriceOrder = item.PriceBreak1;
            }
            else
            {
                item.PriceOrder = item.Price;
            }
            item.PriceOrderDisplay = string.Format("{0:C}", item.PriceOrder);

            SaveItem(item);
            */
            return 1;
        }

        public int UpdateItemQOH(int iItem, int iQOH)
        {
            _database.Execute("update Item set QOH = " + iQOH.ToString() + " where ItemNo = " + iItem.ToString());
            _database.Execute("update ReorderItem set QOH = " + iQOH.ToString() + " where ItemNo = " + iItem.ToString());
            _database.Execute("update OrderDetail set QOH = " + iQOH.ToString() + " where ItemNo = " + iItem.ToString());

            return 1;
        }

        public int GetItemQty(int iItem)
        {
            return _database.ExecuteScalar<int>("select QtyOrder from Item where ItemNo = " + iItem.ToString());
        }

        public List<Category> GetCategories()
        {
            String sQuery = "select * from Category ";
            sQuery += " union ";
            sQuery += " select 'NEW ITEMS', 'NEW ITEMS', '', 0, -1, '' from Category order by Rank, Description";

            return _database.Query<Category>(sQuery);
        }

        public List<Category> GetHomePageCategories()
        {
            String sQuery = "select * from Category where HomePage > 0 order by HomePage limit 4";
            return _database.Query<Category>(sQuery);
        }

        public Category GetCategory(string sCategoryCode)
        {
            return _database.Find<Category>(s => s.Code == sCategoryCode);
        }

        public int SaveCategory(Category categorys)
        {
            return _database.Insert(categorys);
        }

        public int DeleteCategory(Category category)
        {
            return _database.Delete(category);
        }

        public int DeleteCategories()
        {
            return _database.Execute("delete from Category");
        }

        public List<Subcategory> GetSubcategory()
        {
            return _database.Table<Subcategory>().OrderBy(t => t.Description).ToList();
        }

        public List<Subcategory> GetSubcategory(string sCategoryCode)
        {
            String sQuery = "select * from Subcategory where Category = '" + sCategoryCode + "' order by Description";
            return _database.Query<Subcategory>(sQuery);
        }

        public int SaveSubcategory(Subcategory subcategorys)
        {
            return _database.Insert(subcategorys);
        }

        public int DeleteSubcategory(Subcategory subcategory)
        {
            return _database.Delete(subcategory);
        }

        public int DeleteSubcategories()
        {
            return _database.Execute("delete from Subcategory");
        }

        public int DeleteBannersAsync()
        {
            return _database.Execute("delete from Banner");
        }

        public int SaveBannerAsync(Banner banner)
        {
            return _database.Insert(banner);
        }

        public List<Banner> GetBanners()
        {
            return _database.Table<Banner>().OrderBy(t => t.BannerName).ToList();
        }

        public int SaveCustomer(Customer cust)
        {
            _database.Delete(cust);
            return _database.Insert(cust);
        }


        public Customer GetCustomer()
        {
            //String sQuery = "select * from Customer limit 1";
            return _database.Find<Customer>(s => s.CustId == -1);
        }

        public int DeleteCustomer()
        {
            _database.Execute("delete from Customer");
            return 0;
        }

        public string GetSetting(string sKey)
        {
            try
            {
                var _setting = _database.Find<Setting>(s => s.Key == sKey);

                if (_setting != null)
                {
                    return _setting.Value;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public int SaveSetting(string sKey, string sValue)
        {
            Setting setting = new Setting();
            setting.Key = sKey;
            setting.Value = sValue;

            return _database.InsertOrReplace(setting);
        }

        public int SaveLocation(Location location)
        {
            return _database.InsertOrReplace(location);
        }

        public int DeleteLocations()
        {
            return _database.Execute("delete from Location");
        }

        public Location GetLocation(int iLocation)
        {
            return _database.Find<Location>(s => s.LocationId == iLocation);
        }

        public int SaveOrderHeader(OrderHeader oh)
        {
            return _database.InsertOrReplace(oh);
        }

        public List<OrderHeader> GetOrderHeaders()
        {
            //return _database.Table<OrderHeader>().OrderByDescending(t => t.OrderDate).ToList();

            String sQuery = "select * from [OrderHeader] where [CustId] = " + App.g_Customer.CustNo + " order by OrderDate desc";

            return _database.Query<OrderHeader>(sQuery);
        }

        public OrderHeader GetOrderHeader(string sOrderNo)
        {
            return _database.Find<OrderHeader>(s => s.OrderNo == sOrderNo);
        }

        public int DeleteOrderHistory()
        {
            _database.Execute("delete from OrderHeader");
            _database.Execute("delete from OrderDetail");
            return 0;
        }

        public int SaveOrderDetail(OrderDetail od)
        {
            return _database.InsertOrReplace(od);
        }

        public int DeleteOrderDetail(string sOrderNo)
        {
            return _database.Execute("delete from OrderDetail where OrderNo = '" + sOrderNo + "'");
        }

        public List<OrderDetail> GetOrderDetail(string sOrderNo)
        {
            String sQuery = "select * from OrderDetail where OrderNo = '" + sOrderNo + "' order by Description";
            return _database.Query<OrderDetail>(sQuery);
        }

        public List<Item> GetReorderItems()
        {
            String sQuery = "select * from Item where Status = 'A' and LastPurchDateDisplay > '' order by LastPurchDate desc, Description";
            return _database.Query<Item>(sQuery);
        }

        public List<ReorderItem> GetReorderItemsOld()
        {
            String sQuery = "select * from ReorderItem where Status = 'A' order by LastPurchDate desc, Description";
            return _database.Query<ReorderItem>(sQuery);
        }

        public int SaveReorderItem(ReorderItem ri)
        {
            return _database.InsertOrReplace(ri);
        }

        public int GetReorderItemsCount()
        {
            String sQuery = "select count(*) from [Item] where LastPurchDateDisplay > ''";
            return _database.ExecuteScalar<int>(sQuery);
        }

        public int DeleteReorderItems()
        {
            return _database.Execute("delete from ReorderItem");
        }

        public int DeleteSavedCartItems()
        {
            return _database.Execute("delete from CartItem");
        }

        public int SaveCartItems()
        {
            String sQuery = "insert into CartItem select ItemNo, QtyOrder, QtyOnOrderSellUnit1, QtyOnOrderSellUnit3, QtyOnOrderSellUnit3, QtyOnOrderSellUnit4  from [Item] where QtyOrder > 0 or QtyOnOrderSellUnit1 > 0 or QtyOnOrderSellUnit2 > 0 or QtyOnOrderSellUnit3 > 0 or QtyOnOrderSellUnit4 > 0";
            return _database.Execute(sQuery);
        }

        public List<CartItem> GetSavedCartItems()
        {
            String sQuery = "select * from CartItem";
            return _database.Query<CartItem>(sQuery);
        }
        public int DeleteSalesCustomers()
        {
            return _database.Execute("delete from [SalesCustomer]");
        }

        public List<SalesCustomer> GetSalesCustomers()
        {
            String sQuery = "select * from [SalesCustomer] ";
            return _database.Query<SalesCustomer>(sQuery);
        }

        public List<SalesCustomer> GetSalesCustomers(string SearchCustomer)
        {
            String sOrderBy = " order by CompanyName ";
            String sQuery = "select * from [SalesCustomer] ";

            if (SearchCustomer != null)
            {
                if (SearchCustomer.Trim().Replace("'", "") != "")
                {
                    sQuery += " where (CompanyName like '%" + SearchCustomer.Trim().Replace("'", "") + "%' ";
                    sQuery += " or CustNo = '" + SearchCustomer.Trim() + "') ";
                }
            }

            sQuery += sOrderBy;

            return _database.Query<SalesCustomer>(sQuery);
        }

        public SalesCustomer FindSalesCustomer(string CustNo)
        {
            return _database.Find<SalesCustomer>(s => s.CustNo == CustNo);
        }

        public int SaveSalesCustomer(SalesCustomer cust)
        {
            _database.Delete(cust);
            return _database.Insert(cust);
        }

        public int SuspendCartItems(string CustNo)               
        {
            string sQuery = "INSERT INTO SuspendItem (CustNo, ItemNo, QtyOrder) " +
                   "SELECT '" + CustNo + "', ItemNo, QtyOrder " +
                   "FROM [Item] WHERE QtyOrder > 0";
            //String sQuery = "insert into SuspendItem select '" + CustNo + "', ItemNo, QtyOrder, from [Item] where QtyOrder > 0";
            return _database.Execute(sQuery);
        }

        public List<SuspendItem> GetSuspendedCartItems(string CustNo)
        {
            String sQuery = "select * from SuspendItem where CustNo = '" + CustNo + "'";
            return _database.Query<SuspendItem>(sQuery);
        }

        public int RestoreCartItems(string CustNo)
        {
            List<SuspendItem> items = GetSuspendedCartItems(CustNo);

            foreach (SuspendItem item in items)
            {
                if (item.QtyOrder > 0)
                {
                    UpdateItemQtySet(item.ItemNo, item.QtyOrder);
                }
            }

            DeleteSuspendedCartItems(CustNo);

            return 0;
        }

        public int DeleteSuspendedCartItems(string CustNo)
        {
            return _database.Execute("delete from SuspendItem where CustNo = '" + CustNo + "'");
        }
    }
}
