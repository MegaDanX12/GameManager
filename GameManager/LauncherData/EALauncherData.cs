using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameManager.LauncherData
{
    /// <summary>
    /// Informazioni sul launcher EA.
    /// </summary>
    public static class EALauncherData
    {
        /// <summary>
        /// Percorso di installazione del launcher.
        /// </summary>
        public static string? LauncherPath { get; private set; }

        /// <summary>
        /// Percorso libreria.
        /// </summary>
        public static string? LibraryPath { get; private set; }

        /// <summary>
        /// Recupera informazioni sul launcher di Origin.
        /// </summary>
        public static void RetrieveEALauncherData()
        {
            LauncherPath = Path.GetDirectoryName(FindEALauncherPath());
            if (LauncherPath is not null )
            {
                LibraryPath = FindEALibraryPath();
            }
            else
            {
                LibraryPath = null;
            }
        }

        /// <summary>
        /// Recupera il percorso di installazione di Origin.
        /// </summary>
        /// <returns>Il percorso di installazione.</returns>
        private static string? FindEALauncherPath()
        {
            using RegistryKey? OriginKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\WOW6432Node\Electronic Arts\EA Desktop");
            if (OriginKey is not null)
            {
                return (string?)OriginKey.GetValue("DesktopAppPath");
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Recupera il percorso della libreria di Origin.
        /// </summary>
        /// <returns>Il percorso della libreria.</returns>
        private static string? FindEALibraryPath()
        {
            try
            {
                DirectoryInfo EADesktopAppDataFolder = new(Environment.GetEnvironmentVariable("LocalAppData") + "\\Electronic Arts\\EA Desktop");
                IEnumerable<FileInfo> UserFiles = EADesktopAppDataFolder.EnumerateFiles("user_*.ini");
                foreach (FileInfo file in UserFiles)
                {
                    using StreamReader ConfigFile = new(File.OpenRead(file.FullName));
                    string Line = string.Empty;
                    while (!Line.Contains("user.downloadinplacedir"))
                    {
                        Line = ConfigFile.ReadLine()!;
                    }
                    return Line.Replace("user.downloadinplacedir=", string.Empty);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}