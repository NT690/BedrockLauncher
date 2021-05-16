﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Web.UI.WebControls;
using System.Windows.Documents;
using BedrockLauncher.Methods;

namespace BedrockLauncher.Pages.Common
{
    /// <summary>
    /// Логика взаимодействия для ErrorScreen.xaml
    /// </summary>
    public partial class ErrorScreen : Page
    {
        public ErrorScreen()
        {
            InitializeComponent();
        }
        private void ErrorScreenCloseButton_Click(object sender, RoutedEventArgs e)
        {
            // As i understand it not only hide error screen overlay, but also clear it from memory
            ConfigManager.ViewModel.SetDialogFrame(null);
        }

        private void ErrorScreenViewCrashButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", $@"{Environment.CurrentDirectory}\Log.txt");
        }
    }
    public static class ErrorScreenShow
    {
        public static void exceptionmsg(string title, Exception error = null)
        {
            ErrorScreen errorScreen = new ErrorScreen();
            // Show default error message
            if (error == null)
            {
                ConfigManager.ViewModel.SetDialogFrame(new ErrorScreen());
            }
            errorScreen.ErrorType.SetResourceReference(TextBlock.TextProperty, title);
            errorScreen.ErrorText.SetResourceReference(TextBlock.TextProperty, error.Message);
            ConfigManager.ViewModel.SetDialogFrame(errorScreen);
        }
        public static void exceptionmsg(Exception error = null)
        {
            ErrorScreen errorScreen = new ErrorScreen();
            // Show default error message
            if (error == null)
            {
                ConfigManager.ViewModel.SetDialogFrame(new ErrorScreen());
            }
            errorScreen.ErrorType.SetResourceReference(TextBlock.TextProperty, error.HResult);
            errorScreen.ErrorText.SetResourceReference(TextBlock.TextProperty, error.Message);
            ConfigManager.ViewModel.SetDialogFrame(errorScreen);
        }

        public static void errormsg(string error = null)
        {
            ErrorScreen errorScreen = new ErrorScreen();
            // Show default error message
            if (error == null)
            {
                ConfigManager.ViewModel.SetDialogFrame(new ErrorScreen());
            }
            switch (error)
            {
                case "autherror":
                    errorScreen.ErrorType.SetResourceReference(TextBlock.TextProperty, "Error_AuthenticationFailed_Title");
                    errorScreen.ErrorText.SetResourceReference(TextBlock.TextProperty, "Error_AuthenticationFailed");
                    ConfigManager.ViewModel.SetDialogFrame(errorScreen);
                    break;
                case "CantFindJavaLauncher":
                    errorScreen.ErrorType.SetResourceReference(TextBlock.TextProperty, "Error_CantFindJavaLauncher_Title");
                    errorScreen.ErrorText.SetResourceReference(TextBlock.TextProperty, "Error_CantFindJavaLauncher");
                    ConfigManager.ViewModel.SetDialogFrame(errorScreen);
                    break;
                case "CantFindExternalLauncher":
                    errorScreen.ErrorType.SetResourceReference(TextBlock.TextProperty, "Error_CantFindExternalLauncher_Title");
                    errorScreen.ErrorText.SetResourceReference(TextBlock.TextProperty, "Error_CantFindExternalLauncher");
                    ConfigManager.ViewModel.SetDialogFrame(errorScreen);
                    break;
                case "appregistererror":
                    errorScreen.ErrorType.SetResourceReference(TextBlock.TextProperty, "Error_AppReregisterFailed_Title");
                    errorScreen.ErrorText.SetResourceReference(TextBlock.TextProperty, "Error_AppReregisterFailed");
                    ConfigManager.ViewModel.SetDialogFrame(errorScreen);
                    break;
                case "applauncherror":
                    errorScreen.ErrorType.SetResourceReference(TextBlock.TextProperty, "Error_AppLaunchFailed_Title");
                    errorScreen.ErrorText.SetResourceReference(TextBlock.TextProperty, "Error_AppLaunchFailed");
                    ConfigManager.ViewModel.SetDialogFrame(errorScreen);
                    break;
                case "downloadfailederror":
                    errorScreen.ErrorType.SetResourceReference(TextBlock.TextProperty, "Error_AppDownloadFailed_Title");
                    errorScreen.ErrorText.SetResourceReference(TextBlock.TextProperty, "Error_AppDownloadFailed");
                    ConfigManager.ViewModel.SetDialogFrame(errorScreen);
                    break;
                case "notaskinpack":
                    errorScreen.ErrorType.SetResourceReference(TextBlock.TextProperty, "Error_NotaSkinPack_Title");
                    errorScreen.ErrorText.SetResourceReference(TextBlock.TextProperty, "Error_NotaSkinPack");
                    ConfigManager.ViewModel.SetDialogFrame(errorScreen);
                    break;
                case "extractionfailed":
                    errorScreen.ErrorType.SetResourceReference(TextBlock.TextProperty, "Error_AppExtractionFailed_Title");
                    errorScreen.ErrorText.SetResourceReference(TextBlock.TextProperty, "Error_AppExtractionFailed");
                    ConfigManager.ViewModel.SetDialogFrame(errorScreen);
                    break;
                case "CantFindPaidServerList":
                    errorScreen.ErrorType.SetResourceReference(TextBlock.TextProperty, "Error_CantFindPaidServerList_Title");
                    errorScreen.ErrorText.SetResourceReference(TextBlock.TextProperty, "Error_CantFindPaidServerList");
                    ConfigManager.ViewModel.SetDialogFrame(errorScreen);
                    break;
            }
        }
    }
}