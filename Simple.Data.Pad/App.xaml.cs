﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Simple.Data.Pad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //Only English Culture is supported
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            DispatcherUnhandledException += App_DispatcherUnhandledException;
            base.OnStartup(e);
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var unhandledExceptionDialog = new UnhandledExceptionDialog(e.Exception);
            e.Handled = unhandledExceptionDialog.ShowDialog().GetValueOrDefault();
        }
    }
}
