namespace POMuswick
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AppState _appState;
        public App(AppState appState,IServiceProvider serviceProvider)
        {
            InitializeComponent();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Njk0OTc0QDMyMzAyZTMyMmUzMFdNblBzcjZVWWc5Q0VMdHZRdXQxeFYyRGhGdlF5ZGIzUjQ2VGdLU2ZBbGM9");
            _appState = appState;
            _serviceProvider = serviceProvider;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var shell = _serviceProvider.GetRequiredService<AppShell>();
            var window = new Window(shell);
        //For Future user 
        // window.Created += OnAppCreated;
        // window.Activated += OnAppActivated;
        // window.Deactivated += OnAppDeactivated;
        // window.Stopped += OnAppStopped;
        // window.Resumed += OnAppResumed;
            window.Activated += OnResume;
            
            return window;
        }

        private void OnResume(object? sender, EventArgs e)
        {
            _appState.AppResume();
        }
    }
}
