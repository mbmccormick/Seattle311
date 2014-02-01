using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Public311.API;
using Public311.Common;
using Public311.API.Models;
using System.Text;
using Microsoft.WindowsAzure.MobileServices;
using Public311.Models;

namespace Public311
{
    public partial class App : Application
    {
        public static ServiceClient Public311Client;

        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://public311.azure-mobile.net/",
            "KswcDnwXqjrNlPScGYNSZcnkGUzUdk25"
        );

        public static Endpoint CurrentEndpoint;
  
        public static string FeedbackEmailAddress = "feedback@mbmccormick.com";
        public static event EventHandler<ApplicationUnhandledExceptionEventArgs> UnhandledExceptionHandled;
        
        public static string ExtendedVersionNumber
        {
            get
            {
                string assembly = System.Reflection.Assembly.GetExecutingAssembly().FullName;
                string[] version = assembly.Split('=')[1].Split(',')[0].Split('.');

                return version[0] + "." + version[1] + "." + version[2];
            }
        }

        public static string PlatformVersionNumber
        {
            get
            {
                return System.Environment.OSVersion.Version.ToString(3);
            }
        }

        public static PhoneApplicationFrame RootFrame { get; private set; }

        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            if (CurrentEndpoint != null)
                InitializePublic311Client();

            if (System.Diagnostics.Debugger.IsAttached)
                MetroGridHelper.IsVisible = true;

            RootFrame.Navigating += new NavigatingCancelEventHandler(RootFrame_Navigating);
        }

        public static void InitializePublic311Client()
        {
            Public311Client = new ServiceClient(CurrentEndpoint.ServerAddress, CurrentEndpoint.APIKey);

            Public311Client.ImgurServerAddress = "api.imgur.com";
            Public311Client.ImgurAPIKey = "8fdb6a32174203e";

            Public311Client.UserData.device_id = ExtendedPropertiesHelper.DeviceUniqueID;
            Public311Client.UserData.account_id = ExtendedPropertiesHelper.WindowsLiveAnonymousID;
        }

        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            SmartDispatcher.Initialize(RootFrame.Dispatcher);
        }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            CurrentEndpoint = IsolatedStorageHelper.GetObject<Endpoint>("CurrentEndpoint");
        }

        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            Public311Client.SaveData();

            IsolatedStorageHelper.SaveObject<Endpoint>("CurrentEndpoint", CurrentEndpoint);
        }

        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            Public311Client.SaveData();

            IsolatedStorageHelper.SaveObject<Endpoint>("CurrentEndpoint", CurrentEndpoint);
        }

        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        private void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri.ToString().Contains("/MainPage.xaml") == false) return;
            
            if (CurrentEndpoint == null)
            {
                e.Cancel = true;
                RootFrame.Dispatcher.BeginInvoke(delegate
                {
                    RootFrame.Navigate(new Uri("/WelcomePage.xaml", UriKind.Relative));
                });
            }
            else
            {
                RootFrame.Dispatcher.BeginInvoke(delegate
                {
                    RootFrame.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                });
            }
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            LittleWatson.ReportException(e.ExceptionObject, null);

            RootFrame.Dispatcher.BeginInvoke(() =>
            {
                LittleWatson.CheckForPreviousException(false);
            });

            e.Handled = true;

            if (UnhandledExceptionHandled != null)
                UnhandledExceptionHandled(sender, e);

            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            RootFrame.Navigated += CheckForResetNavigation;

            GlobalLoading.Instance.Initialize(RootFrame);

            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion
    }
}
