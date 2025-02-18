﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ORB4
{

    static class Program
    {
#if DEBUG
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

#if DEBUG
            AllocConsole();
#endif

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!System.IO.Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\ORB"))
                System.IO.Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\ORB\\Private");

            System.IO.File.WriteAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\ORB\\Version", Engine.Version, System.Text.Encoding.ASCII);

            Logger.MainLogger = new Logger();
            Logger.MainLogger.Start();

            Logger.MainLogger.Log(Logger.LogTypes.Info, "The program is running!");

            ThumbnailDownloader.MainThumbnailDownloader = new ThumbnailDownloader();

            try
            {
                Application.Run(new MainWindow());
            } catch (Exception e)
            {
                MessageBox.Show("Oops... That's an error: " + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.MainLogger.Log(Logger.LogTypes.Error, e.ToString());
            }
            finally
            {
                Logger.MainLogger.Stop().GetAwaiter().GetResult();
                ThumbnailDownloader.MainThumbnailDownloader.Dispose();
            }

            Environment.Exit(0);
        }
    }
}
