namespace POMuswick.Views
{
    public partial class HomePage : ContentPage
    {
        public String code1;
        public String description1;
        public String imageURL1;
        public String code2;
        public String description2;
        public String imageURL2;
        public String code3;
        public String description3;
        public String imageURL3;
        public String code4;
        public String description4;
        public String imageURL4;
        List<Item> lstItems;

        public string Code1
        {
            get { return code1; }
            set
            {
                code1 = value;
                OnPropertyChanged();
            }
        }
        public string Description1
        {
            get { return description1; }
            set
            {
                description1 = value;
                OnPropertyChanged();
            }
        }
        public string ImageURL1
        {
            get { return imageURL1; }
            set
            {
                imageURL1 = value;
                OnPropertyChanged();
            }
        }
        public string Code2
        {
            get { return code2; }
            set
            {
                code2 = value;
                OnPropertyChanged();
            }
        }
        public string Description2
        {
            get { return description2; }
            set
            {
                description2 = value;
                OnPropertyChanged();
            }
        }
        public string ImageURL2
        {
            get { return imageURL2; }
            set
            {
                imageURL2 = value;
                OnPropertyChanged();
            }
        }
        public string Code3
        {
            get { return code3; }
            set
            {
                code3 = value;
                OnPropertyChanged();
            }
        }
        public string Description3
        {
            get { return description3; }
            set
            {
                description3 = value;
                OnPropertyChanged();
            }
        }
        public string ImageURL3
        {
            get { return imageURL3; }
            set
            {
                imageURL3 = value;
                OnPropertyChanged();
            }
        }
        public string Code4
        {
            get { return code4; }
            set
            {
                code4 = value;
                OnPropertyChanged();
            }
        }
        public string Description4
        {
            get { return description4; }
            set
            {
                description4 = value;
                OnPropertyChanged();
            }
        }
        public string ImageURL4
        {
            get { return imageURL4; }
            set
            {
                imageURL4 = value;
                OnPropertyChanged();
            }
        }

        public HomePage()
        {
            InitializeComponent();

            BindingContext = this;

            App.g_HomePage = this;

            RefreshNewItemsList();

            BannerImage.Source = ImageSource.FromUri(new Uri(Constants.LogoUrl));
            RequestCameraPermission();
            InitializeTimer();
        }

