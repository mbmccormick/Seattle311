using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Seattle311.Resources;
using Seattle311.API;
using Seattle311.Common;

namespace Seattle311
{
    public partial class App : Application
    {
        public static ServiceClient Seattle311Client;

        public static PhoneApplicationFrame RootFrame { get; private set; }

        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            if (Debugger.IsAttached == true)
            {
                Seattle311Client = new ServiceClient("test311request-cityofchicago-org-aik24htrwt73.runscope.net", "30d67e348eed17834cded54d730fdeda");
                // Seattle311Client = new ServiceClient("servicerequest--qa-seattle-gov-aik24htrwt73.runscope.net", "74187328b9dd2f1c7d0d82485d9523c4"); // development
                // Seattle311Client = new ServiceClient("servicerequest-seattle-gov-aik24htrwt73.runscope.net", "c047fae2a21cde1304f6b733b54b9e02"); // production

                Seattle311Client.ImgurServerAddress = "api-imgur-com-aik24htrwt73.runscope.net";
                Seattle311Client.ImgurAPIKey = "8fdb6a32174203e";
            }
            else
            {
                Seattle311Client = new ServiceClient("test311request.cityofchicago.org", "30d67e348eed17834cded54d730fdeda");
                // Seattle311Client = new ServiceClient("servicerequest-qa.seattle.gov", "74187328b9dd2f1c7d0d82485d9523c4"); // development
                // Seattle311Client = new ServiceClient("servicerequest.seattle.gov", "c047fae2a21cde1304f6b733b54b9e02"); // production

                Seattle311Client.ImgurServerAddress = "api.imgur.com";
                Seattle311Client.ImgurAPIKey = "8fdb6a32174203e";
            }
        }

        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            SmartDispatcher.Initialize(RootFrame.Dispatcher);
        }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
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

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
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
