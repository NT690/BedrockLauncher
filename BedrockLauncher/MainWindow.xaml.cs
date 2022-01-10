﻿using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Forms.Design;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows.Data;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Management.Core;
using Windows.Management.Deployment;
using Windows.System;
using BedrockLauncher.Methods;
using BedrockLauncher.Classes;
using System.Windows.Media.Animation;
using BedrockLauncher.Pages;
using BedrockLauncher.Pages.FirstLaunch;
using BedrockLauncher.ViewModels;
using BedrockLauncher.Pages.Settings;
using BedrockLauncher.Pages.Play;
using BedrockLauncher.Pages.News;
using BedrockLauncher.Pages.Preview;
using BedrockLauncher.Pages.Common;
using BedrockLauncher.Pages.Community;
using BedrockLauncher.Components;
using BedrockLauncher.Controls.Toolbar;

namespace BedrockLauncher
{
    //TODO: (Later On) Community Content / Personal Donations Section

    public partial class MainWindow : Window
    {
        private GameTabs MainPage = new GameTabs();
        private SettingsTabs settingsScreenPage = new SettingsTabs();
        private NewsScreenTabs newsScreenPage = new NewsScreenTabs(ViewModels.MainViewModel.Updater);
        private CommunityPage communityPage = new CommunityPage();

        private Navigator Navigator { get; set; } = new Navigator(true);

        public MainWindow()
        {
            this.DataContext = MainViewModel.Default;
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime) Init();
        }

        private void Init()
        {
            Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnLastWindowClose;

            Panel.SetZIndex(OverlayFrame, 0);
            Panel.SetZIndex(ErrorFrame, 1);
            Panel.SetZIndex(UpdateButton, 2);

            MainPage.skinsPage.InitFrameEvents(ErrorFrame);
            MainPage.skinsPage.InitFrameEvents(OverlayFrame);

            UpdateButton.ClickBase.Click += MainViewModel.Updater.UpdateButton_Click;
            MainViewModel.Default.Init(MainFrame);

            bool isFirstLaunch = Properties.LauncherSettings.Default.CurrentProfile == "" ||
                Properties.LauncherSettings.Default.IsFirstLaunch ||
                MainViewModel.Default.Config.profiles.Count() == 0;

            ButtonManager_Base(BedrockEditionButton.Name);
            if (isFirstLaunch) MainViewModel.Default.SetOverlayFrame_Strict(new WelcomePage());
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MainViewModel.Default.AttemptClose(sender, e);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Default.PackageManager.CancelTask();
        }

        #region Navigation