        async void RequestCameraPermission()
        {
            // Check current status
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status == PermissionStatus.Granted)
            {
                // Permission already granted, proceed with camera action
                // e.g., StartCamera();
            }
            else if (status == PermissionStatus.Denied && OperatingSystem.IsAndroid())
            {
                // Android specific: If denied, shouldShowRationale might tell you if you can ask again
                if (Permissions.ShouldShowRationale<Permissions.Camera>())
                {
                    // Show an alert to explain why you need it, then request again
                    if (await DisplayAlertAsync("Permission Needed", "We need camera access to take photos. Allow access?", "OK", "Cancel"))
                    {
                        await Permissions.RequestAsync<Permissions.Camera>();
                    }
                }
            }
            else if (status != PermissionStatus.Granted) // For iOS/Others if not granted or just denied
            {
                // Request permission
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status == PermissionStatus.Granted)
                {
                    // Permission granted after request
                    // e.g., StartCamera();
                }
                else
                {
                    // Permission denied permanently (iOS) or still denied (Android)
                    // Inform the user they need to enable it in settings.
                }
            }
        }

        private void InitializeTimer()
        {
            Dispatcher.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                try
                {
                    MainThread.BeginInvokeOnMainThread(UpdateBanner);
                    return true;
                }
                catch (Exception ex) { return false; }
            });
        }

        private async void UpdateBanner()
        {
            //Database db = new Database();
            var banners = App.g_db.GetBanners();

            try
            {
                if (banners.Count == 0)
                {
                    BannerImage.Source = ImageSource.FromUri(new Uri(Constants.LogoUrl));
                    return;
                }
            }
            catch (Exception ex)
            {
                BannerImage.Source = ImageSource.FromUri(new Uri(Constants.LogoUrl));
                return;
            }

            int iNextIndex = 0;
            String CurrentBanner = BannerImage.Source.ToString();

            foreach (var b in banners)
            {
                iNextIndex++;

                if (CurrentBanner.Contains(b.BannerName))
                {
                    break;
                }
            }

            if (iNextIndex >= banners.Count)
            {
                iNextIndex = 0;
            }

            Banner banner = banners[iNextIndex];

            BannerImage.Source = ImageSource.FromUri(new Uri(banner.BannerURL));
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (!App.g_IsLoggedIn)
            {
                await App.g_Shell.GoToLogin();
                return;
            }

            App.g_CurrentPage = "HomePage";

            if (App.g_Customer.Status == "3")
            {
                //await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Registration request has been completed.  Please check your email for instructions.", "Ok");
                return;
            }

            SetLoginControls();

            App.g_Category.Code = "";
            App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            App.g_SearchText = "";
            App.g_ScanBarcode = "";
            SearchText.Text = "";

            RefreshNewItemsList();
        }

        public void SetLoginControls()
        {
            lblWelcome.Text = "Welcome - " + App.g_UserName;
            if (App.g_Customer.CustNo != "0")
            {
                lblUserName.Text = App.g_Customer.CustNo + " - " + App.g_Customer.CompanyName;
            }
            else
            {
                lblUserName.Text = "";
            }

            App.g_Shell.HideCustomerMenu();
            App.g_Shell.HideMyAccountMenu();

            if (App.g_IsSalesUser)
            {
                App.g_Shell.ShowCustomerMenu();
            }
            else
            {
                App.g_Shell.ShowMyAccountMenu();
            }
        }

        async void CategoryTapped(String Code, String Description)
        {
            Category cat = new Category();
            cat.Code = Code;
            cat.Description = Description;

            App.g_Category = cat;
            App.g_ScanBarcode = "";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            await App.g_Shell.GoToItemSearch();
            //App.g_Shell.GoToSubcategories();

            try
            {
                App.g_SearchPage.RefreshList();
            }
            catch
            {
            }
        }

        void OnTapped1(object sender, EventArgs e)
        {
            CategoryTapped(Code1, Description1);
        }

        void OnTapped2(object sender, EventArgs e)
        {
            CategoryTapped(Code2, Description2);
        }

        void OnTapped3(object sender, EventArgs e)
        {
            CategoryTapped(Code3, Description3);
        }

        void OnTapped4(object sender, EventArgs e)
        {
            CategoryTapped(Code4, Description4);
        }

        async void OnCategoryTapped(object sender, EventArgs e)
        {
            TappedEventArgs te = (TappedEventArgs)e;

            string CategoryCode = (string)te.Parameter;

            //Database db = new Database();
            Category cat = App.g_db.GetCategory(CategoryCode);

            App.g_Category.Code = cat.Code;
            App.g_Category.Description = cat.Description;
            App.g_Category.ImageURL = cat.ImageURL;

            App.g_ScanBarcode = "";
            App.g_SearchText = "";

            await App.g_Shell.GoToItemSearch();
        }

        async void OnSignInClick(object sender, EventArgs e)
        {
            if (!App.g_IsLoggedIn)
            {
                await App.g_Shell.GoToLogin();
            }
            else
            {
                ConfirmLogout();
            }
        }

        public async void ConfirmLogout()
        {
            bool bLogout = await DisplayAlertAsync("Muswick Wholesale Grocers", "Are you sure you wish to logout?", "Yes", "No");

            if (bLogout)
            {
                //Database db = new Database();
                App.g_db.SaveSetting("LoggedIn", "0");
                App.g_IsLoggedIn = false;
                if (App.g_IsSalesUser)
                {
                    try
                    {
                        App.g_db.SuspendCartItems(App.g_Customer.CustNo);
                        App.g_db.ClearCartItems();
                    }
                    catch { }
                }

                App.g_db.DeleteCustomer();
                App.g_Customer = new Customer();

                SetLoginControls();
                await App.g_Shell.GoToLogin();
            }
        }

        async void OnShopNow(object sender, EventArgs e)
        {
            await App.g_Shell.GoToCategories();
        }

        public async void RefreshCategoryList()
        {
            //CategoryList.ItemsSource = null;
            //CategoryList.ItemsSource = App.g_HomePageCategoryList;
        }

        async void OnNewItemsAll(object sender, EventArgs e)
        {
            App.g_Category.Code = "NEW ITEMS";
            App.g_Category.Description = "NEW ITEMS";
            await App.g_Shell.GoToItemSearch();
        }

        public async void RefreshNewItemsList()
        {
            NewItemList.ItemsSource = null;

            lstItems = App.g_db.GetNewItems("", true);

            foreach (Item i in lstItems)
            {
                i.IsLoggedIn = App.g_IsLoggedIn;

                i.IsStepperVisible = false;
                i.IsAddToOrderVisible = true;

                try
                {
                    if (i.QtyOrder > 0)
                    {
                        i.IsStepperVisible = true;
                        i.IsAddToOrderVisible = false;
                    }
                }
                catch (Exception ex)
                {
                    i.IsStepperVisible = false;
                    i.IsAddToOrderVisible = true;
                }

                i.IsQOHBlackVisible = false;
                i.IsQOHRedVisible = false;
                if (App.g_QOHDisplay == "Q")
                {
                    i.IsQOHVisible = true;
                    i.IsInStockVisible = false;
                    i.IsOutOfStockVisible = false;
                    if (i.QOH > 0)
                    {
                        i.IsQOHBlackVisible = true;
                    }
                    else
                    {
                        i.IsQOHRedVisible = true;
                    }
                }
                else if (App.g_QOHDisplay == "I")
                {
                    i.IsQOHVisible = false;
                    if (i.QOH > 0)
                    {
                        i.IsInStockVisible = true;
                        i.IsOutOfStockVisible = false;
                    }
                    else
                    {
                        i.IsInStockVisible = false;
                        i.IsOutOfStockVisible = true;
                    }
                }
                else
                {
                    i.IsQOHVisible = false;
                    i.IsInStockVisible = false;
                    i.IsOutOfStockVisible = false;
                }
                if (i.IsQOHVisible || i.IsInStockVisible || i.IsOutOfStockVisible)
                {
                    i.IsStockRowVisible = true;
                }
                else
                {
                    i.IsStockRowVisible = false;
                }

                if ((i.MaxOrderQty > 0) && (i.MaxOrderQty < 9999))
                {
                    i.IsMaxOrderQtyVisible = true;
                    i.MaxOrderQtyDisplay = "Max " + i.MaxOrderQty.ToString();
                }

                if (App.g_BlockItemsNoQOH)
                {
                    if (i.QOH == 0)
                    {
                        i.IsStepperVisible = false;
                        i.IsAddToOrderVisible = false;
                        i.IsMaxOrderQtyVisible = false;
                    }
                }

                if (i.ItemNo == 89770)
                {
                    i.MaxOrderQtyDisplay = i.MaxOrderQtyDisplay;
                }
            }

            NewItemList.ItemsSource = lstItems;
        }

        async void OnPastPurchases(object sender, EventArgs e)
        {
            //Database db = new Database();
            int iReorderItems = App.g_db.GetReorderItemsCount();

            if (iReorderItems == 0)
            {
                await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Past purchases not found", "Ok");
            }
            else
            {
                await App.g_Shell.GoToReorderItems();
            }
        }

        async void OnRegisterClick(object sender, EventArgs e)
        {
            try
            {

                if (App.g_Customer.Status == "3")
                {
                    await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Registration request has been completed.  Please check your email for instructions.", "Ok");
                    return;
                }
                else if (App.g_Customer.Status == "4")
                {
                    await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Registration request needs further review.  Please check your email for instructions.", "Ok");
                    return;
                }
                else if (App.g_Customer.Status == "8")
                {
                    await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Registration request denied.  Please contact customer service for assistance.", "Ok");
                    return;
                }
                else if (App.g_Customer.Status == "9")
                {
                    await Shell.Current.DisplayAlertAsync("Muswick Wholesale Grocers", "Registration active.", "Ok");
                    return;
                }
            }
            catch (Exception ex)
            {
            }

            await App.g_Shell.GoToRegister();
        }

        protected override bool OnBackButtonPressed()
        {
            // ignore button
            return true;

            // if want to allow back button
            //base.OnBackButtonPressed();
            //return false;
        }

        private double startScale;
        private double currentScale;
        private double xOffset;
        private double yOffset;
        void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                startScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
            }
            if (e.Status == GestureStatus.Running)
            {
                currentScale += (e.Scale - 1) * startScale;
                currentScale = Math.Max(1, currentScale);
                double renderedX = Content.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;
                double renderedY = Content.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * startScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;
                double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);
                Content.TranslationX = Math.Min(0, Math.Max(targetX, -Content.Width * (currentScale - 1)));
                Content.TranslationY = Math.Min(0, Math.Max(targetY, -Content.Height * (currentScale - 1)));
                Content.Scale = currentScale;
            }
            if (e.Status == GestureStatus.Completed)
            {
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
            }
        }

        void OnMenuTapped(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = true;
        }

        async void OnSearchTapped(object sender, EventArgs e)
        {
            App.g_SearchText = SearchText.Text;
            App.g_SearchFromPage = "HomePage";

            await App.g_Shell.GoToItemSearch();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length >= 3)
            {
                App.g_SearchText = SearchText.Text;
                App.g_SearchFromPage = "HomePage";

                //await App.g_Shell.GoToHome();
            }
        }
    }
}
