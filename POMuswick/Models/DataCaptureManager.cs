using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Source;

namespace POMuswick.Models
{
    public class DataCaptureManager
    {
        private static readonly Lazy<DataCaptureManager> instance =
            new Lazy<DataCaptureManager>(() => new DataCaptureManager(), LazyThreadSafetyMode.PublicationOnly);

        public static DataCaptureManager Instance => instance.Value;

        public bool IsLicenseValid { get; private set; }

        private DataCaptureManager()
        {
            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                this.DataCaptureContext = DataCaptureContext.ForLicenseKey("ARjSC52BCaoaK+oT/C4YK3gFmGl6HY0VanY0QrVshuAsRyDm5WOymLhry3GXT+DAlGntSz4TkEebaPPan2kiGioGgB1ydTUQeyYqpR5V9MFfZWqLyECMKhlEABOyGLIIOAXYa2MC49XLcNbzR8v4zR92dQxOBHFeFq/h+ugW2bTd52lj8wD8ttNM7cugqZ6d5NvRDlNFbDf99BvF7ZfTSqVDasX8QS7Kzsw+75J1ijv99f/NSXtJXoBP18vWBWNzCdKoEbd/FeNsHjcffyIuNA+zPr6ARQ1Rg5gcClH51iODPX995tBsjDdXDHDD5xg3fvIr6JbdksbyQf6UCNJ/kapNGiWUjdZ5+Nxa25aTGzdQ7sBzMYlqeGv2wbJCvb20e+QOt89ndjgeGcrwN+Mk3+INTeOZkDU2SgKFutSAAUpXMr4oFjXp/6IshrkLkh5umIqFsA+JTZkLBxEFAdAW1KUXt4VbMBfhiEzBNvZ16288u3/lvXDKsGu9siPCAtrlMKDw0RgIxuGK4Usa1GK/Z7GUFlNORRSe3dFZ9QesE7D4/UCYQx1Cb799ZszDAxDymU6RTg/L44cQydxB3OfN6tu+35RrLRNEf6UXKI3TiZv+YY0OGd0E1eibMJBjJLzH1Eoa3xrRJJGPaAilpNKNCm/EyGyT4yA1prBjPNpT53h9149BM8J22/tnGd/GnqPWStocUADXW7fkBcLRsaaPrtYl6OuWf6HJDtYfXFm3EH4ot3XCY6dqdB80gglc6MF7HluYZoy6nQVm7uF2Vv1IA1+n1wjv5O5mEpcTJ/1WQpscXSL0kn6lviY=");
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                this.DataCaptureContext = DataCaptureContext.ForLicenseKey("AZ8CJrqBLhimMAbzNgcS0r9Cv7cnLaBvHVkzZBl7+g+6TZZWBV5ys85IXYuLcEf4cRM25nxWs1QkY+pvKG4RDzB5RkcCcbUN2D3JhuptsHaAHpUfYWoVUfQd3YB1CYW8ZCP0acczB1hgP9lzjNfUof+8Ef/x/uXFvzAMPBRQCHY/DQyYykU/lrQCoK9UFfnjvlcyA4QVQcF3xX7G2EuIFYYufl/iQ8ynwJAZfdcY/S+riTzRhxeIIJAG5S7w63QJ01ajsIlzr7/dzY7sPdQTiQsPJZokpe+zITMaWe0Hzk6PEZ9RC6ryVGGfi5SUTDbJ1M5bxaSyABOmEU0JXedGIsYsNa2zI1jhvE+7nLvLqBlVLtpgkgg445eKzQorxd4+b5BQphUc5OEeTteI/N+oikpcHKms6uNxvkoOsfhFesET+XNCoeKh1X0zsbxhyWATge8mbnT0tHI0R2bOl02aGJh7ZFfuH5xWPjjVvBgE70ryM/M+5TinqhShR78AW/b+UDgThjSS+Nbgi3bFPuPO32ZO4cYmjTgFFA0DovSKo0+NZQetD7lVoxjf5ZAHXPdfzZ342HoM2NNRZFqgotTRI2yM41Gn89PXfu/HOFRBuFG2wsXZahWV4U5BlkHa+KYzdYEAGlr7A7/W4PIbd6LWv2I7PNM0ppyyvwrST9FCU3+23uJXTb42VC++Xz9UYA953NWReHtQSooKbuI0vhm9+NkRvy8//lDNXhC7ePzypvxFGUBD244vklKHZovstC9+1rU/boyvHMYM0AQi9CKGWP+NIcmmnJcQN0UEeJ4aoFnP6Fsjm0WK8/o=");
            }
            
            DataCaptureContext.StatusChanged += OnStatusChanged;
        }

        #region Initialization

        public void InitializeCamera()
        {
            DataCaptureContext.SetFrameSourceAsync(CurrentCamera);
            CurrentCamera?.ApplySettingsAsync(CameraSettings);
        }

        public void InitializeBarcodeCapture()
        {
            DataCaptureContext.RemoveAllModes();

            BarcodeCaptureSettings = BarcodeCaptureSettings.Create();

            var symbologies = new HashSet<Symbology>
            {
                Symbology.Ean13Upca,
                Symbology.Ean8,
                Symbology.Upce,
                Symbology.Code39,
                Symbology.Code128,
                Symbology.InterleavedTwoOfFive
            };

            BarcodeCaptureSettings.EnableSymbologies(symbologies);
            BarcodeCapture = BarcodeCapture.Create(DataCaptureContext, BarcodeCaptureSettings);
        }

        #endregion

        #region DataCaptureContext

        public DataCaptureContext DataCaptureContext { get; }

        #endregion

        #region Camera

        public Camera CurrentCamera { get; } = Camera.GetDefaultCamera();
        public CameraSettings CameraSettings { get; } = BarcodeCapture.RecommendedCameraSettings;

        #endregion

        #region BarcodeCapture

        public BarcodeCapture BarcodeCapture { get; private set; }
        public BarcodeCaptureSettings BarcodeCaptureSettings { get; private set; }

        #endregion

        private void OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            IsLicenseValid = e.Status.Valid;
        }
    }
}