        public void ResetButtonManager(string buttonName)
        {
            this.Dispatcher.Invoke(() =>
            {
                // just all buttons list
                // ya i know this is really bad, i need to learn mvvm instead of doing this shit
                // but this works fine, at least
                List<ToggleButton> toggleButtons = new List<ToggleButton>() { 
                // main window
                CommunityButton.Button,
                NewsButton.Button,
                BedrockEditionButton.Button,
                JavaEditionButton.Button,
                SettingsButton.Button,
            };

                foreach (ToggleButton button in toggleButtons) { button.IsChecked = false; }

                if (toggleButtons.Exists(x => x.Name == buttonName))
                {
                    toggleButtons.Where(x => x.Name == buttonName).FirstOrDefault().IsChecked = true;
                }
            });

        }
        public void ButtonManager(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var toggleButton = sender as ToggleButton;
                string name = toggleButton.Name;
                Task.Run(() => ButtonManager_Base(name));
            });
        }
        public void ButtonManager_Base(string senderName)
        {
            this.Dispatcher.Invoke(() =>
            {
                ResetButtonManager(senderName);

                if (senderName == BedrockEditionButton.Name) NavigateToMainPage();
                else if (senderName == NewsButton.Name) NavigateToNewsPage();
                else if (senderName == JavaEditionButton.Name) NavigateToJavaLauncher();
                else if (senderName == ExternalLauncherButton.Name) NavigateToExternalLauncher();
                else if (senderName == CommunityButton.Name) NavigateToCommunityScreen();
                else if (senderName == SettingsButton.Name) NavigateToSettings();
            });

        }

        public void NavigateToNewsPage()
        {
            this.Dispatcher.Invoke(() =>
            {
                Navigator.UpdatePageIndex(0);
                NewsButton.Button.IsChecked = true;
                Task.Run(() => Navigator.Navigate(MainWindowFrame, newsScreenPage));
            });

        }
        public void NavigateToMainPage()
        {
            this.Dispatcher.Invoke(() =>
            {
                Navigator.UpdatePageIndex(1);
                BedrockEditionButton.Button.IsChecked = true;
                Task.Run(() => Navigator.Navigate(MainWindowFrame, MainPage));
            });

        }
        public void NavigateToCommunityScreen()
        {
            this.Dispatcher.Invoke(() =>
            {
                Navigator.UpdatePageIndex(3);
                CommunityButton.Button.IsChecked = true;
                Task.Run(() => Navigator.Navigate(MainWindowFrame, communityPage));
            });
        }
        public void NavigateToSettings()
        {
            this.Dispatcher.Invoke(() =>
            {
                Navigator.UpdatePageIndex(4);
                SettingsButton.Button.IsChecked = true;
                Task.Run(() => Navigator.Navigate(MainWindowFrame, settingsScreenPage));
            });

        }
        public async void NavigateToJavaLauncher()
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                Action action = new Action(() =>
                {
                    string JavaPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + @"\Programs\Minecraft Launcher\Minecraft Launcher.lnk";

                    try
                    {
                        // Trying to find and open Java launcher shortcut
                        Process.Start(JavaPath);
                        if (Properties.LauncherSettings.Default.CloseLauncherOnSwitch) Application.Current.MainWindow.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(string.Format("CantFindJavaLauncher: {0}", JavaPath));
                        Debug.WriteLine(ex);
                        MainPage.NavigateToPlayScreen();
                        ErrorScreenShow.errormsg("Error_CantFindJavaLauncher_Title", "Error_CantFindJavaLauncher", ex);
                    }
                });

                NavigateToOtherLauncher(action);
            });
        }
        public async void NavigateToExternalLauncher()
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                Action action = new Action(() =>
                {
                    string LauncherPath = Properties.LauncherSettings.Default.ExternalLauncherPath;
                    string Arguments = Properties.LauncherSettings.Default.ExternalLauncherArguments;
                    try
                    {
                        Process.Start(LauncherPath, Arguments);
                        if (Properties.LauncherSettings.Default.CloseLauncherOnSwitch) Application.Current.MainWindow.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(string.Format("CantFindExternalLauncher:\n\nPath: {0}\nArguments: {1}", LauncherPath, Arguments));
                        Debug.WriteLine(ex);
                        MainPage.NavigateToPlayScreen();
                        ErrorScreenShow.errormsg("Error_CantFindExternalLauncher_Title", "Error_CantFindExternalLauncher", ex);
                    }
                });

                NavigateToOtherLauncher(action);
            });

        }
        public async void NavigateToOtherLauncher(Action action)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (Properties.LauncherSettings.Default.CloseLauncherOnSwitch && MainViewModel.Default.PackageManager.isGameRunning)
                {
                    Task.Run(() => MainViewModel.Default.ShowPrompt_ClosingWithGameStillOpened(action));
                }
                else action.Invoke();
            });

        }
        public void NavigateToNewProfilePage()
        {
            this.Dispatcher.Invoke(() =>
            {
                MainViewModel.Default.SetOverlayFrame(new AddProfilePage());
            });

        }

        #endregion

        #region Toolbar Button Events

        private void BedrockEditionButton_Click(object sender, EventArgs e)
        {
            if (sender != null && sender is ToolbarButtonBase) ButtonManager_Base((sender as ToolbarButtonBase).Name);
        }

        private void NewsButton_Click(object sender, EventArgs e)
        {
            if (sender != null && sender is ToolbarButtonBase) ButtonManager_Base((sender as ToolbarButtonBase).Name);
        }

        private void ProfileButton_Click(object sender, EventArgs e)
        {
            NavigateToNewProfilePage();
        }

        private void JavaEditionButton_Click(object sender, EventArgs e)
        {
            NavigateToJavaLauncher();
        }

        private void ExternalLauncherButton_Click(object sender, EventArgs e)
        {
            NavigateToExternalLauncher();
        }

        private void CommunityButton_Click(object sender, EventArgs e)
        {
            if (sender != null && sender is ToolbarButtonBase) ButtonManager_Base((sender as ToolbarButtonBase).Name);
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            if (sender != null && sender is ToolbarButtonBase) ButtonManager_Base((sender as ToolbarButtonBase).Name);
        }

        #endregion
    }
}
