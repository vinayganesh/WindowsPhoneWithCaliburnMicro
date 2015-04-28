using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using Windows.UI.Popups;
using CaliburnMicroWIndowsPhone.IServices;
using CaliburnMicroWIndowsPhone.Services;
using CaliburnMicroWIndowsPhone.ViewModels;
using CaliburnMicroWIndowsPhone.Views;

namespace CaliburnMicroWIndowsPhone
{
    public sealed partial class App 
    {
        private WinRTContainer _container;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        #region Caliburn Micro Methods
        
        protected override void Configure()
        {
            MessageBinder.SpecialValues.Add("$clickeditem", c => ((ItemClickEventArgs)c.EventArgs).ClickedItem);

            _container = new WinRTContainer();

            _container.RegisterWinRTServices();

            RegisterViewModels();
            RegisterServices();
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            args.Window.VisibilityChanged += Window_VisibilityChanged;
        }

        private void Window_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (!e.Visible)
            {
                OnExitAppNotification();
            }
        }

        private void OnExitAppNotification()
        {

        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            _container.RegisterNavigationService(rootFrame);
        }

        private void RegisterServices()
        {
            _container.Singleton<IEmulateContactsService, EmulateContactsService>();
        }

        private void RegisterViewModels()
        {
            _container.PerRequest<EmulateContactsViewModel>();
        }

        private void DisplayRootView()
        {
            DisplayRootView<EmulateContactsView>();
        }

        private static void PreInitialization()
        {
            var statusBar = StatusBar.GetForCurrentView();
            if (statusBar != null)
            {
                statusBar.BackgroundColor = Colors.Gray;
                statusBar.BackgroundOpacity = 1;
                statusBar.HideAsync();
            }
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        #endregion

        #region App life cycle

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            PreInitialization();
            DisplayRootView();

        }

        protected override void OnActivated(IActivatedEventArgs args)
        {

        }

        protected override void OnSuspending(object sender, SuspendingEventArgs e)
        {

        }

        #endregion

        protected async override void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                //
            }
#if DEBUG
            e.Handled = true;
            var md = new MessageDialog(e.ToString());
            await md.ShowAsync();
#else
             base.OnUnhandledException(sender, e);
#endif
        }

    }
}